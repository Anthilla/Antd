
///-------------------------------------------------------------------------------------
/// Copyright (c) 2014 Anthilla S.r.l. (http://www.anthilla.com)
///
/// Licensed under the BSD licenses.
///
/// 141110
///-------------------------------------------------------------------------------------

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