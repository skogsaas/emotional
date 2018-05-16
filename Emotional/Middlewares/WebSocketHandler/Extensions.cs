using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;

namespace Emotional.Middlewares.WebSocketHandler
{
    public static class Extensions
    {
        public static IApplicationBuilder MapWebSocketHandler(this IApplicationBuilder app, PathString path, Func<WebSocket, IWebSocketConnection> factory)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<WebSocketMiddleware>(factory));
        }
    }
}
