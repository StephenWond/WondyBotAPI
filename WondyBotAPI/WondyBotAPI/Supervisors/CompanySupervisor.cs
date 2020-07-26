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
        private readonly IConfiguration _config;
        private readonly FinnhubHelper _helper;

        private readonly string _domain;
        private readonly string _apiToken;

        public CompanySupervisor(IMemoryCache memoryCache, IConfiguration config, FinnhubHelper helper)
        {
            _cache = memoryCache;
            _config = config;
            _helper = helper;

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

        internal Task GetCompanyDetails(string ticker)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<Symbol>> GetSymbolsFromCache()
        {
            var cacheSymbols = _cache.Get<IEnumerable<Symbol>>("cacheSymbols");

            if (cacheSymbols == null)
            {
                cacheSymbols = await _helper.GetLatestSymbolsFromEndpoint(_domain, _apiToken);
                _cache.Set("cacheSymbols", cacheSymbols, TimeSpan.FromDays(1));
            }

            return cacheSymbols;
        }
    }
}