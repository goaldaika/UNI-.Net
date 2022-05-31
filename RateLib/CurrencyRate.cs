
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RateLib
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

            string url = "https://openexchangerates.org/api/latest.json?app_id=69cb235f2fe74f03baeec270066587cf";

            WebClient myClient = new WebClient();
            string txt = myClient.DownloadString(url);

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var latest = JsonSerializer.Deserialize<CurrencyRate>(txt, options);


            double value = 0;

            if (latest.Rates.TryGetValue(ExchangeRate, out value))
            {
                return value;
            }
            else throw new InvalidOperationException("Valid type not found.");


        }
    }



}

#region rác
//Console.WriteLine(latest.Rates.First());
//Console.WriteLine(string.Join("\n", latest.Rates.Select(rate => $"{rate.Key},{rate.Value}")));
//return string.Join("\n", latest.Rates.Select(rate => $"{rate.Key},{rate.Value}"));
#endregion
