namespace Blogifier.Shared.Configurations
{
    /// <summary>
    /// Common settings shared throughout application.
    /// </summary>
    public class BlogifierConfiguration
    {

        public string PathBase { get; set; }
        public string DbProvider { get; set; }
        public string ConnString { get; set; }
        public string Salt { get; set; }
        public string DemoMode { get; set; }
        public string FileExtensions { get; set; }

    }
}
