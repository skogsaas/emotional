using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emotional.Services
{
    public class VideoService
    {
        private VideoCapture _video;

        private CancellationTokenSource _cancel;
        private Task _worker;

        private TimeSpan _interval;

        public Action<Mat> OnCapture { get; set; }

        public VideoService()
        {
            _interval = TimeSpan.FromMilliseconds(1000);
        }

        public void Start()
        {
            _video = new VideoCapture(0); // Capture from default (0) video device.

            _cancel = new CancellationTokenSource();
            _worker = Task.Run(() => process(), _cancel.Token);
        }

        public void Stop()
        {
            _cancel.Cancel();
        }

        private async void process()
        {
            using (Mat image = new Mat())
            {
                while (!_cancel.IsCancellationRequested)
                {
                    DateTime start = DateTime.Now;

                    _video.Read(image);

                    if (image.Empty())
                    {
                        break;
                    }

                    OnCapture?.Invoke(image);

                    TimeSpan duration = DateTime.Now - start;
                    TimeSpan delay = _interval - duration;

                    Console.WriteLine(delay);

                    await Task.Delay(delay);
                }
            }
        }
    }
}
