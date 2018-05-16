namespace Emotional.Models
{
    public class StartCameraRequest : BaseFrame
    {
        public const string Typename = "startcamera";

        public StartCameraRequest()
        {
            Type = Typename.ToUpper();
        }
    }
}
