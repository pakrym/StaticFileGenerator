using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MusicStore
{
    internal class StaticFileGeneratorUrlHelper : UrlHelper
    {
        private readonly StaticFileGeneratorOptions _options;
        private readonly StaticFileGeneratorServer _server;

        public StaticFileGeneratorUrlHelper(IServer server, StaticFileGeneratorOptions options, ActionContext actionContext) : base(actionContext)
        {
            _options = options;
            _server = server as StaticFileGeneratorServer;
        }

        public override string Action(UrlActionContext actionContext)
        {
            var url = base.Action(actionContext);
            if (_server != null)
            {
                var file = _options.RootPages.FirstOrDefault(p => p.Url == url)?.Path ?? ToFileName(url);
                _server.AddPage(new StaticFileGeneratorPage(url, file));
                return file;
            }
            return url;
        }

        private static string ToFileName(string url)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var file = new string(
                url.ToCharArray().Select(
                    c =>
                    {
                        if (c == Path.AltDirectorySeparatorChar ||
                            c == Path.DirectorySeparatorChar)
                        {
                            return c;
                        }

                        if (invalid.Contains(c))
                        {
                            return '_';
                        }
                        return c;
                    }).ToArray()).TrimEnd('/');
            file += ".html";
            return file;
        }
    }
}