using Microsoft.Extensions.Configuration;

namespace MapiExplorer.UI
{
    public class AppSettings : IAppSettings
    {
        public string ApplicationId { get; internal set; }

        public AppSettings(IConfiguration configuration)
        {
            ApplicationId = configuration["AzureAd:ClientId"];
        }
    }
}
