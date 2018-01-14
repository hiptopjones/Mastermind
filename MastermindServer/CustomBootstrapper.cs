using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Diagnostics;
using Nancy.TinyIoc;

namespace MastermindServer
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = @"secret" }; }
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<GameService>().AsSingleton();
        }
    }
}
