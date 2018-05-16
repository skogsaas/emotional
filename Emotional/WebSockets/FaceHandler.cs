using Emotional.Middlewares.WebSocketHandler;
using Emotional.Models;
using Emotional.Services;
using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Emotional.WebSockets
{
    public class FaceHandler : WebSocketConnection
    {
        private ICloudFaceApi _emotionApi;
        private VideoService _video;

        private CascadeClassifier _classifier;

        private TransformBlock<ImageCapture, FaceCropResult> _faceCrop;
        private TransformBlock<FaceCropResult, RecognitionResult> _recognition;
        private BroadcastBlock<FaceCropResult> _faceCropBroadcast;
        private ActionBlock<object> _sendResult;

        private struct ImageCapture
        {
            public DateTime captureTime;
            public string tag;
            public Mat image;
        }

        public FaceHandler(WebSocket webSocket, ICloudFaceApi emotionApi, VideoService video, int bufferSize = 65536)
            : base(webSocket, bufferSize)
        {
            _emotionApi = emotionApi;
            _video = video;
            _classifier = new CascadeClassifier("classifiers/haarcascade_frontalface_alt2.xml");

            _faceCrop = new TransformBlock<ImageCapture, FaceCropResult>(i => DetectFaces(i));
            _faceCropBroadcast = new BroadcastBlock<FaceCropResult>(f => f);
            _recognition = new TransformBlock<FaceCropResult, RecognitionResult>(f => RecognizeEmotions(f));
            _sendResult = new ActionBlock<object>(r => SendResult(r));

            _faceCrop.LinkTo(_faceCropBroadcast);

            _faceCropBroadcast.LinkTo(_recognition);
            _faceCropBroadcast.LinkTo(_sendResult);

            _recognition.LinkTo(_sendResult);
        }

        ~FaceHandler()
        {
            _video.Stop();
        }

        public override Task ReceiveMessageAsync(string message)
        {
            return Task.Run(() =>
            {
                BaseFrame frame = JsonConvert.DeserializeObject<BaseFrame>(message);

                switch(frame.Type.ToLower())
                {
                    case StartCameraRequest.Typename:
                        StartCamera();
                        break;

                    case RecognizeImageRequest.Typename:
                        RecognizeImage(JsonConvert.DeserializeObject<RecognizeImageRequest>(message));
                        break;
                }
            });
        }

        private void StartCamera()
        {
            _video.Start();
            _video.OnCapture = OnVideoCapture;
        }

        private void OnVideoCapture(Mat image)
        {
            ImageCapture capture = new ImageCapture
            {
                tag = "",
                captureTime = DateTime.Now,
                image = image.Clone()
            };

            _faceCrop.Post(capture);
        }

        private void RecognizeImage(RecognizeImageRequest req)
        {
            ImageCapture capture = new ImageCapture
            {
                tag = req.Tag,
                captureTime = DateTime.Now, // TODO: Get this from the request
                image = Mat.FromImageData(Convert.FromBase64String(req.ImageBase64))
            };

            _faceCrop.Post(capture);
        }

        private FaceCropResult DetectFaces(ImageCapture capture)
        {
            Rect[] rects = _classifier.DetectMultiScale(capture.image, 1.08, 2, HaarDetectionType.ScaleImage, new Size(30, 30));

            FaceCropResult faces = new FaceCropResult
            {
                CaptureTime = capture.captureTime,
                Faces = new FaceCropResult.Face[rects.Length]
            };

            for(int i = 0; i < rects.Length; i++)
            {
                // Increase the size of the image with 25%.
                rects[i].Inflate((int)(rects[i].Width * 0.25), (int)(rects[i].Height * 0.25));

                FaceCropResult.Face face = new FaceCropResult.Face();
                face.Id = i.ToString();
                face.ImageBase64 = Convert.ToBase64String(capture.image.SubMat(rects[i]).ToBytes(".jpg"));

                faces.Faces[i] = face;
            }

            return faces;
        }

        private RecognitionResult RecognizeEmotions(FaceCropResult faces)
        {
            RecognitionResult result = new RecognitionResult();
            result.CaptureTime = faces.CaptureTime;
            result.Tag = faces.Tag;

            try
            {
                result.Faces = _emotionApi.DetectEmotions(faces.Faces).GetAwaiter().GetResult();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return result;
        }

        private async void SendResult(object result)
        {
            string json = JsonConvert.SerializeObject(result);

            await SendMessageAsync(json);
        }
    }
}
