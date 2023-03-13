using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Me.Contacts.Item;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace MapiExplorer.Services
{
    public class ContactsService : IContactsService
    {
        private readonly GraphServiceClient _client;
        private readonly ILogger _logger;
        public ContactsService(GraphServiceClient client, ILogger<ContactsService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<Contact>> GetContactAsync()
        {
            try
            {
                var response = await _client.Me.Contacts.GetAsync(x =>
                {
                    x.QueryParameters.Select = new string[] { "id", "displayName", "emailAddresses", "companyName", "mobilePhone" };
                    x.QueryParameters.Top = 50;
                });
                return response.Value;
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load contacts.");
                return new List<Contact>();
            }
        }

        public async Task<SingleValueLegacyExtendedProperty> GetSingleValueExtendedProperties(string contactId, string propertyId)
        {
            var request = _client.Me.Contacts[contactId].ToGetRequestInformation(x =>
            {
                x.QueryParameters.Select = new[] { "id" };
            });
            request.QueryParameters["%24expand"] = new string[] { $"singleValueExtendedProperties($filter=id eq '{propertyId}')" };
            var index = request.UrlTemplate.Length;
            request.UrlTemplate = request.UrlTemplate.Insert(index - 1, ",%24expand");
            try
            {
                var result = await new ContactItemRequestBuilder(request.URI.OriginalString, _client.RequestAdapter).GetAsync();
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

        public async Task<bool> SetSingleValueExtendedPropertyValueAsync(string contactId, string propertyId, string propertyValue)
        {
            var body = new Contact
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
                await _client.Me.Contacts[contactId].PatchAsync(body);
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

        public async Task<MultiValueLegacyExtendedProperty> GetMultiValueExtendedProperties(string contactId, string propertyId)
        {
            var request = _client.Me.Contacts[contactId].ToGetRequestInformation(x =>
            {
                x.QueryParameters.Select = new[] { "id" };
            });
            request.QueryParameters["%24expand"] = new string[] { $"multiValueExtendedProperties($filter=id eq '{propertyId}')" };
            var index = request.UrlTemplate.Length;
            request.UrlTemplate = request.UrlTemplate.Insert(index - 1, ",%24expand");
            try
            {
                var result = await new ContactItemRequestBuilder(request.URI.OriginalString, _client.RequestAdapter).GetAsync();
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

        public async Task<bool> SetMultiValueExtendedPropertyValueAsync(string contactId, string propertyId, List<string> propertyValues)
        {
            var body = new Contact
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
                await _client.Me.Contacts[contactId].PatchAsync(body);
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
