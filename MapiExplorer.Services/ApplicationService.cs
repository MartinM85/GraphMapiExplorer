using MapiExplorer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace MapiExplorer.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly GraphServiceClient _client;
        private readonly ILogger _logger;

        public ApplicationService(GraphServiceClient client, ILogger<ApplicationService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<ExtensionProperty> GetExtensionAsync(string appId, string name)
        {
            var extensionName = ConstructExtensionName(appId, name);
            try
            {
                var app = await GetApplicationAsync(appId);

                var result = await _client.Applications[app.Id].ExtensionProperties.GetAsync(x =>
                {
                    x.QueryParameters.Filter = $"name eq '{extensionName}'";
                });
                return result.Value.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, $"Failed to load extension property '{extensionName}' for app '{appId}'");
                return null;
            }
        }

        public async Task<ExtensionProperty> CreateExtensionAsync(string appId, string name, List<string> targetObjects, GraphPropertyTypes dataType)
        {
            var extensionProperty = new ExtensionProperty
            {
                DataType = $"{dataType}",
                Name = name,
                TargetObjects = targetObjects
            };
            try
            {
                var app = await GetApplicationAsync(appId);
                return await _client.Applications[app.Id].ExtensionProperties.PostAsync(extensionProperty);
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, $"Failed to create extension property '{name}' for app '{appId}'.");
                return null;
            }
        }

        public async Task AddExtensionPropertyValue(string appId, string name, string extensionValue)
        {
            var extensionName = ConstructExtensionName(appId, name);
            var appUpdate = new Application
            {
                AdditionalData = new Dictionary<string, object>
                {
                    { extensionName, extensionValue }
                }
            };
            try
            {
                var app = await GetApplicationAsync(appId);
                await _client.Applications[app.Id].PatchAsync(appUpdate);
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, $"Failed to set value '{extensionValue}' of extension property '{extensionName}' for app '{appId}'.");
            }
        }

        public async Task<T> GetExtensionPropertyValueAsync<T>(string appId, string name)
        {
            var extensionName = ConstructExtensionName(appId, name);
            try
            {
                var app = await GetApplicationAsync(appId);
                var result = await _client.Applications[app.Id].GetAsync(x =>
                {
                    x.QueryParameters.Select = new[] { "id", extensionName };
                });
                if (result.AdditionalData.TryGetValue(extensionName, out var value))
                {
                    return (T)value;
                }
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, $"Failed to get value of extension property '{extensionName}' for app '{appId}'.");
            }
            return default;
        }

        public async Task<Application> GetApplicationAsync(string appId)
        {
            try
            {
                var result = await _client.Applications.GetAsync(x =>
                {
                    x.QueryParameters.Filter = $"appId eq '{appId}'";
                });
                var app = result.Value.FirstOrDefault();
                if (app == null)
                {
                    throw new Exception($"Application with id '{appId}' not found.");
                }
                return app;
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, $"Failed to load application '{appId}'.");
                return null;
            }
        }

        private string ConstructExtensionName(string appId, string name)
        {
            // construct the correct graph api extension name
            var id = Guid.Parse(appId).ToString("N");
            return $"extension_{id}_{name}";
        }
    }
}