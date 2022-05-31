using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace RateLib2
{
    public class CurrencyRate
    {
        public string Disclaimer { get; set; }
        public string License { get; set; }
        public int Timestamp { get; set; }
        public string Base { get; set; }
        public IDictionary<string, double> Rates { get; set; }
        public double getRate(string ExchangeRate)
        {

            string url = "https://api.exchangerate-api.com/v4/latest/USD";

            WebClient myClient = new WebClient();
            string txt = myClient.DownloadString(url);

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var latest = JsonSerializer.Deserialize<CurrencyRate>(txt, options);


            double value = 0;

            if (latest.Rates.TryGetValue(ExchangeRate, out value))
            {
                return value;
            }
            else throw new InvalidOperationException("Currency type not found!");


        }
    }
}
