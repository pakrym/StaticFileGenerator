using System.Collections.Generic;

namespace MusicStore
{
    public class StaticFileGeneratorOptions
    {
        public string OutputPath { get; set; } = "_Generated";

        public IList<StaticFileGeneratorPage> RootPages { get; set; } = new List<StaticFileGeneratorPage>();
    }
}