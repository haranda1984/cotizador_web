using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeiLiving.Quotes.Api.Repositories
{
    public class TradePoliciesRepository :  BaseRepository<TradePolicy>, ITradePoliciesRepository
    {
        public TradePoliciesRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task CreateTradePolicyAsync(TradePolicy tradePolicy)
        {
            tradePolicy.Id = Guid.NewGuid();
            tradePolicy.CreatedAt = tradePolicy.ModifiedAt = DateTime.UtcNow;

            DetachLocal(tradePolicy, p => p.Id.Equals(tradePolicy.Id));
            Create(tradePolicy);
            await SaveAsync();
        }

        public async Task DeleteTradePolicyAsync(TradePolicy tradePolicy)
        {
            tradePolicy.IsActive = false;
            tradePolicy.ModifiedAt = DateTime.UtcNow;

            DetachLocal(tradePolicy, p => p.Id.Equals(tradePolicy.Id));
            Update(tradePolicy);
            await SaveAsync();
        }

        public async Task<IEnumerable<TradePolicy>> GetAllTradePoliciesAsync()
        {
            return await FindAll()
                        .Include(p => p.Project).AsNoTracking()
                        .ToListAsync();
        }

        public async Task<IEnumerable<TradePolicy>> GetAllTradePoliciesByProjectIdAsync(Guid projectId)
        {
            return await FindByCondition(tradePolicy => tradePolicy.ProjectId.Equals(projectId))
                        .Include(p => p.Project).AsNoTracking()
                        .ToListAsync();
        }

        public async Task<TradePolicy> GetTradePolicyByIdAsync(Guid tradePolicyId)
        {
            return await FindByCondition(tradePolicy => tradePolicy.Id.Equals(tradePolicyId))
                        .Include(p => p.Project).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task UpdateTradePolicyAsync(TradePolicy dbTradePolicy, TradePolicy tradePolicy)
        {
            dbTradePolicy.Name = tradePolicy.Name;
            dbTradePolicy.LastPayment = tradePolicy.LastPayment;
            dbTradePolicy.MonthlyPayments = tradePolicy.MonthlyPayments;
            dbTradePolicy.AdditionalPayments = tradePolicy.AdditionalPayments;
            dbTradePolicy.Deposit = tradePolicy.Deposit;
            dbTradePolicy.Discount = tradePolicy.Discount;
            dbTradePolicy.ModifiedAt = DateTime.UtcNow;
            dbTradePolicy.IsActive = tradePolicy.IsActive;

            DetachLocal(dbTradePolicy, p => p.Id.Equals(dbTradePolicy.Id));
            Update(dbTradePolicy);
            await SaveAsync();
        }
    }
}