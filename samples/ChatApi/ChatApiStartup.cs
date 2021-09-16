using ChatApi.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using SignalR.Extensions.Orleans.Internals;

namespace ChatApi
{
    public class ChatApiStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IUserIdProvider, UserIdProvider>()
                .AddSignalR()
                .AddOrleans()
                ;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/node", async context =>
                {
                    var node = context.RequestServices.GetRequiredService<ISignalRNode>();
                    await context.Response.WriteAsync(node.ToString());
                });

                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }

    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.Features.Get<IHttpContextFeature>().HttpContext.Request.Headers["x-user"];
        }
    }
}
