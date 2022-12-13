using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using HeiLiving.Quotes.Api.Services;
using HeiLiving.Quotes.Api.Models;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Helpers;
using System.Collections.Generic;

namespace HeiLiving.Quotes.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _projectService;
        private readonly IUnitService _unitService;
        private readonly IUnitEquipmentsService _unitEquipmentService;
        private readonly ITradePoliciesService _tradePolicyService;
        private readonly IStageService _stageService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ProjectsController(IProjectsService projectService, IUnitService unitService, ITradePoliciesService tradePolicyService,
                                IUnitEquipmentsService unitEquipmentService, IStageService stageService, IStringLocalizer<SharedResource> localizer)
        {
            _projectService = projectService;
            _unitService = unitService;
            _unitEquipmentService = unitEquipmentService;
            _tradePolicyService = tradePolicyService;
            _stageService = stageService;
            _localizer = localizer;
        }

        // GET api/projects
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();

            var viewModel = new List<ProjectResponseViewModel>();
            foreach (var project in projects)
            {
                var units = await _unitService.GetAllUnitsByProjectIdAsync(project.Id);
                var unitsAvailables = units.Count(x => x.Status == UnitStatus.ForSale);
                var unitsSoldAsCondoHotel = units.Count(x => x.Status != UnitStatus.ForSale && x.IsCondoHotel);

                var tradePolicies = await _tradePolicyService.GetAllTradePoliciesByProjectIdAsync(project.Id);

                var stages = await _stageService.GetAllStagesByProjectIdAsync(project.Id);

                viewModel.Add(new ProjectResponseViewModel
                {
                    Id = project.Id,
                    Name = project.Name,
                    DisplayName = project.DisplayName,
                    Description = project.Description,
                    PhotoUrl = project.PhotoUrl,
                    ThemeColor = project.ThemeColor,
                    Location = project.Location,
                    IsActive = project.IsActive,
                    DeliveryDate = project.DeliveryDate,
                    Appreciation = project.Appreciation,
                    AllowsCondoHotel = project.AllowsCondoHotel,
                    MinimumCondoHotelUnits = project.MinimumCondoHotelUnits,
                    CurrentStage = project.CurrentStage?.Name,
                    CurrentStageId = project.CurrentStageId,
                    UnitsAvailables = unitsAvailables,
                    UnitsSoldAsCondoHotel = unitsSoldAsCondoHotel,
                    IsCondoHotelOptional = project.AllowsCondoHotel && ((unitsSoldAsCondoHotel + unitsAvailables) - project.MinimumCondoHotelUnits) > 0,
                    UnitsNumber = units.Count(),
                    TradePoliciesNumber = tradePolicies.Count(),
                    Stages = stages.OrderBy(x => x.Order).Select(x => new StageViewModel { Id = x.Id, Name = x.Name, IsActive = x.IsActive, ProjectId = project.Id }).ToList(),
                    CapRate = project.CapRate == null ? Formulas.CapRate() : project.CapRate,
                });
            }

            return Ok(viewModel);
        }

        // GET api/projects/5
        [HttpGet("{id}", Name = "ProjectById")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            var units = await _unitService.GetAllUnitsByProjectIdAsync(project.Id);

            var unitsAvailables = units.Count(x => x.Status == UnitStatus.ForSale);
            var unitsSoldAsCondoHotel = units.Count(x => x.Status != UnitStatus.ForSale && x.IsCondoHotel);

            var viewModel = new ProjectResponseViewModel
            {
                Id = project.Id,
                Name = project.Name,
                DisplayName = project.DisplayName,
                Description = project.Description,
                PhotoUrl = project.PhotoUrl,
                ThemeColor = project.ThemeColor,
                Location = project.Location,
                IsActive = project.IsActive,
                DeliveryDate = project.DeliveryDate,
                Appreciation = project.Appreciation,
                AllowsCondoHotel = project.AllowsCondoHotel,
                MinimumCondoHotelUnits = project.MinimumCondoHotelUnits,
                CurrentStage = project.CurrentStage?.Name,
                CurrentStageId = project.CurrentStageId,
                UnitsAvailables = unitsAvailables,
                UnitsSoldAsCondoHotel = unitsSoldAsCondoHotel,
                IsCondoHotelOptional = project.AllowsCondoHotel && ((unitsSoldAsCondoHotel + unitsAvailables) - project.MinimumCondoHotelUnits) > 0,
                CapRate = project.CapRate == null ? Formulas.CapRate() : project.CapRate,
            };

            return Ok(viewModel);
        }

        // GET api/projects/5/unit
        [HttpGet("{id}/units")]
        public async Task<IActionResult> GetProjectUnitById(Guid id, string currency)
        {
            var project = await _projectService.GetProjectByIdAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            var units = await _unitService.GetAllUnitsByProjectIdAsync(id);

            if (units == null)
            {
                return NotFound();
            }

            var unitsEquipmentByProject = await _unitEquipmentService.GetAllUnitEquipmentsByProjectIdAsync(id);

            var rate = ExchangeRate.GetRate((!string.IsNullOrEmpty(currency) && currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase) ? Currency.USD : Currency.MXN));

            // Set prices to MXN for convenience
            var exchangeRate = ExchangeRate.GetRate("usd");
            foreach (var unit in units)
            {
                foreach (var unitPrice in unit.UnitPrices)
                {
                    if (unitPrice.Currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase))
                    {
                        unitPrice.Price *= exchangeRate;
                    }
                }
            }

            var unitsAvailables = units.Count(x => x.Status == UnitStatus.ForSale);
            var unitsSoldCondoHotel = units.Count(x => x.Status != UnitStatus.ForSale && x.IsCondoHotel);
            var isCondoHotelOptional = project.AllowsCondoHotel && ((unitsSoldCondoHotel + unitsAvailables) - project.MinimumCondoHotelUnits) > 0;

            var viewModel = units.Select(x => new UnitViewModel
            {
                ProjectId = id,
                Id = x.Id,
                Name = x.Model.Name,
                DisplayName = string.Format("{0} {1}", x.Number, x.Model.Name),
                GrossArea = x.GrossArea,
                BuiltUpArea = x.BuiltUpArea,
                TerraceArea = x.TerraceArea,
                Level = x.Level,
                MinimumExpectedValue = Math.Round(x.UnitPrices.Max(x => x.Price) / rate, 4),
                Number = x.Number,
                Price = Math.Round(x.UnitPrices.Where(p => p.StageId.ToString() == x.Model.Project.CurrentStageId.ToString()).Select(p => p.Price).FirstOrDefault() / rate, 4),
                Status = x.Status,
                IsActive = x.IsActive,
                IsCondoHotel = x.IsCondoHotel,
                AllowsCondoHotel = project.AllowsCondoHotel,
                IsCondoHotelOptional = isCondoHotelOptional,
                ProjectAppreciation = project.Appreciation,
                EquipmentPrice = Math.Round(unitsEquipmentByProject.Where(p => p.UnitId.Equals(x.Id)).Sum(p => p.Cost) / rate, 4),
                PreOperatingCost = x.Model.Project.CondoHotelPreOperatingCost.HasValue ? Math.Round(x.Model.Project.CondoHotelPreOperatingCost.Value / rate, 4) : 0m,
                Currency = (!string.IsNullOrEmpty(currency) && currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase) ? "usd" : "mxn")
            });

            return Ok(viewModel);
        }

        // GET api/projects/5/units/{number}
        [HttpGet("{id}/units/{number}")]
        public async Task<IActionResult> GetProjectUnitsByIdAndNumber(Guid id, int number, string currency)
        {
            var project = await _projectService.GetProjectByIdAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            var units = await _unitService.GetAllUnitsByProjectIdAsync(id);
            var unit = units.Take(number);
            if (unit == null)
            {
                return NotFound();
            }

            var unitsEquipmentByProject = await _unitEquipmentService.GetAllUnitEquipmentsByProjectIdAsync(id);

            var rate = ExchangeRate.GetRate((!string.IsNullOrEmpty(currency) && currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase) ? Currency.USD : Currency.MXN));

            var viewModel = units.Select(x => new UnitViewModel
            {
                ProjectId = id,
                Id = x.Id,
                Name = x.Model.Name,
                DisplayName = string.Format("{0} {1}", x.Number, x.Model.Name),
                GrossArea = x.GrossArea,
                BuiltUpArea = x.BuiltUpArea,
                TerraceArea = x.TerraceArea,
                Level = x.Level,
                MinimumExpectedValue = Math.Round(x.UnitPrices.Max(x => x.Price) / rate, 4),
                Number = x.Number,
                Price = Math.Round(x.UnitPrices.Where(p => p.StageId.ToString() == x.Model.Project.CurrentStageId.ToString()).Select(p => p.Price).FirstOrDefault() / rate, 4),
                Status = x.Status,
                IsActive = x.IsActive,
                IsCondoHotel = x.IsCondoHotel,
                AllowsCondoHotel = project.AllowsCondoHotel,
                ProjectAppreciation = project.Appreciation,
                EquipmentPrice = Math.Round(unitsEquipmentByProject.Where(p => p.UnitId.Equals(x.Id)).Sum(p => p.Cost) / rate, 4),
                PreOperatingCost = x.Model.Project.CondoHotelPreOperatingCost.HasValue ? Math.Round(x.Model.Project.CondoHotelPreOperatingCost.Value / rate, 4) : 0m,
                Currency = (!string.IsNullOrEmpty(currency) && currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase) ? "usd" : "mxn")
            });


            return Ok(viewModel);
        }

        // GET api/projects/5/tradepolicies
        [HttpGet("{id}/tradepolicies")]
        public async Task<IActionResult> GetProjectTradePolicyById(Guid id)
        {
            var tradePolicies = await _tradePolicyService.GetAllTradePoliciesByProjectIdAsync(id);

            if (tradePolicies == null)
            {
                return NotFound();
            }

            var viewModel = tradePolicies.Select(x => new TradePolicyViewModel
            {
                ProjectId = id,
                Id = x.Id,
                Name = x.Name,
                AdditionalPayments = x.AdditionalPayments,
                Deposit = x.Deposit,
                Discount = x.Discount,
                LastPayment = x.LastPayment,
                MonthlyPayments = x.MonthlyPayments,
                IsActive = x.IsActive
            });

            return Ok(viewModel);
        }

        // GET api/projects/5/stages
        [HttpGet("{id}/stages")]
        public async Task<IActionResult> GetProjectStagesById(Guid id)
        {
            var stages = await _stageService.GetAllStagesByProjectIdAsync(id);

            if (stages == null)
            {
                return NotFound();
            }

            var viewModel = stages.Select(x => new
            {
                ProjectId = id,
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive,
                Order = x.Order
            }).OrderBy(x => x.Order);

            return Ok(viewModel);
        }

        // POST api/projects
        [Authorize(Roles = "super_admin,admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectViewModel resource)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Project object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var project = new Project
            {
                Name = resource.Name,
                DisplayName = resource.DisplayName,
                Description = resource.Description,
                PhotoUrl = resource.PhotoUrl,
                ThemeColor = resource.ThemeColor,
                Location = resource.Location,
                AllowsCondoHotel = resource.AllowsCondoHotel,
                MinimumCondoHotelUnits = resource.MinimumCondoHotelUnits,
                CapRate = resource.CapRate,
                IsActive = true
            };

            await _projectService.CreateProjectAsync(project);

            var viewModel = new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                DisplayName = project.DisplayName,
                Description = project.Description,
                PhotoUrl = project.PhotoUrl,
                ThemeColor = project.ThemeColor,
                Location = project.Location,
                AllowsCondoHotel = project.AllowsCondoHotel,
                MinimumCondoHotelUnits = project.MinimumCondoHotelUnits,
                IsActive = project.IsActive
            };

            return Ok(viewModel);

            // return CreatedAtRoute("ProjectById", new { id = resource.Id }, resource);
        }

        // PUT api/projects/5
        [Authorize(Roles = "super_admin,admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectViewModel resource, Guid id)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Project object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var dbProject = await _projectService.GetProjectByIdAsync(id, false);
            if (dbProject == null)
            {
                return NotFound();
            }

            var project = new Project
            {
                Name = resource.Name,
                DisplayName = resource.DisplayName,
                Description = resource.Description,
                PhotoUrl = resource.PhotoUrl,
                ThemeColor = resource.ThemeColor,
                Location = resource.Location,
                DeliveryDate = resource.DeliveryDate,
                CurrentStageId = resource.CurrentStageId,
                AllowsCondoHotel = resource.AllowsCondoHotel,
                MinimumCondoHotelUnits = resource.MinimumCondoHotelUnits,
                CapRate = resource.CapRate == null ? Formulas.CapRate() : resource.CapRate,
            };

            await _projectService.UpdateProjectAsync(dbProject, project);

            return NoContent();
        }

        // DELETE api/projects/5
        [Authorize(Roles = "super_admin,admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            await _projectService.DeleteProjectAsync(project);

            return NoContent();
        }
    }
}