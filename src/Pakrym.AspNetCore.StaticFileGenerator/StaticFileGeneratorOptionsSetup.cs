using Microsoft.Extensions.Options;

namespace MusicStore
{
    public class StaticFileGeneratorOptionsSetup : IConfigureOptions<StaticFileGeneratorOptions>
    {
        public void Configure(StaticFileGeneratorOptions options)
        {
        }
    }
}