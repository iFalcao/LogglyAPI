using LogglyAPI.Converters;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

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
        [JsonConverter(typeof(EpochToDateTimeJsonConverter))]
        public DateTime DateFrom { get; set; }
        
        [JsonProperty("date_to")]
        [JsonConverter(typeof(EpochToDateTimeJsonConverter))]
        public DateTime DateTo { get; set; }
    }

    public enum SearchOrder
    {
        [Description("desc")]
        DESC,
        [Description("asc")]
        ASC
    }
}
