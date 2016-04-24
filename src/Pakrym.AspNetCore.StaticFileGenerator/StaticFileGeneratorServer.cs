using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MusicStore
{
    public class StaticFileGeneratorServer : IServer
    {
        private readonly ILogger _logger;
        private readonly IOptions<StaticFileGeneratorOptions> _options;
        private readonly IApplicationLifetime _lifetime;
        private readonly Queue<StaticFileGeneratorPage> _pages;
        private readonly HashSet<string> _processed;

        public StaticFileGeneratorServer(IOptions<StaticFileGeneratorOptions> options, IApplicationLifetime lifetime, ILoggerFactory loggerFactory)
        {
            _options = options;
            _lifetime = lifetime;
            _logger = loggerFactory.CreateLogger(typeof(StaticFileGeneratorServer));
            _pages = new Queue<StaticFileGeneratorPage>();
            _processed = new HashSet<string>();
        }

        public IFeatureCollection Features { get; } = new FeatureCollection();

        public void Dispose()
        {
        }

        public void AddPage(StaticFileGeneratorPage page)
        {
            if (_processed.Add(page.Url))
            {
                _logger.LogInformation("Adding url \"{url}\" to file \"{file}\"", page.Url, page.Path);
                _pages.Enqueue(page);
            }
        }

        public void Start<TContext>(IHttpApplication<TContext> application)
        {
            foreach (var page in _options.Value.RootPages)
            {
                AddPage(page);
            }
            Task.Run(() => ProcessPages(application));
        }

        private async Task ProcessPages<TContext>(IHttpApplication<TContext> application)
        {
            while (_pages.Any())
            {
                var page = _pages.Dequeue();
                try
                {
                    var requestFeatures = new FeatureCollection(Features);
                    var httpRequest = CreateRequest<TContext>(page);
                    var responseStream = new MemoryStream();
                    var httpResponse = new StaticFilesGeneratorHttpResponseFeature()
                    {
                        Body = responseStream
                    };

                    requestFeatures.Set(httpRequest);
                    requestFeatures.Set((IHttpResponseFeature)httpResponse);

                    _logger.LogInformation("Processing {page}", page);
                    var context = application.CreateContext(requestFeatures);
                    try
                    {
                        await application.ProcessRequestAsync(context);
                        await httpResponse.StartingAsync();
                        await httpResponse.CompleteAsync();
                        application.DisposeContext(context, null);

                        SaveFile(page, responseStream);
                    }
                    catch (Exception ex)
                    {
                        application.DisposeContext(context, ex);
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception while processing url {url}: {ex}", page.Url, ex.ToString());
                }
            }
            _lifetime.StopApplication();
        }

        private static IHttpRequestFeature CreateRequest<TContext>(StaticFileGeneratorPage page)
        {
            string path = page.Url;
            var i = path.IndexOf('?');
            string query = null;
            if (i > 0)
            {
                query = path.Substring(i, path.Length - i);
                path = path.Substring(0, i);
            }
            IHttpRequestFeature httpRequest = new HttpRequestFeature()
            {
                Protocol = "http",
                Scheme = "http",
                Method = "GET",
                Path = path,
                QueryString = query,
                Body = new MemoryStream(),
            };
            return httpRequest;
        }

        private void SaveFile(StaticFileGeneratorPage page, MemoryStream memoryStream)
        {
            var filePath = page.Path.TrimStart('/');
            var fullPath = Path.Combine(_options.Value.OutputPath, filePath);
            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllBytes(fullPath, memoryStream.ToArray());
        }

        public class StaticFilesGeneratorHttpResponseFeature : HttpResponseFeature
        {
            List<Tuple<Func<object, Task>, object>> _onCompletedCallbacks = new List<Tuple<Func<object, Task>, object>>();
            List<Tuple<Func<object, Task>, object>> _onStartingCallbacks = new List<Tuple<Func<object, Task>, object>>();

            public override void OnCompleted(Func<object, Task> callback, object state)
            {
                _onCompletedCallbacks.Add(new Tuple<Func<object, Task>, object>(callback, state));
            }

            public async Task CompleteAsync()
            {
                var callbacks = _onCompletedCallbacks;
                _onCompletedCallbacks = null;
                foreach (var callback in callbacks)
                {
                    await callback.Item1(callback.Item2);
                }
            }

            public override void OnStarting(Func<object, Task> callback, object state)
            {
                _onStartingCallbacks.Add(new Tuple<Func<object, Task>, object>(callback, state));
            }

            public async Task StartingAsync()
            {
                var callbacks = _onStartingCallbacks;
                _onStartingCallbacks = null;
                foreach (var callback in callbacks)
                {
                    await callback.Item1(callback.Item2);
                }
            }
        }
    }
}