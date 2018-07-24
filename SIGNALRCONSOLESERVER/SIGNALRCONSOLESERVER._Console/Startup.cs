using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(SIGNALRCONSOLESERVER._Console.Startup))]

namespace SIGNALRCONSOLESERVER._Console
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", map =>
            {
                var hubConfiguration = new HubConfiguration
                {
                    EnableDetailedErrors = true,
                };
                map.UseCors(CorsOptions.AllowAll);
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}
