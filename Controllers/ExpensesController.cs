using System;
using System.Linq;
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
    public class ExpensesController : ControllerBase
    {
        private readonly IExpensesService _Expenseservice;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ExpensesController(IExpensesService Expenseservice, IStringLocalizer<SharedResource> localizer)
        {
            _Expenseservice = Expenseservice;
            _localizer = localizer;
        }

        // GET api/Expenses
        [HttpGet]
        public async Task<IActionResult> GetAllExpensesAsync()
        {
            var expenses = await _Expenseservice.GetAllExpensesAsync();

            if (expenses == null)
            {
                return NotFound();
            }

            var viewModel = expenses.Select(expense => new ExpenseViewModel
            {
                Id = expense.Id,
                Name = expense.Name,
                DefaultValue = expense.DefaultValue,
            }).OrderBy(e => e.Name);                
            return Ok(viewModel);
        }


        // GET api/Expenses/5
        [HttpGet("{id}", Name = "ExpenseById")]
        public async Task<IActionResult> GetExpenseByIdAsync(Guid expenseId)
        {
            var expense = await _Expenseservice.GetExpenseByIdAsync(expenseId);

            if (expense == null)
            {
                return NotFound();
            }

            var viewModel = new ExpenseViewModel
            {
                Id = expense.Id,
                Name = expense.Name,
                DefaultValue = expense.DefaultValue,
            };

            return Ok(viewModel);
        }

        // GET api/Expenses/{name}
        [HttpGet("{name}", Name = "ExpenseByName")]
        public async Task<IActionResult> GetExpenseByNameAsync(string name)
        {
            var expense = await _Expenseservice.GetExpenseByNameAsync(name);

            if (expense == null)
            {
                return NotFound();
            }

            var viewModel = new ExpenseViewModel
            {
                Id = expense.Id,
                Name = expense.Name,
                DefaultValue = expense.DefaultValue,
            };

            return Ok(viewModel);
        }

        // POST api/Expenses
        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] ExpenseViewModel resource)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Expense object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var expense = new Expense
            {
                Id = resource.Id,
                Name = resource.Name,
                DefaultValue = resource.DefaultValue,
            };

            await _Expenseservice.CreateExpenseAsync(expense);

            return CreatedAtRoute("ExpenseById", new { id = resource.Id }, resource);
        }

        // PUT api/Expenses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense([FromBody] ExpenseViewModel resource, Guid id)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Expense object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var dbExpense = await _Expenseservice.GetExpenseByIdAsync(id);
            if (dbExpense == null)
            {
                return NotFound();
            }

            var expense = new Expense
            {
                Id = resource.Id,
                Name = resource.Name,
                DefaultValue = resource.DefaultValue,
            };

            await _Expenseservice.UpdateExpenseAsync(dbExpense, expense);

            return NoContent();
        }

        // DELETE api/Expenses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            var expense = await _Expenseservice.GetExpenseByIdAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            await _Expenseservice.DeleteExpenseAsync(expense);

            return NoContent();
        }
    }
}