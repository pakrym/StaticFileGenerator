namespace MusicStore
{
    public class StaticFileGeneratorPage
    {
        public StaticFileGeneratorPage(string url, string path)
        {
            Url = url;
            Path = path;
        }

        public string Url { get; set; }
        public string Path { get; set; }
    }
}