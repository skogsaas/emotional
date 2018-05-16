using Newtonsoft.Json;
using System;

namespace Emotional.Models
{
    public class FaceCropResult : BaseFrame
    {
        [JsonProperty("captureTime")]
        public DateTime CaptureTime { get; set; }

        [JsonProperty("faces")]
        public Face[] Faces { get; set; }

        public struct Face
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("imageBase64")]
            public string ImageBase64 { get; set; }
        }

        public static readonly string Typename = "FaceCrop";

        public FaceCropResult()
        {
            Type = Typename.ToUpper();
        }
    }
}
