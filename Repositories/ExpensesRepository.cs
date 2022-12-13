using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Repositories
{
    public class ExpensesRepository : BaseRepository<Expense>, IExpensesRepository
    {
        public ExpensesRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Expense>> GetAllExpensesAsync()
        {
            return await FindAll()
                        .ToListAsync();
        }


        public async Task<Expense> GetExpenseByIdAsync(Guid expenseId)
        {
            return await FindByCondition(Expense => Expense.Id.Equals(expenseId))
                        .SingleOrDefaultAsync();
        }

        public async Task<Expense> GetExpenseByNameAsync(string name)
        {
            return await FindByCondition(Expense => Expense.Name.ToUpper() == name.ToUpper()).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task CreateExpenseAsync(Expense expense)
        {
            expense.Id = Guid.NewGuid();

            DetachLocal(expense, p => p.Id.Equals(expense.Id));
            Create(expense);
            await SaveAsync();
        }

        public async Task UpdateExpenseAsync(Expense dbExpense, Expense expense)
        {
            dbExpense.Name = expense.Name;
            dbExpense.DefaultValue = expense.DefaultValue;
            dbExpense.Type = expense.Type;

            DetachLocal(dbExpense, p => p.Id.Equals(dbExpense.Id));
            Update(dbExpense);
            await SaveAsync();
        }

        public Task DeleteExpenseAsync(Expense expense)
        {
            throw new NotImplementedException();
        }
    }
}