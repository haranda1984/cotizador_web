using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using HeiLiving.Quotes.Api.Models;
using DinkToPdf;
using HeiLiving.Quotes.Api.Helpers;
using Microsoft.Extensions.Configuration;
using HubSpot.Core;
using HubSpot.Api.Contact.Dto;
using HubSpot.Api.Deal.Dto;
using Microsoft.Extensions.Logging;

namespace HeiLiving.Quotes.Api.Services
{
    public class QuotesService : IQuotesService
    {
        private readonly IProjectsService _projectService;
        private readonly IUnitService _unitService;
        private readonly IUnitEquipmentsService _unitEquipmentService;
        private readonly IUnitRatesService _unitRatesService;
        private readonly ICondoHotelExpensesService _condoHotelExpensesService;
        private readonly ITemporalitiesService _temporalityService;
        private readonly ITradePoliciesService _tradePolicyService;
        private readonly IStageService _stageService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public QuotesService(IUserService userService, IProjectsService projectService, IUnitService unitService, IUnitEquipmentsService unitEquipmentService, IUnitRatesService unitRatesService,
                                ITemporalitiesService temporalityService, ICondoHotelExpensesService condoHotelExpensesService, ITradePoliciesService tradePolicyService, IStageService stageService,
                                IConfiguration configuration, ILogger<QuotesService> logger)
        {
            _userService = userService;
            _projectService = projectService;
            _unitService = unitService;
            _unitEquipmentService = unitEquipmentService;
            _unitRatesService = unitRatesService;
            _condoHotelExpensesService = condoHotelExpensesService;
            _temporalityService = temporalityService;
            _tradePolicyService = tradePolicyService;
            _stageService = stageService;
            _configuration = configuration;
            _logger = logger;
        }

        public QuoteResultViewModel Calculate(QuoteInputViewModel input)
        {
            var project = _projectService.GetProjectByIdAsync(input.ProjectId).Result;
            var unit = _unitService.GetUnitByIdAsync(input.UnitId).Result;

            var exchangeRate = ExchangeRate.GetRate("usd");

            var unitPrice = unit.UnitPrices.Where(p => p.StageId.ToString() == unit.Model.Project.CurrentStageId.ToString()).FirstOrDefault();
            var priceList = unitPrice.Currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase)
                            ? Math.Round(unitPrice.Price * exchangeRate, 2)
                            : Math.Round(unitPrice.Price, 2);
            var minimumExpectedValue = unitPrice.Currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase)
                                        ? unit.UnitPrices.Max(x => x.Price * exchangeRate)
                                        : unit.UnitPrices.Max(x => x.Price);

            var capRate = project.CapRate.HasValue ? project.CapRate.Value : Formulas.CapRate();

