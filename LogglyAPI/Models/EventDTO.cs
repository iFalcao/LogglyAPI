using LogglyAPI.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LogglyAPI.Models
{
    public class EventsResult
    {
        [JsonProperty("total_events")]
        public long TotalEvents { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("events")]
        public IEnumerable<EventDTO> Events { get; set; }
    }

    public class EventsIteratorResult
    {
        [JsonProperty("next")]
        public string NextPageUrl { get; set; }

        [JsonProperty("events")]
        public IEnumerable<EventDTO> Events { get; set; }
    }

    public class EventDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("tags")]
        public IEnumerable<string> Tags { get; set; }

        [JsonProperty("logtypes")]
        public IEnumerable<string> LogTypes { get; set; }

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(EpochToDateTimeJsonConverter))]
        public DateTime Timestamp { get; set; }

        [JsonProperty("logmsg")]
        public string Message { get; set; }

        [JsonProperty("event")]
        public object ParsedObject { get; set; }

        [JsonProperty("raw")]
        public string Raw { get; set; }

        [JsonProperty("unparsed", NullValueHandling = NullValueHandling.Ignore)]
        public object Unparsed { get; set; }
    }
}
