namespace Blogifier.Shared.Configurations
{
    public class HostingConfiguration
    {
    /// <summary>
    ///The base path for hosting this application. Not setting this implies root "/", so you do not need to set this when hosting as a subdomain (nginx server with location / or Windows IIS Spplication).
    /// If you are serving this site as an application, i.e it will be served from a subdomain (iis application or nginx server) then you do not need to set this.
    /// If on other hand this will be served from a non-root path, e.g mywebsite.domain/thisapplication then set this to the base path, i.e /thisapplication. In nginx you would configure hosting using location /thisapplication or iis virtual directory.
    /// </summary>
        public string PathBase { get; set; }

    }
}
