using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Services
{
    public interface ITradePoliciesService
    {
        Task<IEnumerable<TradePolicy>> GetAllTradePoliciesAsync();
        Task<IEnumerable<TradePolicy>> GetAllTradePoliciesByProjectIdAsync(Guid projectId);
        Task<TradePolicy> GetTradePolicyByIdAsync(Guid tradePolicyId);
        Task CreateTradePolicyAsync(TradePolicy tradePolicy);
        Task UpdateTradePolicyAsync(TradePolicy dbTradePolicy, TradePolicy tradePolicy);
        Task DeleteTradePolicyAsync(TradePolicy tradePolicy);
    }
}