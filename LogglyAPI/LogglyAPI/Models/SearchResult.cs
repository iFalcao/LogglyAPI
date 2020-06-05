using Newtonsoft.Json;

namespace LogglyAPI.Models
{
    public class SearchResult
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("elapsed_time")]
        public decimal ElapsedTime { get; set; }
        
        [JsonProperty("date_from")]
        public long DateFrom { get; set; }
        
        [JsonProperty("date_to")]
        public long DateTo { get; set; }
    }
}
