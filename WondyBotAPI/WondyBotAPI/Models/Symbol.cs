using Newtonsoft.Json;

namespace WondyBotAPI.Models
{
    public class Symbol
    {
        public int SymbolId { get; set; }

        [JsonProperty("symbol")]
        public string Ticker { get; set; }

        [JsonProperty("description")]
        public string Name { get; set; }
    }
}