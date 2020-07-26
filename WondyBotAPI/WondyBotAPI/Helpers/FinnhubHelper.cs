using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WondyBotAPI.Models;

namespace WondyBotAPI.Helpers
{
    public class FinnhubHelper
    {
        private readonly HttpClient _httpClient;

        public FinnhubHelper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Symbol>> GetLatestSymbolsFromEndpoint(string domain, string apiToken)
        {
            var usSymbols = GetLastestExchangeSymbols(domain, "US", apiToken);
            var ukSymbols = GetLastestExchangeSymbols(domain, "L", apiToken);
            var chinaSymbols = GetLastestExchangeSymbols(domain, "T", apiToken);

            await Task.WhenAll(usSymbols, ukSymbols, chinaSymbols);

            var companyList = new List<Symbol>();
            companyList.AddRange(await usSymbols);
            companyList.AddRange(await ukSymbols);
            companyList.AddRange(await chinaSymbols);

            return companyList;
        }

        private async Task<IEnumerable<Symbol>> GetLastestExchangeSymbols(string domain, string exchange, string apiToken)
        {
            var symbolPayload = await _httpClient.GetAsync($"{domain}stock/symbol?exchange={exchange}&token={apiToken}");

            var listPayload = JsonConvert.DeserializeObject<IEnumerable<Symbol>>(await symbolPayload.Content.ReadAsStringAsync());
            return listPayload;
        }
    }
}
