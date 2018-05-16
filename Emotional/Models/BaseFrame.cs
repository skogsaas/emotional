using Newtonsoft.Json;

namespace Emotional.Models
{
    public class BaseFrame
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }
    }
}
