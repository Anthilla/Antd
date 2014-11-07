using Nancy;
using Nancy.Conventions;

namespace Antd
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions conv)
        {
            base.ConfigureConventions(conv);

            conv.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("Scripts", @"/Scripts/")
            );
        }
    }
}