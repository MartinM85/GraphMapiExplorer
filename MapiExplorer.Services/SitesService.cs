using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace MapiExplorer.Services
{
    public class SitesService : ISitesService
    {
        private readonly GraphServiceClient _client;
        private readonly ILogger _logger;

        public SitesService(GraphServiceClient client, ILogger<SitesService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<Site> FindRootSiteAsync()
        {
            try
            {
                return await _client.Sites["root"].GetAsync();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load root site.");
                return null;
            }
        }
    }
}