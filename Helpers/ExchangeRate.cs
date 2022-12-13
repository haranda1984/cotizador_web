using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace HeiLiving.Quotes.Api.Helpers
{
    public static class ExchangeRate
    {
        private static readonly HttpClient httpClient = new HttpClient();
        
        public static decimal GetRate(string currency)
        {
            return GetRate((!string.IsNullOrEmpty(currency) && currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase) ? Currency.USD : Currency.MXN));
        }

        public static decimal GetRate(Currency currency)
        {
            var rate = 0m;

            switch(currency)
            {
                case Currency.USD:
                    rate = GetBmxRate();
                    break;
                default:
                    rate = 1m;
                    break;
            }

            return rate;
        }

        private static decimal GetBmxRate()
        {
            try
            {
                string url = "https://www.banxico.org.mx/SieAPIRest/service/v1/series/SF43717/datos/oportuno";
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Accept = "application/json";
                request.Headers["Bmx-Token"] = "7e2a1c077923e087fdadc5661fffade3f75096cbed8284211e1993ca4d703948";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format(
                    "Server error (HTTP {0}: {1}).",
                    response.StatusCode,
                    response.StatusDescription));
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(BmxResponse));
                object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                BmxResponse jsonResponse = objResponse as BmxResponse;
                var data = jsonResponse.SeriesResponse.Series[0].Data[0].Data;
                return decimal.Parse(data, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 19m;
            }
        }
    }

    public enum Currency
    {
        MXN,
        USD
    }

    [DataContract]
    class BmxSerie
    {
        [DataMember(Name = "titulo")]
        public string Title { get; set; }

        [DataMember(Name = "idSerie")]
        public string IdSerie { get; set; }

        [DataMember(Name = "datos")]
        public BmxDataSerie[] Data { get; set; }

    }

    [DataContract]
    class BmxDataSerie
    {
        [DataMember(Name = "fecha")]
        public string Date { get; set; }

        [DataMember(Name = "dato")]
        public string Data { get; set; }
    }

    [DataContract]
    class BmxSeriesResponse
    {
        [DataMember(Name = "series")]
        public BmxSerie[] Series { get; set; }
    }

    [DataContract]
    class BmxResponse
    {
        [DataMember(Name = "bmx")]
        public BmxSeriesResponse SeriesResponse { get; set; }
    }
}