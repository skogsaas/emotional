using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emotional.Services.GoogleVisionModels
{
    public class AnnotationResponse
    {
        [JsonProperty("responses")]
        public AnnotateImageResponse[] Responses { get; set; }

        public struct AnnotateImageResponse
        {
            [JsonProperty("faceAnnotations")]
            public FaceAnnotations[] FaceAnnotationData { get; set; }

            public struct FaceAnnotations
            {
                [JsonProperty("joyLikelihood")]
                public string Joy { get; set; }

                [JsonProperty("sorrowLikelihood")]
                public string Sorrow { get; set; }

                [JsonProperty("angerLikelihood")]
                public string Anger { get; set; }

                [JsonProperty("surpriseLikelihood")]
                public string Surprise { get; set; }
            }
        }
    }
}
