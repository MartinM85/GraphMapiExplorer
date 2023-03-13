using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Me.MailFolders.Item;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace MapiExplorer.Services
{
    public class MailFoldersService : IMailFoldersService
    {
        private readonly GraphServiceClient _client;
        private readonly ILogger _logger;
        public MailFoldersService(GraphServiceClient client, ILogger<MailFoldersService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<MailFolder>> GetMailFoldersAsync()
        {
            try
            {
                var mailFolderResponse = await _client.Me.MailFolders.GetAsync(x =>
                {
                    x.QueryParameters.Select = new string[] { "id", "displayName", "childFolderCount", "totalItemCount", "unreadItemCount" };
                    x.QueryParameters.Top = 20;
                });
                return mailFolderResponse.Value;
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load mail folders.");
                return new List<MailFolder>();
            }
        }

        public async Task<SingleValueLegacyExtendedProperty> GetSingleValueExtendedProperties(string mailFolderId, string propertyId)
        {
            var request = _client.Me.MailFolders[mailFolderId].ToGetRequestInformation(x =>
            {
                x.QueryParameters.Select = new[] { "id" };
            });
            request.QueryParameters["%24expand"] = new string[] { $"singleValueExtendedProperties($filter=id eq '{propertyId}')" };
            var index = request.UrlTemplate.Length;
            request.UrlTemplate = request.UrlTemplate.Insert(index - 1, ",%24expand");
            try
            {
                var result = await new MailFolderItemRequestBuilder(request.URI.OriginalString, _client.RequestAdapter).GetAsync();
                return result.SingleValueExtendedProperties?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, $"Failed to read value of extended property '{propertyId}'.");
                return null;
            }
        }

        public async Task<bool> SetSingleValueExtendedPropertyValueAsync(string mailFolderId, string propertyId, string propertyValue)
        {
            var body = new MailFolder
            {
                SingleValueExtendedProperties = new List<SingleValueLegacyExtendedProperty>
                {
                    new SingleValueLegacyExtendedProperty
                    {
                        Id = propertyId,
                        Value = propertyValue
                    }
                }
            };
            try
            {
                await _client.Me.MailFolders[mailFolderId].PatchAsync(body);
                return true;
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to set value of single extended property.");
                return false;
            }
        }

        public async Task<MultiValueLegacyExtendedProperty> GetMultiValueExtendedProperties(string mailFolderId, string propertyId)
        {
            var request = _client.Me.MailFolders[mailFolderId].ToGetRequestInformation(x =>
            {
                x.QueryParameters.Select = new[] { "id" };
            });
            request.QueryParameters["%24expand"] = new string[] { $"multiValueExtendedProperties($filter=id eq '{propertyId}')" };
            var index = request.UrlTemplate.Length;
            request.UrlTemplate = request.UrlTemplate.Insert(index - 1, ",%24expand");
            try
            {
                var result = await new MailFolderItemRequestBuilder(request.URI.OriginalString, _client.RequestAdapter).GetAsync();
                return result.MultiValueExtendedProperties?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, $"Failed to read value of extended property '{propertyId}'.");
                return null;
            }
        }

        public async Task<bool> SetMultiValueExtendedPropertyValueAsync(string mailFolderId, string propertyId, List<string> propertyValues)
        {
            var body = new MailFolder
            {
                MultiValueExtendedProperties = new List<MultiValueLegacyExtendedProperty>
                {
                    new MultiValueLegacyExtendedProperty
                    {
                        Id = propertyId,
                        Value = propertyValues
                    }
                }
            };
            try
            {
                await _client.Me.MailFolders[mailFolderId].PatchAsync(body);
                return true;
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to set value of multi value extended property.");
                return false;
            }
        }
    }
}
