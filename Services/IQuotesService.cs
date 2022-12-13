using System;
using HeiLiving.Quotes.Api.Models;
using DinkToPdf;

namespace HeiLiving.Quotes.Api.Services
{
    public interface IQuotesService
    {
        QuoteResultViewModel Calculate(QuoteInputViewModel input);
        HtmlToPdfDocument ExportSummaryToPdf(QuoteInputViewModel input, QuoteResultViewModel summary, Guid userId);
        void CreateUpdateHubspotDeal(QuoteInputViewModel inputViewModel, QuoteResultViewModel summary, Guid userId);
    }
}