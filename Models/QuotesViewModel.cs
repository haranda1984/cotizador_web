using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HeiLiving.Quotes.Api.Models
{
    public class QuoteInputViewModel
    {
        [JsonPropertyName("project-id")]
        [Required]
        public Guid ProjectId { get; set; }

        [JsonPropertyName("unit-id")]
        [Required]
        public Guid UnitId { get; set; }

        [JsonPropertyName("date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [JsonPropertyName("discount")]
        [Required]
        public decimal Discount { get; set; }

        [JsonPropertyName("trade-policy-id")]
        [Required]
        public Guid TradePolicyId { get; set; }

        [JsonPropertyName("price")]
        [Required]
        public decimal Price { get; set; }

        [JsonPropertyName("deposit")]
        [Required]
        public decimal Deposit { get; set; }

        [JsonPropertyName("additions-payments")]
        public AdditionalPayment[] AdditionalPayments { get; set; }

        [JsonPropertyName("last-payment")]
        public decimal LastPayment { get; set; }

        [JsonPropertyName("customer")]
        public CustomerViewModel Customer { get; set; }

        [JsonPropertyName("condohotel")]
        public bool CondoHotel { get; set; }
        [JsonPropertyName("condohotel-occupancy")]
        public decimal? CondoHotelOccupancy { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("extra-weeks")]
        public List<CondoHotelExtraWeeksInputViewModel> ExtraWeeks { get; set; }
        
        [JsonPropertyName("optional-expenses")]
        public List<CondoHotelExpenseDetailViewModel> OptionalExpenses { get; set; }

        [JsonPropertyName("has-optional-expenses")]
        public bool HasOptionalExpenses { get; set; }
    }

    public class CondoHotelExtraWeeksInputViewModel
    {
        [JsonPropertyName("rate-id")]
        public Guid RateId { get; set; }
        public int Extra { get; set; }
    }

    public class QuoteResultViewModel
    {
        [JsonPropertyName("investment")]
        public decimal Investment { get; set; }
        [JsonPropertyName("price-list")]
        public decimal PriceList { get; set; }
        [JsonPropertyName("minimum-expected-value")]
        public decimal MinimumExpectedValue { get; set; }
        [JsonPropertyName("roi")]
        public decimal ReturnOfInvestment { get; set; }
        [JsonPropertyName("capital-gain")]
        public decimal CapitalGain { get; set; }
        [JsonPropertyName("rent-year")]
        public decimal RentPerYear { get; set; }
        [JsonPropertyName("rent-month")]
        public decimal RentPerMonth { get; set; }
        [JsonPropertyName("cap-rate-investment")]
        public decimal CapRateInvestment { get; set; }
        [JsonPropertyName("cap-rate-Final")]
        public decimal CapRatePriceFinal { get; set; }
        [JsonPropertyName("product")]
        public string ProductName { get; set; }
        [JsonPropertyName("condohotel")]
        public CondoHotelResultViewModel CondoHotel { get; set; }
        [JsonIgnore]
        public List<(int Number, DateTime Date, decimal Amount)> Details { get; set; }
        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("exchange-rate")]
        public decimal ExchangeRate { get; set; }
    }

    public class AdditionalPayment
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class CondoHotelResultViewModel
    {
        [JsonPropertyName("equipment-price")]
        public decimal EquipmentPrice { get; set; }
        [JsonPropertyName("pre-operating-cost")]
        public decimal PreOperatingCost { get; set; }
        [JsonPropertyName("equipment-pre-oper-cost")]
        public decimal EquipmentPreOper { get; set; }
        [JsonPropertyName("investment-total")]
        public decimal InvestmentTotal { get; set; }
        [JsonPropertyName("occupancy")]
        public decimal Occupancy { get; set; }
        [JsonPropertyName("occupancy-max")]
        public decimal OccupancyMax { get; set; }
        [JsonPropertyName("rate-per-night")]
        public decimal RatePerNight { get; set; }
        [JsonPropertyName("annual-income")]
        public decimal AnnualIncome { get; set; }
        [JsonPropertyName("net-annual-income")]
        public decimal NetAnnualIncome { get; set; }
        [JsonPropertyName("cost-operation")]
        public decimal CostOperation { get; set; }
        [JsonPropertyName("management-fee")]
        public decimal ManagementFee { get; set; }
        [JsonPropertyName("roi-percentage")]
        public decimal ReturnOfInvestment { get; set; }
        [JsonPropertyName("roi-years")]
        public decimal ReturnOfInvestmentYears { get; set; }

        // [JsonPropertyName("invesment-details")]
        [JsonIgnore]
        public List<CondoHotelInvesmentDetailViewModel> InvesmentDetails { get; set; }
        // [JsonPropertyName("rate-per-night-details")]
        [JsonIgnore]
        public List<CondoHotelRatePerNightDetailViewModel> RatePerNightDetails { get; set; }
        // [JsonPropertyName("income-details")]
        [JsonIgnore]
        public List<CondoHotelIncomeDetailViewModel> IncomeDetails { get; set; }
        [JsonPropertyName("expenses-details")]
        public List<CondoHotelExpenseDetailViewModel> ExpensesDetails { get; set; }
        [JsonPropertyName("fee-details")]
        public List<CondoHotelExpenseDetailViewModel> FeeDetails { get; set; }
        [JsonPropertyName("extra-weeks")]
        public List<CondoHotelExtraWeeksResultViewModel> ExtraWeeks { get; set; }
    }

    public class CondoHotelInvesmentDetailViewModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }

    public class CondoHotelExpenseDetailViewModel
    {
        [JsonPropertyName("name")]
        public string ExpenseName { get; set; }
        [JsonPropertyName("percentage")]
        public decimal Percentage { get; set; }
        [JsonPropertyName("cost")]
        public decimal Cost { get; set; }
        [JsonPropertyName("type")]
        public int Type { get; set; }
        [JsonPropertyName("depends-on-occupancy")]
        public bool DependsOnOccupancy { get; set; }
        [JsonPropertyName("is-optional")]
        public bool IsOptional { get; set; }
    }

    public class CondoHotelRatePerNightDetailViewModel
    {
        [JsonPropertyName("month")]
        public int Month { get; set; }
        [JsonPropertyName("month-name")]
        public string MonthName { get; set; }
        [JsonPropertyName("days")]
        public decimal Days { get; set; }
        [JsonPropertyName("rate-type")]
        public string RateType { get; set; }
        [JsonPropertyName("rate-per-night")]
        public decimal RatePerNight { get; set; }
    }

    public class CondoHotelIncomeDetailViewModel
    {
        [JsonPropertyName("month")]
        public int Month { get; set; }
        [JsonPropertyName("month-name")]
        public string MonthName { get; set; }
        [JsonPropertyName("income")]
        public decimal Income { get; set; }
    }

    public class CondoHotelExtraWeeksResultViewModel
    {
        [JsonPropertyName("rate-id")]
        public Guid RateId { get; set; }
        [JsonPropertyName("rate-name")]
        public string RateName { get; set; }
        public int Included { get; set; }
        public int Extra { get; set; }
        public decimal Multiplier { get; set; }
        public decimal Cost { get; set; }
        public int Order { get; set; }
    }
}