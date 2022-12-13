using System;

namespace HeiLiving.Quotes.Api.Models
{
    public class ExpenseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal DefaultValue { get; set; }
    }
}