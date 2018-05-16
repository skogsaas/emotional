using Newtonsoft.Json;

namespace Emotional.Models
{
    public class RecognizeImageRequest : BaseFrame
    {
        [JsonProperty("imageBase64")]
        public string ImageBase64 { get; set; }

        public const string Typename = "recognizeimage";

        public RecognizeImageRequest()
        {
            Type = Typename.ToUpper();
        }
    }
}
