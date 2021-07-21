using Newtonsoft.Json;
using System.Collections.Generic;
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

        public async Task<IEnumerable<Symbol>> GetSymbolList(string url)
        {
            var symbolPayload = await _httpClient.GetAsync(url);

            var listPayload = JsonConvert.DeserializeObject<IEnumerable<Symbol>>(await symbolPayload.Content.ReadAsStringAsync());
            return listPayload;
        }

        public async Task<string> GetCompanyData(string url)
        {
            var symbolPayload = await _httpClient.GetAsync(url);
            var content = await symbolPayload.Content.ReadAsStringAsync();
            return content;
            //var listPayload = JsonConvert.DeserializeObject<dynamic>(content);
            //return listPayload;
        }
    }
}
