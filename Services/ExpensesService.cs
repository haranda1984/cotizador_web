using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;

namespace HeiLiving.Quotes.Api.Services
{
    public class ExpensesService : IExpensesService
    {
        private readonly IExpensesRepository _stageRepository;

        public ExpensesService(IExpensesRepository stageRepository)
        {
            _stageRepository = stageRepository;
        }

        public Task<IEnumerable<Expense>> GetAllExpensesAsync()
        {
            return _stageRepository.GetAllExpensesAsync();
        }

        public Task<Expense> GetExpenseByIdAsync(Guid stageId)
        {
            return _stageRepository.GetExpenseByIdAsync(stageId);
        }

        public Task<Expense> GetExpenseByNameAsync(string name)
        {
            return _stageRepository.GetExpenseByNameAsync(name);
        }

        public Task CreateExpenseAsync(Expense stage)
        {
            return _stageRepository.CreateExpenseAsync(stage);
        }

        public Task UpdateExpenseAsync(Expense dbstage, Expense stage)
        {
            return _stageRepository.UpdateExpenseAsync(dbstage, stage);
        }
        public Task DeleteExpenseAsync(Expense stage)
        {
            return _stageRepository.DeleteExpenseAsync(stage);
        }
    }
}