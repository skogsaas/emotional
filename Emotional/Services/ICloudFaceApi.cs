using Emotional.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emotional.Services
{
    public interface ICloudFaceApi
    {
        Task<RecognitionResult.Face[]> DetectEmotions(FaceCropResult.Face[] faces);
    }
}
