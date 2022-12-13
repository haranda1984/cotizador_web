using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;

namespace HeiLiving.Quotes.Api.Services
{
    public class TradePoliciesService : ITradePoliciesService
    {
        private readonly ITradePoliciesRepository _tradePolicyRepository;

        public TradePoliciesService(ITradePoliciesRepository tradePolicyRepository)
        {
            _tradePolicyRepository = tradePolicyRepository;
        }

        public Task CreateTradePolicyAsync(TradePolicy tradePolicy)
        {
            return _tradePolicyRepository.CreateTradePolicyAsync(tradePolicy);
        }

        public Task DeleteTradePolicyAsync(TradePolicy tradePolicy)
        {
            return _tradePolicyRepository.DeleteTradePolicyAsync(tradePolicy);
        }

        public Task<IEnumerable<TradePolicy>> GetAllTradePoliciesAsync()
        {
            return _tradePolicyRepository.GetAllTradePoliciesAsync();
        }

        public Task<IEnumerable<TradePolicy>> GetAllTradePoliciesByProjectIdAsync(Guid projectId)
        {
            return _tradePolicyRepository.GetAllTradePoliciesByProjectIdAsync(projectId);
        }

        public Task<TradePolicy> GetTradePolicyByIdAsync(Guid tradePolicyId)
        {
            return _tradePolicyRepository.GetTradePolicyByIdAsync(tradePolicyId);
        }

        public Task UpdateTradePolicyAsync(TradePolicy dbTradePolicy, TradePolicy tradePolicy)
        {
            return _tradePolicyRepository.UpdateTradePolicyAsync(dbTradePolicy, tradePolicy);
        }
    }
}