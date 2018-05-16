using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emotional.Middlewares.WebSocketHandler
{
    public abstract class WebSocketConnection : IWebSocketConnection
    {
        private WebSocket _webSocket;
        private byte[] _buffer;

        private SemaphoreSlim _lock;

        public WebSocketConnection(WebSocket webSocket, int bufferSize = 4096)
        {
            _webSocket = webSocket;
            _buffer = new byte[bufferSize];

            _lock = new SemaphoreSlim(1, 1);
        }

        public abstract Task ReceiveMessageAsync(string message);

        public virtual async Task SendMessageAsync(string message)
        {
            await _lock.WaitAsync();

            try
            {
                if (_webSocket.State != WebSocketState.Open)
                {
                    return;
                }

                var arr = Encoding.UTF8.GetBytes(message);
                var buffer = new ArraySegment<byte>(arr, 0, arr.Length);

                await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch(Exception)
            {

            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task Listen()
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(_buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(_buffer, 0, result.Count);

                    await ReceiveMessageAsync(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by WebSocketConnection", CancellationToken.None);
                }
            }
        }
    }
}
