using System;

namespace HeiLiving.Quotes.Api.Models
{
    public class TradePolicyViewModel
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public decimal Discount { get; set; }
        public decimal Deposit { get; set; }
        public decimal LastPayment { get; set; }
        public decimal AdditionalPayments { get; set; }
        public int MonthlyPayments { get; set; }
        public bool IsActive { get; set; }
    }
}