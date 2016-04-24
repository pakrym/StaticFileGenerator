using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;

namespace MusicStore
{
    internal class StaticFileGeneratorUrlHelperFactory : IUrlHelperFactory
    {
        private readonly IServer _server;
        private readonly IOptions<StaticFileGeneratorOptions> _options;

        public StaticFileGeneratorUrlHelperFactory(IServer server, IOptions<StaticFileGeneratorOptions> options)
        {
            _server = server;
            _options = options;
        }

        public IUrlHelper GetUrlHelper(ActionContext context)
        {
            return new StaticFileGeneratorUrlHelper(_server, _options.Value, context);
        }
    }
}