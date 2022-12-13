using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Repositories
{
    public interface IExpensesRepository : IBaseRepository<Expense>
    {
        Task<IEnumerable<Expense>> GetAllExpensesAsync();
        Task<Expense> GetExpenseByIdAsync(Guid expenseId);
        Task<Expense> GetExpenseByNameAsync(string name);
        Task CreateExpenseAsync(Expense expense);
        Task UpdateExpenseAsync(Expense dbExpense, Expense expense);
        Task DeleteExpenseAsync(Expense expense);
    }
}

