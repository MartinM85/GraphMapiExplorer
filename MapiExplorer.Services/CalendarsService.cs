using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Me.Calendars.Item;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace MapiExplorer.Services
{
    public class CalendarsService : ICalendarsService
    {
        private readonly GraphServiceClient _client;
        private readonly ILogger _logger;
        public CalendarsService(GraphServiceClient client, ILogger<CalendarsService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<Calendar>> GetCalendarsAsync()
        {
            try
            {
                var response = await _client.Me.Calendars.GetAsync(x =>
                {
                    x.QueryParameters.Select = new string[] { "id", "hexColor", "name", "color", "isDefaultCalendar" };
                    x.QueryParameters.Top = 20;
                });
                return response.Value;
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load calendars.");
                return new List<Calendar>();
            }
        }

        public async Task<SingleValueLegacyExtendedProperty> GetSingleValueExtendedProperties(string calendarId, string propertyId)
        {
            var request = _client.Me.Calendars[calendarId].ToGetRequestInformation(x =>
            {
                x.QueryParameters.Select = new[] { "id" };
            });
            request.QueryParameters["%24expand"] = new string[] { $"singleValueExtendedProperties($filter=id eq '{propertyId}')" };
            var index = request.UrlTemplate.Length;
            request.UrlTemplate = request.UrlTemplate.Insert(index - 1, ",%24expand");
            try
            {
                var result = await new CalendarItemRequestBuilder(request.URI.OriginalString, _client.RequestAdapter).GetAsync();
                return result.SingleValueExtendedProperties?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, $"Failed to read value of extended property {propertyId}.");
                return null;
            }
        }

        public async Task<bool> SetSingleValueExtendedPropertyValueAsync(string calendarId, string propertyId, string propertyValue)
        {
            var body = new Calendar
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
                await _client.Me.Calendars[calendarId].PatchAsync(body);
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

        public async Task<MultiValueLegacyExtendedProperty> GetMultiValueExtendedProperties(string calendarId, string propertyId)
        {
            var request = _client.Me.Calendars[calendarId].ToGetRequestInformation(x =>
            {
                x.QueryParameters.Select = new[] { "id" };
            });
            request.QueryParameters["%24expand"] = new string[] { $"multiValueExtendedProperties($filter=id eq '{propertyId}')" };
            var index = request.UrlTemplate.Length;
            request.UrlTemplate = request.UrlTemplate.Insert(index - 1, ",%24expand");
            try
            {
                var result = await new CalendarItemRequestBuilder(request.URI.OriginalString, _client.RequestAdapter).GetAsync();
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

        public async Task<bool> SetMultiValueExtendedPropertyValueAsync(string calendarId, string propertyId, List<string> propertyValues)
        {
            var body = new Calendar
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
                await _client.Me.Calendars[calendarId].PatchAsync(body);
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
