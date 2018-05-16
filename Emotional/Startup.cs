using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Emotional.Middlewares.WebSocketHandler;
using Emotional.Services;
using Emotional.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Emotional
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Add(new ServiceDescriptor(typeof(VideoService), typeof(VideoService), ServiceLifetime.Singleton));
            services.Add(new ServiceDescriptor(typeof(ICloudFaceApi), typeof(GoogleVisionApiService), ServiceLifetime.Transient));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseBrowserLink();
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseMvc();
                
            app.UseWebSockets(new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(10) });
            app.MapWebSocketHandler("/ws", (WebSocket socket) => { return new FaceHandler(socket, serviceProvider.GetService<ICloudFaceApi>(), serviceProvider.GetService<VideoService>()); });
        }
    }
}
