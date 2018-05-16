using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Emotional.Middlewares.WebSocketHandler
{
    /// <summary> 
    /// This should the last middleware in the pipeline when use websocket 
    /// </summary> 
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        Func<WebSocket, IWebSocketConnection> _factory;

        public WebSocketMiddleware(RequestDelegate next, Func<WebSocket, IWebSocketConnection> factory)
        {
            _next = next;
            _factory = factory;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var connection = _factory.Invoke(webSocket);

                if (connection != null)
                {
                    await connection.Listen();
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            }
        }
    }
}