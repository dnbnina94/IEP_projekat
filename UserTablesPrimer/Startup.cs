using FluentScheduler;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UserTablesPrimer.Startup))]
namespace UserTablesPrimer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
            JobManager.Initialize(new Tasks.Timer());
        }
    }
}
