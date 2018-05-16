using Newtonsoft.Json;
using System;

namespace Emotional.Models
{
    public class RecognitionResult : BaseFrame
    {
        [JsonProperty("captureTime")]
        public DateTime CaptureTime { get; set; }

        [JsonProperty("faces")]
        public Face[] Faces { get; set; }

        public struct Face
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("emotion")]
            public Emotion Emotion { get; set; }
        }

        public struct Emotion
        {
            [JsonProperty("anger")]
            public double Anger { get; set; }

            [JsonProperty("happiness")]
            public double Happiness { get; set; }

            [JsonProperty("neutral")]
            public double Neutral { get; set; }

            [JsonProperty("sadness")]
            public double Sadness { get; set; }

            [JsonProperty("surprise")]
            public double Surprise { get; set; }
        }

        public static readonly string Typename = "RecognitionResult";

        public RecognitionResult()
        {
            Type = Typename.ToUpper();
        }
    }
}
