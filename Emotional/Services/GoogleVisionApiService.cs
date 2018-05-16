using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Emotional.Models;
using Emotional.Services.GoogleVisionModels;
using Newtonsoft.Json;

namespace Emotional.Services
{
    public class GoogleVisionApiService : ICloudFaceApi
    {
        private const string _apiKey = "AIzaSyDmLt3kQwKwcfoat6M07S8noOix2kBvQ-w";

        public async Task<RecognitionResult.Face[]> DetectEmotions(FaceCropResult.Face[] faces)
        {
            AnnotationRequest request = new AnnotationRequest();
            request.Requests = new AnnotationRequest.AnnotateImageRequest[faces.Length];
            
            for(int i = 0; i < faces.Length; i++)
            {
                FaceCropResult.Face face = faces[i];

                var r = new AnnotationRequest.AnnotateImageRequest
                {
                    ImageData = new AnnotationRequest.AnnotateImageRequest.Image
                    {
                        Content = face.ImageBase64
                    },
                    Features = new AnnotationRequest.AnnotateImageRequest.Feature[]
                    {
                        new AnnotationRequest.AnnotateImageRequest.Feature
                        {
                            Type = "FACE_DETECTION",
                            MaxResults = 5
                        }
                    }
                };

                request.Requests[i] = r;
            }

            try
            {
                HttpClient client = new HttpClient();
                HttpContent content = new StringContent(JsonConvert.SerializeObject(request));

                var httpResponse = await client.PostAsync("https://vision.googleapis.com/v1/images:annotate?key=" + _apiKey, content);

                string responseData = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.IsSuccessStatusCode)
                {
                    AnnotationResponse response = JsonConvert.DeserializeObject<AnnotationResponse>(responseData);

                    List<RecognitionResult.Face> faceResults = new List<RecognitionResult.Face>();

                    for (int i = 0; i < response.Responses.Length && i < faces.Length; i++)
                    {
                        AnnotationResponse.AnnotateImageResponse.FaceAnnotations faceAnnotations = response.Responses[i].FaceAnnotationData[0];
                        RecognitionResult.Face faceResult = new RecognitionResult.Face
                        {
                            Emotion = new RecognitionResult.Emotion
                            {
                                Anger = FromLikelyhood(faceAnnotations.Anger),
                                Happiness = FromLikelyhood(faceAnnotations.Joy),
                                Neutral = 0.0,
                                Sadness = FromLikelyhood(faceAnnotations.Sorrow),
                                Surprise = FromLikelyhood(faceAnnotations.Surprise)
                            }
                        };

                        faceResults.Add(faceResult);
                    }

                    return faceResults.ToArray();
                }
            }
            catch(Exception ex)
            {
                // TODO?
            }

            return null;
        }

        private static double FromLikelyhood(string like)
        {
            switch(like.ToUpper())
            {
                default:
                case "VERY_UNLIKELY":
                    return 0.0;

                case "UNLIKELY":
                    return 0.25;

                case "POSSIBLE":
                    return 0.5;

                case "LIKELY":
                    return 0.75;

                case "VERY_LIKELY":
                    return 1.0;
            }
        }
    }
}
