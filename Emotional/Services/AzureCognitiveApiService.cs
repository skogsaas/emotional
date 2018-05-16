using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emotional.Models;

namespace Emotional.Services
{
    public class AzureCognitiveApiService : ICloudFaceApi
    {
        public AzureCognitiveApiService()
        {

        }

        public Task<RecognitionResult.Face[]> DetectEmotions(FaceCropResult.Face[] faces)
        {
            throw new NotImplementedException();
        }
    }
}
