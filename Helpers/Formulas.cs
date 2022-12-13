using System;

namespace HeiLiving.Quotes.Api.Helpers
{
    public static class Formulas
    {
        private const decimal RentFactor = 0.06m;

        public static decimal ReturnOfInvestment()
        {
            throw new NotImplementedException();
        }

        public static decimal CapitalGain()
        {
            throw new NotImplementedException();
        }

        public static decimal CapRate()
        {
            return RentFactor;
        }

        public static int MonthsDiff(DateTime start, DateTime end)
        {
            return ((end.Year - start.Year) * 12) + end.Month - start.Month;
        }

        public static int RoundUp(int n, int m)
        {
            return (n + (m - 1)) / m * m;
        }
    }
}