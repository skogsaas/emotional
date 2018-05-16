using System.Threading.Tasks;

namespace Emotional.Middlewares.WebSocketHandler
{
    public interface IWebSocketConnection
    {
        Task Listen();
    }
}