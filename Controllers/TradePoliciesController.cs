using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using HeiLiving.Quotes.Api.Services;
using HeiLiving.Quotes.Api.Models;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Controllers
{
    [Authorize(Roles = "super_admin,admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class TradePoliciesController : ControllerBase
    {
        private readonly ITradePoliciesService _tradePolicyService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public TradePoliciesController(ITradePoliciesService tradePolicyService, IStringLocalizer<SharedResource> localizer)
        {
            _tradePolicyService = tradePolicyService;
            _localizer = localizer;
        }

        // GET api/tradePolicies/5
        [HttpGet("{id}", Name = "TradePolicyById")]
        public async Task<IActionResult> GetTradePolicyById(Guid id)
        {
            var tradePolicy = await _tradePolicyService.GetTradePolicyByIdAsync(id);

            if (tradePolicy == null)
            {
                return NotFound();
            }

            var viewModel = new TradePolicyViewModel
            {
                Id = tradePolicy.Id,
                Name = tradePolicy.Name,
                Discount = tradePolicy.Discount,
                Deposit = tradePolicy.Deposit,
                LastPayment = tradePolicy.LastPayment,
                AdditionalPayments = tradePolicy.AdditionalPayments,
                MonthlyPayments = tradePolicy.MonthlyPayments,
                IsActive = tradePolicy.IsActive,
            };

            return Ok(viewModel);
        }

        // POST api/tradePolicies
        [HttpPost]
        public async Task<IActionResult> CreateTradePolicy([FromBody] TradePolicyViewModel resource)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["TradePolicy object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var tradePolicy = new TradePolicy
            {
                ProjectId = resource.ProjectId,
                Name = resource.Name,
                Discount = resource.Discount,
                Deposit = resource.Deposit,
                LastPayment = resource.LastPayment,
                AdditionalPayments = resource.AdditionalPayments,
                MonthlyPayments = resource.MonthlyPayments,
                IsActive = resource.IsActive,
            };

            await _tradePolicyService.CreateTradePolicyAsync(tradePolicy);

            return Ok(new
            {
                Id = tradePolicy.Id,
                ProjectId = tradePolicy.ProjectId,
                Name = tradePolicy.Name,
                Discount = tradePolicy.Discount,
                Deposit = tradePolicy.Deposit,
                LastPayment = tradePolicy.LastPayment,
                AdditionalPayments = tradePolicy.AdditionalPayments,
                MonthlyPayments = tradePolicy.MonthlyPayments,
                IsActive = tradePolicy.IsActive,
            });
            // return CreatedAtRoute("TradePolicyById", new { id = resource.Id }, resource);
        }

        // PUT api/tradePolicies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTradePolicy([FromBody] TradePolicyViewModel resource, Guid id)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["TradePolicy object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var dbTradePolicy = await _tradePolicyService.GetTradePolicyByIdAsync(id);
            if (dbTradePolicy == null)
            {
                return NotFound();
            }

            var tradePolicy = new TradePolicy
            {
                Name = resource.Name,
                Discount = resource.Discount,
                Deposit = resource.Deposit,
                LastPayment = resource.LastPayment,
                AdditionalPayments = resource.AdditionalPayments,
                MonthlyPayments = resource.MonthlyPayments,
                IsActive = resource.IsActive,
            };

            await _tradePolicyService.UpdateTradePolicyAsync(dbTradePolicy, tradePolicy);

            return NoContent();
        }

        // DELETE api/tradePolicies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTradePolicy(Guid id)
        {
            var tradePolicy = await _tradePolicyService.GetTradePolicyByIdAsync(id);
            if (tradePolicy == null)
            {
                return NotFound();
            }

            await _tradePolicyService.DeleteTradePolicyAsync(tradePolicy);

            return NoContent();
        }
    }
}