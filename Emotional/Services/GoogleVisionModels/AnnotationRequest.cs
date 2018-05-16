using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emotional.Services.GoogleVisionModels
{
    public class AnnotationRequest
    {
        [JsonProperty("requests")]
        public AnnotateImageRequest[] Requests { get; set; }

        public struct AnnotateImageRequest
        {
            [JsonProperty("image")]
            public Image ImageData { get; set; }

            [JsonProperty("features")]
            public Feature[] Features { get; set; }

            public struct Image
            {
                [JsonProperty("content")]
                public string Content { get; set; }
            }

            public struct Feature
            {
                [JsonProperty("type")]
                public string Type { get; set; }

                [JsonProperty("maxResults")]
                public int MaxResults { get; set; }
            }
        }
    }
}
