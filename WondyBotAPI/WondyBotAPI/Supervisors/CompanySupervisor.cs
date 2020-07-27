using F23.StringSimilarity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WondyBotAPI.Helpers;
using WondyBotAPI.Models;

namespace WondyBotAPI.Supervisors
{
    public class CompanySupervisor
    {
        private readonly IMemoryCache _cache;
        private readonly FinnhubHelper _helper;
        private readonly IConfiguration _config;

        private readonly string _domain;
        private readonly string _apiToken;

        public CompanySupervisor(IMemoryCache memoryCache, FinnhubHelper helper, IConfiguration config)
        {
            _cache = memoryCache;
            _helper = helper;
            _config = config;

            _domain = _config["Finnhub:apiEndpoint"];
            _apiToken = _config["Finnhub:apiToken"];
        }

        internal async Task<IEnumerable<Symbol>> GetCompanyNameMatches(string companyId)
        {
            //get list of symbols from cache or Finnhub
            var symbolsList = await GetSymbolsFromCache();

            var matches = new List<Symbol>();
            var jw = new JaroWinkler();

            //is user input either a company name or ticker
            foreach (var symbol in symbolsList)
            {
                var comparisonName = jw.Similarity(companyId.ToLower(), symbol.Name.ToLower());
                var comparisonTicker = jw.Similarity(companyId.ToLower(), symbol.Ticker.ToLower());
                if (comparisonName > 0.85 || comparisonTicker > 0.90)
                {
                    matches.Add(symbol);
                }
            }

            return matches;
        }

        private async Task<IEnumerable<Symbol>> GetSymbolsFromCache()
        {
            var cacheSymbols = _cache.Get<IEnumerable<Symbol>>("cacheSymbols");

            if (cacheSymbols == null)
            {
                cacheSymbols = await GetLatestSymbolsFromEndpoint();
                _cache.Set("cacheSymbols", cacheSymbols, TimeSpan.FromDays(1));
            }

            return cacheSymbols;
        }

        public async Task<IEnumerable<Symbol>> GetLatestSymbolsFromEndpoint()
        {
            var arrayOfExchanges = _config.GetSection("Finnhub:exchangeArray").Get<string[]>();

            var taskList = new List<Task<IEnumerable<Symbol>>>();

            foreach (var exchange in arrayOfExchanges)
            {
                taskList.Add(_helper.GetSymbolList($"{_domain}stock/symbol?exchange={exchange}&token={_apiToken}"));
            }

            await Task.WhenAll(taskList);


            var companyList = new List<Symbol>();

            foreach(var t in taskList)
            {
                var symbols = await t;
                companyList.AddRange(symbols);
            }

            return companyList;
        }


        internal async Task<string> GetCompanyDetails(string ticker)
        {
            var profilePayload = _helper.GetCompanyData($"{_domain}stock/profile2?symbol={ticker}&token={_apiToken}");
            var quotePayload = _helper.GetCompanyData($"{_domain}quote?symbol={ticker}&token={_apiToken}");
            var targetPricePayload = _helper.GetCompanyData($"{_domain}stock/price-target?symbol={ticker}&token={_apiToken}");
            var reccomendationPayload = _helper.GetCompanyData($"{_domain}stock/recommendation?symbol={ticker}&token={_apiToken}");

            await Task.WhenAll(profilePayload, quotePayload, targetPricePayload, reccomendationPayload);

            var companyDetail = new CompanyDetail(await profilePayload, await quotePayload, await targetPricePayload, await reccomendationPayload);
            return companyDetail.ToString();
        }
    }
}