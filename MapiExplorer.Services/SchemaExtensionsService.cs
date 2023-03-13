using MapiExplorer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace MapiExplorer.Services
{
    public class SchemaExtensionsService : ISchemaExtensionsService
    {
        private readonly GraphServiceClient _client;
        private readonly ILogger _logger;

        public SchemaExtensionsService(GraphServiceClient client, ILogger<SchemaExtensionsService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<SchemaExtension>> GetExtensionsAsync(string filter)
        {
            try
            {
                var result = await _client.SchemaExtensions.GetAsync(x =>
                {
                    // it seems that select is ignored
                    x.QueryParameters.Select = new[] { "id", "description" };
                    if (!string.IsNullOrEmpty(filter))
                    {
                        x.QueryParameters.Filter = $"id eq '{filter}'";
                    }
                    // return only 100 items
                    x.QueryParameters.Top = 100;
                });
                return result.Value;
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load schema extensions.");
                return new List<SchemaExtension>();
            }
        }

        public async Task<SchemaExtension> CreateSchemaExtensionAsync(string appId, SchemaExtensionDto schema)
        {
            var schemaExtension = new SchemaExtension
            {
                Id = schema.Id,
                Description = schema.Description,
                TargetTypes = schema.TargetTypes,
                Owner = appId,
                Properties = schema.Properties.Select(x => new ExtensionSchemaProperty
                {
                    Name = x.Name,
                    Type = x.Type,
                }).ToList()
            };
            try
            {
                return await _client.SchemaExtensions.PostAsync(schemaExtension);
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to create new schema extension.");
                return null;
            }
        }

        public async Task MakeSchemaExtensionAvailableAsync(string schemaExtensionId)
        {
            var body = new SchemaExtension
            {
                Status = "Available"
            };
            try
            {
                await _client.SchemaExtensions[schemaExtensionId].PatchAsync(body);
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, $"Failed to update status of schema extension '{schemaExtensionId}'.");
            }
        }

        public async Task<SchemaExtension> GetSchemaExtensionAsync(string schemaExtensionId)
        {
            try
            {
                return await _client.SchemaExtensions[schemaExtensionId].GetAsync();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, $"Failed to get schema extension '{schemaExtensionId}'.");
                return null;
            }
        }
    }
}