            var result = new QuoteResultViewModel
            {
                CapitalGain = Math.Round(minimumExpectedValue - input.Price, 4),
                CapRateInvestment = Math.Round((minimumExpectedValue * capRate) / input.Price, 4),
                CapRatePriceFinal = Math.Round((minimumExpectedValue * capRate) / minimumExpectedValue, 4),
                Details = GetAdditionalPaymentsDetails(input.Deposit, input.LastPayment, input.AdditionalPayments, input.Date, project.DeliveryDate),
                Investment = Math.Round(input.Price, 2),
                MinimumExpectedValue = Math.Round(minimumExpectedValue, 2),
                ProductName = project.DisplayName,
                PriceList = priceList,
                RentPerMonth = Math.Round((minimumExpectedValue * capRate) / 12, 2),
                RentPerYear = Math.Round(minimumExpectedValue * capRate, 2),
                ReturnOfInvestment = Math.Round((minimumExpectedValue - input.Price) / input.Price, 4),
                Currency = (!string.IsNullOrEmpty(input.Currency) && input.Currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase) ? "usd" : "mxn"),
                ExchangeRate = Math.Round(exchangeRate, 2)
            };

            if (input.CondoHotel && project.AllowsCondoHotel)
            {
                var unitEquipmentResult = _unitEquipmentService.GetAllUnitEquipmentsByProjectIdAsync(input.ProjectId).Result.FirstOrDefault(x => x.UnitId.Equals(input.UnitId));
                var unitRateResult = _unitRatesService.GetAllUnitRatesByUnitIdAsync(input.UnitId).Result.FirstOrDefault();
                var temporalityResult = _temporalityService.GetAllTemporalitiesByProjectIdAsync(input.ProjectId).Result;
                var condoHotelExpensesResult = _condoHotelExpensesService.GetAllCondoHotelExpensesByProjectIdAsync(input.ProjectId).Result;
                var condoHotelExpensesSelected = input.HasOptionalExpenses
                                                ? condoHotelExpensesResult.Where(x => !x.IsOptional
                                                                || (!input.OptionalExpenses.Select(x => x.ExpenseName).ToList().Contains(x.Expense.Name)))
                                                : condoHotelExpensesResult.Where(x => !x.IsOptional);

                var unitRate = unitRateResult.CostPerNight.HasValue
                                ? unitRateResult.CostPerNight.Value
                                : (unitRateResult.BuiltUpAreaCost.HasValue && unitRateResult.TerraceAreaCost.HasValue)
                                    ? (unit.BuiltUpArea * unitRateResult.BuiltUpAreaCost.Value) + (unit.TerraceArea * unitRateResult.TerraceAreaCost.Value)
                                    : 0m;
                var ratePerNight = temporalityResult.Average(x => unitRate * x.Percentage / 100);
                var annualOccupancyMax = temporalityResult.Sum(x => x.OccupationInDaysMax) / 365;
                var annualOccupancy = (input.CondoHotelOccupancy.HasValue && input.CondoHotelOccupancy.Value > 0)
                                    ? input.CondoHotelOccupancy.Value / 100
                                    : Math.Ceiling(temporalityResult.Sum(x => x.OccupationInDays) / 365 * 20) / 20;
                var annualIncome = (input.CondoHotelOccupancy.HasValue && input.CondoHotelOccupancy.Value > 0)
                                    ? (input.CondoHotelOccupancy.Value / 100 * 365) * ratePerNight
                                    : temporalityResult.Sum(x => (unitRate * x.Percentage / 100) * x.OccupationInDays);

                var costOperationIncome = annualIncome * condoHotelExpensesSelected.Where(x => x.Expense.Type.Equals(0) && x.Percentage > 0).Sum(x => x.Percentage / 100);
                costOperationIncome += condoHotelExpensesSelected.Where(x => x.Expense.Type.Equals(0) && x.Cost > 0).Sum(x => x.Cost);
                var managementFeeIncome = annualIncome * condoHotelExpensesSelected.Where(x => x.Expense.Type.Equals(2) && x.Percentage > 0).Sum(x => x.Percentage / 100);
                managementFeeIncome += condoHotelExpensesSelected.Where(x => x.Expense.Type.Equals(2) && x.Cost > 0).Sum(x => x.Cost);

                var netIncome = annualIncome - (costOperationIncome + managementFeeIncome);

                var expensesDetails = condoHotelExpensesResult.Where(x => x.Expense.Type == 0 || x.Expense.Type == 1).Select(x => new CondoHotelExpenseDetailViewModel
                {
                    ExpenseName = x.Expense.Name,
                    Percentage = x.Percentage > 0
                                     ? Math.Round((x.Percentage / 100), 4)
                                     : (x.Expense.Type == 0
                                        ? Math.Round((x.Cost / annualIncome), 4)
                                        : Math.Round((x.Cost / netIncome), 4)),
                    Cost = x.Cost > 0
                                ? Math.Round(x.Cost, 2)
                                : (x.Expense.Type == 0
                                    ? Math.Round((annualIncome * (x.Percentage / 100)), 2)
                                    : Math.Round((netIncome * (x.Percentage / 100)), 2)),
                    Type = x.Expense.Type,
                    DependsOnOccupancy = x.Percentage > 0,
                    IsOptional = x.IsOptional
                }).ToList();

                var feeDetails = condoHotelExpensesResult.Where(x => x.Expense.Type == 2 || x.Expense.Type == 3).Select(x => new CondoHotelExpenseDetailViewModel
                {
                    ExpenseName = x.Expense.Name,
                    Percentage = x.Percentage > 0
                                 ? Math.Round((x.Percentage / 100), 4)
                                 : (x.Expense.Type == 2
                                    ? Math.Round((x.Cost / annualIncome), 4)
                                    : Math.Round((x.Cost / netIncome), 4)),
                    Cost = x.Cost > 0
                            ? Math.Round(x.Cost, 2)
                            : (x.Expense.Type == 2
                                ? Math.Round((annualIncome * (x.Percentage / 100)), 2)
                                : Math.Round((netIncome * (x.Percentage / 100)), 2)),
                    Type = x.Expense.Type,
                    DependsOnOccupancy = x.Percentage > 0,
                    IsOptional = x.IsOptional
                }).ToList();

                var costOperationProfit = 0m;
                var managementFeeProfit = 0m;
                if (netIncome > 0)
                {
                    costOperationProfit = netIncome * condoHotelExpensesSelected.Where(x => x.Expense.Type.Equals(1)).Sum(x => x.Percentage / 100);
                    managementFeeProfit = netIncome * condoHotelExpensesSelected.Where(x => x.Expense.Type.Equals(3)).Sum(x => x.Percentage / 100);
                    netIncome -= (costOperationProfit + managementFeeProfit);
                }

                var extraWeeks = new List<CondoHotelExtraWeeksResultViewModel>
                {
                    new CondoHotelExtraWeeksResultViewModel {
                        RateId = Guid.Parse("66A090A3-4602-4D2A-9693-73812223B3AA"),
                        RateName = "Temporada alta",
                        Included = 0,
                        Extra = input.ExtraWeeks != null && input.ExtraWeeks.Any(x => x.RateId.Equals(Guid.Parse("66A090A3-4602-4D2A-9693-73812223B3AA")))
                                ? input.ExtraWeeks.Where(x => x.RateId.Equals(Guid.Parse("66A090A3-4602-4D2A-9693-73812223B3AA"))).FirstOrDefault().Extra
                                : 0,
                        Multiplier = 1.15m,
                        Cost =  input.ExtraWeeks != null && input.ExtraWeeks.Any(x => x.RateId.Equals(Guid.Parse("66A090A3-4602-4D2A-9693-73812223B3AA")))
                                ? Math.Round(
                                    (annualIncome/annualOccupancy/365) *
                                    (input.ExtraWeeks.Where(x => x.RateId.Equals(Guid.Parse("66A090A3-4602-4D2A-9693-73812223B3AA"))).FirstOrDefault().Extra * 7 * 1.15m)
                                    , 4)
                                : 0,
                        Order = 0
                    },
                    new CondoHotelExtraWeeksResultViewModel {
                        RateId = Guid.Parse("D95AE993-3328-49EF-8403-2E2D62FE7EF1"),
                        RateName = "Temporada media",
                        Included = 2,
                        Extra = input.ExtraWeeks != null && input.ExtraWeeks.Any(x => x.RateId.Equals(Guid.Parse("D95AE993-3328-49EF-8403-2E2D62FE7EF1")))
                                ? input.ExtraWeeks.Where(x => x.RateId.Equals(Guid.Parse("D95AE993-3328-49EF-8403-2E2D62FE7EF1"))).FirstOrDefault().Extra
                                : 0,
                        Multiplier = .9m,
                        Cost =  input.ExtraWeeks != null && input.ExtraWeeks.Any(x => x.RateId.Equals(Guid.Parse("D95AE993-3328-49EF-8403-2E2D62FE7EF1")))
                                ? Math.Round(
                                    (annualIncome/annualOccupancy/365) *
                                    (input.ExtraWeeks.Where(x => x.RateId.Equals(Guid.Parse("D95AE993-3328-49EF-8403-2E2D62FE7EF1"))).FirstOrDefault().Extra * 7 * .9m)
                                    , 4)
                                : 0,
                        Order = 1
                    },
                    new CondoHotelExtraWeeksResultViewModel {
                        RateId = Guid.Parse("3DA2FA78-5D1C-4964-9468-DA3051DEED6C"),
                        RateName = "Temporada baja",
                        Included = 2,
                        Extra = input.ExtraWeeks != null && input.ExtraWeeks.Any(x => x.RateId.Equals(Guid.Parse("3DA2FA78-5D1C-4964-9468-DA3051DEED6C")))
                                ? input.ExtraWeeks.Where(x => x.RateId.Equals(Guid.Parse("3DA2FA78-5D1C-4964-9468-DA3051DEED6C"))).FirstOrDefault().Extra
                                : 0,
                        Multiplier = .66m,
                        Cost =  input.ExtraWeeks != null && input.ExtraWeeks.Any(x => x.RateId.Equals(Guid.Parse("3DA2FA78-5D1C-4964-9468-DA3051DEED6C")))
                                ? Math.Round(
                                    (annualIncome/annualOccupancy/365) *
                                    (input.ExtraWeeks.Where(x => x.RateId.Equals(Guid.Parse("3DA2FA78-5D1C-4964-9468-DA3051DEED6C"))).FirstOrDefault().Extra * 7 * .66m)
                                    , 4)
                                : 0,
                        Order = 2
                    }
                };
                var extraWeeksCost = extraWeeks.Sum(x => x.Cost);
                netIncome -= extraWeeksCost;

                var preOperatingCost = project.CondoHotelPreOperatingCost.HasValue ? project.CondoHotelPreOperatingCost.Value : 0m;
                var investmentTotal = input.Price + unitEquipmentResult.Cost + preOperatingCost;

                result.CondoHotel = new CondoHotelResultViewModel
                {
                    EquipmentPrice = Math.Round(unitEquipmentResult.Cost, 2),
                    PreOperatingCost = Math.Round(preOperatingCost, 2),
                    EquipmentPreOper = Math.Round((unitEquipmentResult.Cost + preOperatingCost), 2),
                    InvestmentTotal = Math.Round(investmentTotal, 2),
                    AnnualIncome = Math.Round(annualIncome, 2),
                    CostOperation = Math.Round((costOperationIncome + costOperationProfit), 2),
                    ManagementFee = Math.Round((managementFeeIncome + managementFeeProfit), 2),
                    NetAnnualIncome = Math.Round(netIncome, 2),
                    Occupancy = Formulas.RoundUp((int)(annualOccupancy * 100), 5) / 100m,
                    OccupancyMax = Formulas.RoundUp((int)(annualOccupancyMax * 100), 5) / 100m,
                    RatePerNight = Math.Round(ratePerNight, 2),
                    ReturnOfInvestment = Math.Round(netIncome / investmentTotal, 4),
                    ReturnOfInvestmentYears = Math.Round(1 / (netIncome / investmentTotal), 1),
                    // InvesmentDetails = new List<CondoHotelInvesmentDetailViewModel>
                    // {
                    //     new CondoHotelInvesmentDetailViewModel {
                    //         Name = "Precio unidad",
                    //         Amount = Math.Round(input.Price, 2)
                    //     },
                    //     new CondoHotelInvesmentDetailViewModel {
                    //         Name = "Precio equipamiento",
                    //         Amount = Math.Round(unitEquipmentResult.Cost, 2)
                    //     },
                    //     new CondoHotelInvesmentDetailViewModel {
                    //         Name = "Costo pre operativo",
                    //         Amount = Math.Round(preOperatingCost, 2)
                    //     }
                    // },
                    ExpensesDetails = expensesDetails,
                    FeeDetails = feeDetails,
                    // IncomeDetails = temporalityResult.Select(x => new CondoHotelIncomeDetailViewModel
                    // {
                    //     Month = x.Month,
                    //     MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                    //     Income = Math.Round(x.OccupationInDays * (unitRate * x.Percentage), 2)
                    // }).OrderBy(x => x.Month).ToList(),
                    // RatePerNightDetails = temporalityResult.Select(x => new CondoHotelRatePerNightDetailViewModel
                    // {
                    //     Month = x.Month,
                    //     MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                    //     Days = x.OccupationInDays,
                    //     RateType = x.Rate.Description,
                    //     RatePerNight = Math.Round(unitRate * x.Percentage / 100, 2)
                    // }).OrderBy(x => x.Month).ToList(),
                    ExtraWeeks = extraWeeks
                };
            }

            return result;
        }

        public HtmlToPdfDocument ExportSummaryToPdf(QuoteInputViewModel input, QuoteResultViewModel summary, Guid userId)
        {
            if (summary == null)
            {
                return null;
            }

            var project = _projectService.GetProjectByIdAsync(input.ProjectId).Result;

            var pdfCover = GetPdfCover(project.Id);
            var pdfIntro = GetPdfIntro(project.Id);
            var pdfAmenities = GetPdfAmenities(project.Id);
            var pdfUnit = GetPdfUnit(input);
            var pdfFinal = GetPdfFinal(project.Id);

            var pdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.Letter,
                    Margins = new MarginSettings { Top = 16.9333, Right = 16.9333, Left = 16.9333, Bottom = 15.9333},
                    DocumentTitle = "Cotización de unidad"
                }
            };
            pdfDocument.Objects.Add(pdfCover);
            pdfDocument.Objects.Add(pdfIntro);
            pdfDocument.Objects.Add(pdfAmenities);
            pdfDocument.Objects.Add(pdfUnit);

            if (input.CondoHotel && project.AllowsCondoHotel)
            {
                var pdfSummaryT = GetPdfSummaryT(input, summary);
                pdfDocument.Objects.Add(pdfSummaryT);
                var pdfSummaryTravel = GetPdfSummaryTravel(input, summary);
                pdfDocument.Objects.Add(pdfSummaryTravel);
            }
            else
            {
                var pdfSummary = GetPdfSummary(input, summary);
                pdfDocument.Objects.Add(pdfSummary);
            }
            pdfDocument.Objects.Add(pdfFinal);

            return pdfDocument;
        }

        public void CreateUpdateHubspotDeal(QuoteInputViewModel input, QuoteResultViewModel summary, Guid userId)
        {
            try
            {
                var project = _projectService.GetProjectByIdAsync(input.ProjectId).Result;
                var unit = _unitService.GetUnitByIdAsync(input.UnitId).Result;
                var tradePolicy = _tradePolicyService.GetTradePolicyByIdAsync(input.TradePolicyId).Result;
                var unitPrice = unit.UnitPrices.Where(p => p.StageId.ToString() == unit.Model.Project.CurrentStageId.ToString()).FirstOrDefault();
                var unitEquipmentResult = _unitEquipmentService.GetAllUnitEquipmentsByProjectIdAsync(input.ProjectId).Result.FirstOrDefault(x => x.UnitId.Equals(input.UnitId));
                var preOperatingCost = project.CondoHotelPreOperatingCost.HasValue ? project.CondoHotelPreOperatingCost.Value : 0m;
                var investmentTotal = input.Price + (unitEquipmentResult != null ? unitEquipmentResult.Cost : 0) + preOperatingCost;
                var priceDiscount = unitPrice.Price * input.Discount / 100;
                var finalPrice = unitPrice.Price - priceDiscount;
                var isCustomTradePolicy = (tradePolicy.Discount * 100) != input.Discount
                                        || Math.Truncate((finalPrice * tradePolicy.Deposit)) != Math.Truncate(input.Deposit)
                                        || tradePolicy.MonthlyPayments != input.AdditionalPayments.Count()
                                        || Math.Truncate((finalPrice * tradePolicy.AdditionalPayments)) != Math.Truncate(input.AdditionalPayments.Sum(x => x.Amount))
                                        || Math.Truncate((finalPrice * tradePolicy.LastPayment)) != Math.Truncate(input.LastPayment);

                string hapiKey = _configuration["Hubspot:ApiKey"];
                var pipelineId = _configuration["Hubspot:PipelineId"];
                var stageIdNew = _configuration["Hubspot:StageIdNew"];
                var stageIdQuote = _configuration["Hubspot:StageIdQuote"];

                var hapiClient = new HubSpotApi(hapiKey);

                // Get contact by email
                ContactHubSpotModel contact = null;
                contact = hapiClient.Contact.GetByEmail(input.Customer.Email);

                // If contact does not exists, it has to be created
                if (contact == null)
                {
                    contact = hapiClient.Contact.Create(new ContactHubSpotModel()
                    {
                        Email = input.Customer.Email,
                        FirstName = input.Customer.FirstName,
                        LastName = input.Customer.LastName,
                        Phone = input.Customer.Phone,
                        Address = input.Customer.Address
                    });
                }

                // ToDo Create constants for HEI properties
                var dealsAssociated = hapiClient.Deal.ListAssociated(true, contact.Id.Value, new ListRequestOptions
                {
                    Limit = 100,
                    PropertiesToInclude = new List<string> {
                                "dealname",
                                "producto_de_interes_",
                                "unidad_de_interes",
                                "fecha_cotizacion",
                                "es_condo_hotel_",
                                "precio_equipamiento_condo_hotel",
                                "precio_preoperativo_condo_hotel",
                                "precio_equipamientopreoper_condo_hotel",
                                "precio_de_lista",
                                "politica_comercial_2_0",
                                "descuento",
                                "precio_final",
                                "monto_de_enganche",
                                "numero_de_mensualidad",
                                "monto_diferido",
                                "monto_contra_entrega",
                                "asesor_que_cotizo",
                                "pipeline",
                                "dealstage"
                            }
                });

                var deal = dealsAssociated.Deals.Where(x =>
                    (x.Properties.ContainsKey("producto_de_interes_") && x.Properties["producto_de_interes_"].Value.ToString() == project.DisplayName)
                    && (x.Properties.ContainsKey("pipeline") && x.Properties["pipeline"].Value.ToString() == pipelineId)
                    // && (x.Properties.ContainsKey("dealstage") && x.Properties["dealstage"].Value.ToString() == stageIdQuote)
                    ).SingleOrDefault();

                if (deal == null)
                {
                    deal = hapiClient.Deal.Create(new DealHubSpotModel
                    {
                        Amount = Decimal.ToDouble(Math.Round(investmentTotal, 2)),
                        Name = $"{contact.Properties["firstname"].Value} {contact.Properties["lastname"].Value} - {project.DisplayName} - {unit.Model.Name} {unit.Number}",
                        ProductOfInterest = project.DisplayName,
                        UnitOfInterest = unit.Number,
                        // QuoteDate = input.Date,
                        IsCondoHotel = input.CondoHotel ? "Sí" : "No",
                        EquipmentCost = unitEquipmentResult != null ? Decimal.ToDouble(Math.Round(unitEquipmentResult.Cost, 2)) : 0,
                        PreOperatingCost = Decimal.ToDouble(Math.Round(preOperatingCost, 2)),
                        EquipmentPreOper = Decimal.ToDouble(Math.Round(unitEquipmentResult.Cost, 2)) + Decimal.ToDouble(Math.Round(preOperatingCost, 2)),
                        ListPrice = Decimal.ToDouble(Math.Round(unitPrice.Price, 2)),
                        TradePolicy = isCustomTradePolicy ? "Política Especial" : tradePolicy.Name,
                        Discount = Decimal.ToDouble(input.Discount),
                        FinalPrice = Decimal.ToDouble(Math.Round(finalPrice, 2)),
                        Deposit = Decimal.ToDouble(input.Deposit),
                        MonthlyPayments = input.AdditionalPayments.Count(),
                        AdditionalPayments = Decimal.ToDouble(input.AdditionalPayments.Sum(x => x.Amount)),
                        LastPayment = Decimal.ToDouble(input.LastPayment),
                        Pipeline = pipelineId,
                        Stage = stageIdNew
                    });

                    hapiClient.Deal.Associate(deal.Id.Value, "CONTACT", contact.Id.Value);
                }
                else
                {
                    deal.Amount = Decimal.ToDouble(Math.Round(investmentTotal, 2));
                    deal.Name = $"{contact.Properties["firstname"].Value} {contact.Properties["lastname"].Value} - {project.DisplayName} - {unit.Model.Name} {unit.Number}";
                    deal.ProductOfInterest = project.DisplayName;
                    deal.UnitOfInterest = unit.Number;
                    // deal.QuoteDate = input.Date;
                    deal.IsCondoHotel = input.CondoHotel ? "Sí" : "No";
                    deal.EquipmentCost = unitEquipmentResult != null ? Decimal.ToDouble(Math.Round(unitEquipmentResult.Cost, 2)) : 0;
                    deal.PreOperatingCost = Decimal.ToDouble(Math.Round(preOperatingCost, 2));
                    deal.EquipmentPreOper = Decimal.ToDouble(Math.Round(unitEquipmentResult.Cost, 2)) + Decimal.ToDouble(Math.Round(preOperatingCost, 2));
                    deal.ListPrice = Decimal.ToDouble(Math.Round(unitPrice.Price, 2));
                    deal.TradePolicy = isCustomTradePolicy ? "Política Especial" : tradePolicy.Name;
                    deal.Discount = Decimal.ToDouble(input.Discount);
                    deal.FinalPrice = Decimal.ToDouble(Math.Round(finalPrice, 2));
                    deal.Deposit = Decimal.ToDouble(input.Deposit);
                    deal.MonthlyPayments = input.AdditionalPayments.Count();
                    deal.AdditionalPayments = Decimal.ToDouble(input.AdditionalPayments.Sum(x => x.Amount));
                    deal.LastPayment = Decimal.ToDouble(input.LastPayment);

                    hapiClient.Deal.Update(deal);
                }
            }
            catch (Exception ex)
            {
                // ToDo Just log error
                _logger.LogError($"Hubspot. {ex.Message}");
            }
        }

        private ObjectSettings GetPdfCover(Guid projectId)
        {
            var htmlBodyCover = string.Empty;
            using (StreamReader reader = new StreamReader($"./Templates/Cover.html"))
            {
                htmlBodyCover = reader.ReadToEnd();
            }
            var htmlContentCover = string.Format(htmlBodyCover, string.Format("https://heicommunityquotes.blob.core.windows.net/realtor/cover/{0}.jpg", projectId.ToString()));

            return new ObjectSettings()
            {
                PagesCount = false,
                HtmlContent = htmlContentCover,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") }
            };
        }

        private ObjectSettings GetPdfIntro(Guid projectId)
        {
            var htmlBodyCover = string.Empty;
            using (StreamReader reader = new StreamReader($"./Templates/Intro.html"))
            {
                htmlBodyCover = reader.ReadToEnd();
            }
            var htmlContentCover = string.Format(htmlBodyCover, string.Format("https://heicommunityquotes.blob.core.windows.net/realtor/intro/{0}.jpg", projectId.ToString()));

            return new ObjectSettings()
            {
                PagesCount = false,
                HtmlContent = htmlContentCover,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") }
            };
        }

        private ObjectSettings GetPdfAmenities(Guid projectId)
        {
            var htmlBodyCover = string.Empty;
            using (StreamReader reader = new StreamReader($"./Templates/Amenities.html"))
            {
                htmlBodyCover = reader.ReadToEnd();
            }
            var htmlContentCover = string.Format(htmlBodyCover, string.Format("https://heicommunityquotes.blob.core.windows.net/realtor/amenities/{0}.jpg", projectId.ToString()));

            return new ObjectSettings()
            {
                PagesCount = false,
                HtmlContent = htmlContentCover,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") }
            };
        }

        private ObjectSettings GetPdfUnit(QuoteInputViewModel input)
        {
            var project = _projectService.GetProjectByIdAsync(input.ProjectId).Result;
            var unit = _unitService.GetUnitByIdAsync(input.UnitId).Result;

            var htmlBodyCover = string.Empty;
            using (StreamReader reader = new StreamReader($"./Templates/Unit.html"))
            {
                htmlBodyCover = reader.ReadToEnd();
            }
            var htmlContentCover = string.Format(htmlBodyCover,
                                                $"{input.Customer?.FirstName} {input.Customer?.LastName}",
                                                project.DisplayName,
                                                string.Format("{0} {1}", unit.Number, unit.Model?.Name),
                                                string.Format("{0:0.##}", unit.GrossArea),
                                                unit.Level > 0 ? unit.Level.ToString() : "PB",
                                                unit.TerraceArea > 0 ? string.Format("{0:0.##}", unit.TerraceArea) : "N/A",
                                                string.Format("https://heicommunityquotes.blob.core.windows.net/realtor/units/renders/{0}/{1}.jpg", project.Id.ToString(), unit.ModelId.ToString()));

            return new ObjectSettings()
            {
                PagesCount = false,
                HtmlContent = htmlContentCover,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") }
            };
        }

        private ObjectSettings GetPdfSummary(QuoteInputViewModel input, QuoteResultViewModel summary)
        {
            var culture = CultureInfo.GetCultureInfo("es-MX");

            var htmlBodySummary = string.Empty;
            using (StreamReader reader = new StreamReader($"./Templates/Summary.html"))
            {
                htmlBodySummary = reader.ReadToEnd();
            }

            var sbDetailsHead = string.Empty;
            var sbDetails = new StringBuilder();
            
                sbDetailsHead = @"<tr>
                                    <th style='width: 33%'><span class='indent-left'/>Número</th>
                                    <th style='width: 33%'>Fecha</th>
                                    <th style='width: 33%'>Abono unidad<span class='indent-right'/></th>
                                </tr>";
                foreach (var details in summary.Details)
                {
                    sbDetails.AppendFormat(@"<tr>
                                    <td style='width: 33%'><span class='indent-left'/>{0}</td>
                                    <td style='width: 33%'>{1}</td>
                                    <td style='width: 33%'>{2}<span class='indent-right'/></td>
                                  </tr>",
                                    details.Number,
                                    details.Date.ToShortDateString(),
                                    (details.Amount == 0) ? "$ -" : details.Amount.ToString("C", culture));
                }
          

            var exchangeRate = ExchangeRate.GetRate("usd");

            var tradePolicy = _tradePolicyService.GetTradePolicyByIdAsync(input.TradePolicyId).Result;
            var unit = _unitService.GetUnitByIdAsync(input.UnitId).Result;

            var unitPrice = unit.UnitPrices.Where(p => p.StageId.ToString() == unit.Model.Project.CurrentStageId.ToString()).FirstOrDefault();
            var priceList = unitPrice.Currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase)
                            ? Math.Round(unitPrice.Price * exchangeRate, 2)
                            : Math.Round(unitPrice.Price, 2);
            var minimumExpectedValue = unitPrice.Currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase)
                                        ? unit.UnitPrices.Max(x => x.Price * exchangeRate)
                                        : unit.UnitPrices.Max(x => x.Price);
            var priceDiscount = priceList * input.Discount / 100;
            var finalPrice = priceList - priceDiscount;
            var capitalGain = minimumExpectedValue - finalPrice;

            var project = _projectService.GetProjectByIdAsync(input.ProjectId).Result;
            var capRate = project.CapRate.HasValue ? project.CapRate.Value : Formulas.CapRate();

            var htmlContentSummary = string.Format(htmlBodySummary,
                                                    finalPrice.ToString("C", culture),
                                                    priceList.ToString("C", culture),
                                                    (input.Discount / 100).ToString("P", culture),
                                                    priceDiscount.ToString("C", culture),
                                                    tradePolicy.Name,
                                                    (input.Deposit / summary.Investment).ToString("P", culture),
                                                    input.Deposit.ToString("C", culture),
                                                    (input.AdditionalPayments.Sum(x => x.Amount) / summary.Investment).ToString("P", culture),
                                                    input.AdditionalPayments.Sum(x => x.Amount).ToString("C", culture),
                                                    (input.LastPayment / summary.Investment).ToString("P", culture),
                                                    input.LastPayment.ToString("C", culture),
                                                    (capitalGain / finalPrice).ToString("P", culture),
                                                    minimumExpectedValue.ToString("C", culture),
                                                    capitalGain.ToString("C", culture),
                                                    (minimumExpectedValue * capRate / 12).ToString("C", culture),
                                                    (minimumExpectedValue * capRate).ToString("C", culture),
                                                    (minimumExpectedValue * capRate / minimumExpectedValue).ToString("P", culture),
                                                    (minimumExpectedValue * capRate / finalPrice).ToString("P", culture),
                                                    sbDetailsHead,
                                                    sbDetails.ToString());

            return new ObjectSettings()
            {
                HtmlContent = htmlContentSummary,
                WebSettings = {
                    DefaultEncoding = "utf-8",
                    UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css")
                }
            };
        }

        private ObjectSettings GetPdfSummaryT(QuoteInputViewModel input, QuoteResultViewModel summary)
        {
            var culture = CultureInfo.GetCultureInfo("es-MX");

            var htmlBodySummary = string.Empty;
            using (StreamReader reader = new StreamReader($"./Templates/SummaryT.html"))
            {
                htmlBodySummary = reader.ReadToEnd();
            }

            var sbDetailsHead = string.Empty;
            var sbDetails = new StringBuilder();
            
                sbDetailsHead = @"<tr>
                                    <th style='width: 25%'><span class='indent-left'/>Número</th>
                                    <th style='width: 25%'>Fecha</th>
                                    <th style='width: 25%'>Abono unidad</th>
                                    <th style='width: 25%'>Equipamiento + Preoperativo<span class='indent-right'/></th>
                                </tr>";



            if(summary.Details.Count < 7)
            {
                int cont = summary.Details.Count;
                foreach (var details in summary.Details)
                {
                    sbDetails.AppendFormat(@"<tr>
                                    <td style='width: 25%'><span class='indent-left'/>{0}</td>
                                    <td style='width: 25%'>{1}</td>
                                    <td style='width: 25%'>{2}</td>
                                    <td style='width: 25%'>{3}<span class='indent-right'/></td>
                                  </tr>",
                                    details.Number,
                                    details.Date.ToShortDateString(),
                                    (details.Amount == 0) ? "$ -" : details.Amount.ToString("C", culture),
                                    (cont == summary.Details.Count) ? summary.CondoHotel.EquipmentPreOper.ToString("C", culture) : "$ -");

                    cont--;
                }

            }
            else 
                { 

                    var index = 0;
                    foreach (var details in summary.Details)
                    {
                        sbDetails.AppendFormat(@"<tr>
                                        <td style='width: 25%'><span class='indent-left'/>{0}</td>
                                        <td style='width: 25%'>{1}</td>
                                        <td style='width: 25%'>{2}</td>
                                        <td style='width: 25%'>{3}<span class='indent-right'/></td>
                                      </tr>",
                                        details.Number,
                                        details.Date.ToShortDateString(),
                                        (details.Amount == 0) ? "$ -" : details.Amount.ToString("C", culture),

                                        (summary.Details.Count < 7)
                                        ? (summary.CondoHotel.EquipmentPreOper).ToString("C", culture) : (index == 0)

                                            ? (summary.CondoHotel.EquipmentPreOper * .1m).ToString("C", culture) : (index == summary.Details.Count - 6)
                                                
                                                ? (summary.CondoHotel.EquipmentPreOper * .9m).ToString("C", culture) : "$ -");
                        index++;
                    }

                }


            var exchangeRate = ExchangeRate.GetRate("usd");

            var tradePolicy = _tradePolicyService.GetTradePolicyByIdAsync(input.TradePolicyId).Result;
            var unit = _unitService.GetUnitByIdAsync(input.UnitId).Result;

            var unitPrice = unit.UnitPrices.Where(p => p.StageId.ToString() == unit.Model.Project.CurrentStageId.ToString()).FirstOrDefault();
            var priceList = unitPrice.Currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase)
                            ? Math.Round(unitPrice.Price * exchangeRate, 2)
                            : Math.Round(unitPrice.Price, 2);
            var minimumExpectedValue = unitPrice.Currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase)
                                        ? unit.UnitPrices.Max(x => x.Price * exchangeRate)
                                        : unit.UnitPrices.Max(x => x.Price);
            var priceDiscount = priceList * input.Discount / 100;
            var finalPrice = priceList - priceDiscount;
            var capitalGain = minimumExpectedValue - finalPrice;

            var project = _projectService.GetProjectByIdAsync(input.ProjectId).Result;
            var capRate = project.CapRate.HasValue ? project.CapRate.Value : Formulas.CapRate();

            var htmlContentSummary = string.Format(htmlBodySummary,
                                                    finalPrice.ToString("C", culture),
                                                    priceList.ToString("C", culture),
                                                    (input.Discount / 100).ToString("P", culture),
                                                    priceDiscount.ToString("C", culture),
                                                    tradePolicy.Name,
                                                    (input.Deposit / summary.Investment).ToString("P", culture),
                                                    input.Deposit.ToString("C", culture),
                                                    (input.AdditionalPayments.Sum(x => x.Amount) / summary.Investment).ToString("P", culture),
                                                    input.AdditionalPayments.Sum(x => x.Amount).ToString("C", culture),
                                                    (input.LastPayment / summary.Investment).ToString("P", culture),
                                                    input.LastPayment.ToString("C", culture),
                                                    (capitalGain / finalPrice).ToString("P", culture),
                                                    minimumExpectedValue.ToString("C", culture),
                                                    capitalGain.ToString("C", culture),
                                                    (minimumExpectedValue * capRate / 12).ToString("C", culture),
                                                    (minimumExpectedValue * capRate).ToString("C", culture),
                                                    (minimumExpectedValue * capRate / minimumExpectedValue).ToString("P", culture),
                                                    (minimumExpectedValue * capRate / finalPrice).ToString("P", culture),
                                                    sbDetailsHead,
                                                    sbDetails.ToString());

            return new ObjectSettings()
            {
                HtmlContent = htmlContentSummary,
                WebSettings = {
                    DefaultEncoding = "utf-8",
                    UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css")
                }
            };
        }


        private ObjectSettings GetPdfSummaryTravel(QuoteInputViewModel input, QuoteResultViewModel summary)
        {
            var project = _projectService.GetProjectByIdAsync(input.ProjectId).Result;
            var unit = _unitService.GetUnitByIdAsync(input.UnitId).Result;

            var exchangeRate = ExchangeRate.GetRate("usd");

            var unitPrice = unit.UnitPrices.Where(p => p.StageId.ToString() == unit.Model.Project.CurrentStageId.ToString()).FirstOrDefault();

            var unitEquipmentResult = _unitEquipmentService.GetAllUnitEquipmentsByProjectIdAsync(input.ProjectId).Result.FirstOrDefault(x => x.UnitId.Equals(input.UnitId));
            var unitRateResult = _unitRatesService.GetAllUnitRatesByUnitIdAsync(input.UnitId).Result.FirstOrDefault();
            var temporalityResult = _temporalityService.GetAllTemporalitiesByProjectIdAsync(input.ProjectId).Result;
            var condoHotelExpensesResult = _condoHotelExpensesService.GetAllCondoHotelExpensesByProjectIdAsync(input.ProjectId).Result;
            var condoHotelExpensesSelected = input.HasOptionalExpenses
                                            ? condoHotelExpensesResult.Where(x => !x.IsOptional
                                                            || (!input.OptionalExpenses.Select(x => x.ExpenseName).ToList().Contains(x.Expense.Name)))
                                            : condoHotelExpensesResult.Where(x => !x.IsOptional);

            var unitRate = unitRateResult.CostPerNight.HasValue
                            ? unitRateResult.CostPerNight.Value
                            : (unitRateResult.BuiltUpAreaCost.HasValue && unitRateResult.TerraceAreaCost.HasValue)
                                ? (unit.BuiltUpArea * unitRateResult.BuiltUpAreaCost.Value) + (unit.TerraceArea * unitRateResult.TerraceAreaCost.Value)
                                : 0m;
            var ratePerNight = temporalityResult.Average(x => unitRate * x.Percentage / 100);
            var annualOccupancyMax = temporalityResult.Sum(x => x.OccupationInDaysMax) / 365;
            var annualOccupancy = (input.CondoHotelOccupancy.HasValue && input.CondoHotelOccupancy.Value > 0)
                                ? input.CondoHotelOccupancy.Value / 100
                                : temporalityResult.Sum(x => x.OccupationInDays) / 365;
            var annualIncome = (input.CondoHotelOccupancy.HasValue && input.CondoHotelOccupancy.Value > 0)
                                ? (input.CondoHotelOccupancy.Value / 100 * 365) * ratePerNight
                                : temporalityResult.Sum(x => (unitRate * x.Percentage / 100) * x.OccupationInDays);

            var costOperationIncome = annualIncome * condoHotelExpensesSelected.Where(x => x.Expense.Type.Equals(0) && x.Percentage > 0).Sum(x => x.Percentage / 100);
            costOperationIncome += condoHotelExpensesSelected.Where(x => x.Expense.Type.Equals(0) && x.Cost > 0).Sum(x => x.Cost);
            var managementFeeIncome = annualIncome * condoHotelExpensesSelected.Where(x => x.Expense.Type.Equals(2) && x.Percentage > 0).Sum(x => x.Percentage / 100);
            managementFeeIncome += condoHotelExpensesSelected.Where(x => x.Expense.Type.Equals(2) && x.Cost > 0).Sum(x => x.Cost);

            var netIncome = annualIncome - (costOperationIncome + managementFeeIncome);

            var expensesDetails = condoHotelExpensesResult.Where(x => x.Expense.Type == 0 || x.Expense.Type == 1).Select(x => new CondoHotelExpenseDetailViewModel
            {
                ExpenseName = x.Expense.Name,
                Percentage = x.Percentage > 0
                                 ? Math.Round((x.Percentage / 100), 4)
                                 : (x.Expense.Type == 0
                                    ? Math.Round((x.Cost / annualIncome), 4)
                                    : Math.Round((x.Cost / netIncome), 4)),
                Cost = x.Cost > 0
                            ? Math.Round(x.Cost, 2)
                            : (x.Expense.Type == 0
                                ? Math.Round((annualIncome * (x.Percentage / 100)), 2)
                                : Math.Round((netIncome * (x.Percentage / 100)), 2)),
                Type = x.Expense.Type,
                DependsOnOccupancy = x.Percentage > 0,
                IsOptional = x.IsOptional
            }).ToList();

            var feeDetails = condoHotelExpensesResult.Where(x => x.Expense.Type == 2 || x.Expense.Type == 3).Select(x => new CondoHotelExpenseDetailViewModel
            {
                ExpenseName = x.Expense.Name,
                Percentage = x.Percentage > 0
                             ? Math.Round((x.Percentage / 100), 4)
                             : (x.Expense.Type == 2
                                ? Math.Round((x.Cost / annualIncome), 4)
                                : Math.Round((x.Cost / netIncome), 4)),
                Cost = x.Cost > 0
                        ? Math.Round(x.Cost, 2)
                        : (x.Expense.Type == 2
                            ? Math.Round((annualIncome * (x.Percentage / 100)), 2)
                            : Math.Round((netIncome * (x.Percentage / 100)), 2)),
                Type = x.Expense.Type,
                DependsOnOccupancy = x.Percentage > 0,
                IsOptional = x.IsOptional
            }).ToList();

            var costOperationProfit = 0m;
            var managementFeeProfit = 0m;
            if (netIncome > 0)
            {
                costOperationProfit = netIncome * condoHotelExpensesSelected.Where(x => x.Expense.Type.Equals(1)).Sum(x => x.Percentage / 100);
                managementFeeProfit = netIncome * condoHotelExpensesSelected.Where(x => x.Expense.Type.Equals(3)).Sum(x => x.Percentage / 100);
                netIncome -= (costOperationProfit + managementFeeProfit);
            }

            var extraWeeks = new List<CondoHotelExtraWeeksResultViewModel>
                {
                    new CondoHotelExtraWeeksResultViewModel {
                        RateId = Guid.Parse("66A090A3-4602-4D2A-9693-73812223B3AA"),
                        RateName = "Temporada alta",
                        Included = 0,
                        Extra = input.ExtraWeeks != null && input.ExtraWeeks.Any(x => x.RateId.Equals(Guid.Parse("66A090A3-4602-4D2A-9693-73812223B3AA")))
                                ? input.ExtraWeeks.Where(x => x.RateId.Equals(Guid.Parse("66A090A3-4602-4D2A-9693-73812223B3AA"))).FirstOrDefault().Extra
                                : 0,
                        Multiplier = 1.15m,
                        Cost =  input.ExtraWeeks != null && input.ExtraWeeks.Any(x => x.RateId.Equals(Guid.Parse("66A090A3-4602-4D2A-9693-73812223B3AA")))
                                ? Math.Round(
                                    (annualIncome/annualOccupancy/365) *
                                    (input.ExtraWeeks.Where(x => x.RateId.Equals(Guid.Parse("66A090A3-4602-4D2A-9693-73812223B3AA"))).FirstOrDefault().Extra * 7 * 1.15m)
                                    , 4)
                                : 0,
                        Order = 0
                    },
                    new CondoHotelExtraWeeksResultViewModel {
                        RateId = Guid.Parse("D95AE993-3328-49EF-8403-2E2D62FE7EF1"),
                        RateName = "Temporada media",
                        Included = 2,
                        Extra = input.ExtraWeeks != null && input.ExtraWeeks.Any(x => x.RateId.Equals(Guid.Parse("D95AE993-3328-49EF-8403-2E2D62FE7EF1")))
                                ? input.ExtraWeeks.Where(x => x.RateId.Equals(Guid.Parse("D95AE993-3328-49EF-8403-2E2D62FE7EF1"))).FirstOrDefault().Extra
                                : 0,
                        Multiplier = .9m,
                        Cost =  input.ExtraWeeks != null && input.ExtraWeeks.Any(x => x.RateId.Equals(Guid.Parse("D95AE993-3328-49EF-8403-2E2D62FE7EF1")))
                                ? Math.Round(
                                    (annualIncome/annualOccupancy/365) *
                                    (input.ExtraWeeks.Where(x => x.RateId.Equals(Guid.Parse("D95AE993-3328-49EF-8403-2E2D62FE7EF1"))).FirstOrDefault().Extra * 7 * .9m)
                                    , 4)
                                : 0,
                        Order = 1
                    },
                    new CondoHotelExtraWeeksResultViewModel {
                        RateId = Guid.Parse("3DA2FA78-5D1C-4964-9468-DA3051DEED6C"),
                        RateName = "Temporada baja",
                        Included = 2,
                        Extra = input.ExtraWeeks != null && input.ExtraWeeks.Any(x => x.RateId.Equals(Guid.Parse("3DA2FA78-5D1C-4964-9468-DA3051DEED6C")))
                                ? input.ExtraWeeks.Where(x => x.RateId.Equals(Guid.Parse("3DA2FA78-5D1C-4964-9468-DA3051DEED6C"))).FirstOrDefault().Extra
                                : 0,
                        Multiplier = .66m,
                        Cost =  input.ExtraWeeks != null && input.ExtraWeeks.Any(x => x.RateId.Equals(Guid.Parse("3DA2FA78-5D1C-4964-9468-DA3051DEED6C")))
                                ? Math.Round(
                                    (annualIncome/annualOccupancy/365) *
                                    (input.ExtraWeeks.Where(x => x.RateId.Equals(Guid.Parse("3DA2FA78-5D1C-4964-9468-DA3051DEED6C"))).FirstOrDefault().Extra * 7 * .66m)
                                    , 4)
                                : 0,
                        Order = 2
                    }
                };
            var extraWeeksCost = extraWeeks.Sum(x => x.Cost);
            netIncome -= extraWeeksCost;

            var preOperatingCost = project.CondoHotelPreOperatingCost.HasValue ? project.CondoHotelPreOperatingCost.Value : 0m;
            var investmentTotal = input.Price + unitEquipmentResult.Cost + preOperatingCost;

            var result = new CondoHotelResultViewModel
            {
                EquipmentPrice = Math.Round(unitEquipmentResult.Cost, 2),
                PreOperatingCost = Math.Round(preOperatingCost, 2),
                EquipmentPreOper = Math.Round((unitEquipmentResult.Cost + preOperatingCost), 2),
                InvestmentTotal = Math.Round(investmentTotal, 2),
                AnnualIncome = Math.Round(annualIncome, 2),
                CostOperation = Math.Round((costOperationIncome + costOperationProfit), 2),
                ManagementFee = Math.Round((managementFeeIncome + managementFeeProfit), 2),
                NetAnnualIncome = Math.Round(netIncome, 2),
                Occupancy = Formulas.RoundUp((int)(annualOccupancy * 100), 5) / 100m,
                OccupancyMax = Formulas.RoundUp((int)(annualOccupancyMax * 100), 5) / 100m,
                RatePerNight = Math.Round(ratePerNight, 2),
                ReturnOfInvestment = Math.Round(netIncome / investmentTotal, 4),
                ReturnOfInvestmentYears = Math.Round(1 / (netIncome / investmentTotal), 1),
                ExpensesDetails = expensesDetails,
                FeeDetails = feeDetails,
                ExtraWeeks = extraWeeks
            };

            var culture = CultureInfo.GetCultureInfo("es-MX");

            var htmlBodySummary = string.Empty;
            using (StreamReader reader = new StreamReader($"./Templates/SummaryTravel.html"))
            {
                htmlBodySummary = reader.ReadToEnd();
            }

            var extraWeeksDetails = new StringBuilder();
            foreach (var details in extraWeeks.OrderBy(x => x.Order))
            {
                extraWeeksDetails.AppendFormat(@"<tr>
                                                    <td><span class='indent-left' />{0}</td>
                                                    <td>{1}</td>
                                                    <td>{2}</td>
                                                    <td>{3}</td>
                                                    <td>{4}</td>
                                                </tr>",
                                                details.RateName,
                                                details.Included,
                                                details.Extra,
                                                (details.Cost / exchangeRate).ToString("C", culture),
                                                details.Cost.ToString("C", culture));
            }

            var htmlContentSummary = string.Format(htmlBodySummary,
                                                    (result.NetAnnualIncome / exchangeRate).ToString("C", culture),
                                                    result.NetAnnualIncome.ToString("C", culture),
                                                    (result.InvestmentTotal / exchangeRate).ToString("C", culture),
                                                    result.InvestmentTotal.ToString("C", culture),
                                                    (result.RatePerNight / exchangeRate).ToString("C", culture),
                                                    result.RatePerNight.ToString("C", culture),
                                                    (result.AnnualIncome / exchangeRate).ToString("C", culture),
                                                    result.AnnualIncome.ToString("C", culture),
                                                    (result.CostOperation / exchangeRate).ToString("C", culture),
                                                    result.CostOperation.ToString("C", culture),
                                                    (result.ManagementFee / exchangeRate).ToString("C", culture),
                                                    result.ManagementFee.ToString("C", culture),
                                                    extraWeeksDetails);

            return new ObjectSettings()
            {
                HtmlContent = htmlContentSummary,
                WebSettings = {
                    DefaultEncoding = "utf-8",
                    UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css")
                }
            };
        }

        private ObjectSettings GetPdfFinal(Guid projectId)
        {
            var htmlBodyCover = string.Empty;
            using (StreamReader reader = new StreamReader($"./Templates/Final.html"))
            {
                htmlBodyCover = reader.ReadToEnd();
            }
            var htmlContentCover = string.Format(htmlBodyCover, string.Format("https://heicommunityquotes.blob.core.windows.net/realtor/final/{0}.jpg", projectId.ToString()));

            return new ObjectSettings()
            {
                PagesCount = false,
                HtmlContent = htmlContentCover,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") }
            };
        }

        private List<(int Number, DateTime Date, decimal Amount)> GetAdditionalPaymentsDetails(decimal firstPayment, decimal lastPayment, AdditionalPayment[] payments, DateTime initialDate, DateTime deliveryDate)
        {
            var additionalPayments = new Stack<(DateTime Date, decimal Amount)>();
            if (payments != null && payments.Any())
            {
                foreach (var item in payments.OrderByDescending(x => x.Date))
                {
                    if (additionalPayments.Count > 0 && additionalPayments.Peek().Date.Year.Equals(item.Date.Year) && additionalPayments.Peek().Date.Month.Equals(item.Date.Month))
                    {
                        var mergedAdditionsAmount = additionalPayments.Pop().Amount + item.Amount;
                        additionalPayments.Push((Date: item.Date, Amount: mergedAdditionsAmount));
                    }
                    else
                    {
                        additionalPayments.Push((Date: item.Date, Amount: item.Amount));
                    }
                }
            }

            var monthsTillDeliver = Formulas.MonthsDiff(initialDate, deliveryDate);
            var details = new List<(int number, DateTime Date, decimal Amount)>();
            for (var i = 0; i <= monthsTillDeliver; i++)
            {
                var payment = (i == 0) ? firstPayment : 0m;
                payment += (i == monthsTillDeliver) ? lastPayment : 0m;

                var currentMonth = initialDate.AddMonths(i);
                var nextMonth = initialDate.AddMonths(i + 1);
                if (additionalPayments.Count > 0 && (additionalPayments.Peek().Date >= currentMonth && additionalPayments.Peek().Date < nextMonth))
                {
                    var item = additionalPayments.Pop();
                    payment += item.Amount;
                }

                details.Add((i + 1, initialDate.AddMonths(i), payment));
            }

            return details;
        }
    }
}