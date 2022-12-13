using System;

namespace HeiLiving.Quotes.Api.Entities
{
    public class TradePolicy
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public string Name { get; set; }
        public decimal Discount { get; set; }
        public decimal Deposit { get; set; }
        public decimal LastPayment { get; set; }
        public decimal AdditionalPayments { get; set; }
        public int MonthlyPayments { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }  
    }
}