using Azure.Core;
using MapiExplorer.Models;
using MapiExplorer.Models.MAPI;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Groups.Item.Calendar.Events.Item;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace MapiExplorer.Services
{
    public class EventsService : IEventsService
    {
        private readonly GraphServiceClient _client;
        private readonly ILogger _logger;
        public EventsService(GraphServiceClient client, ILogger<EventsService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<Event>> GetEventsAsync(string calendarId, MeetingDuration durationFilter, string timezone, DateTime startDateTimeUtc, DateTime endDateTimeUtc)
        {
            try
            {
                var response = await _client.Me.Calendars[calendarId].CalendarView.GetAsync(x =>
                {
                    x.QueryParameters.Select = new[] { "id", "subject", "organizer", "start", "end", "location", "bodyPreview" };
                    if (durationFilter != null && !string.IsNullOrEmpty(durationFilter.FilterOperator))
                    {
                        var pidLid = PidPropertyFactory.Create(PidLidProperties.PidLidAppointmentDuration);
                        x.QueryParameters.Filter = $"singleValueExtendedProperties/Any(ep: ep/id eq '{pidLid.GraphId}' and cast(ep/value, Edm.Int32) {durationFilter.FilterOperator} {durationFilter.DurationMinutes})";
                    }
                    x.QueryParameters.StartDateTime = startDateTimeUtc.ToString("yyyy-MM-ddThh:mm:ss");
                    x.QueryParameters.EndDateTime = endDateTimeUtc.ToString("yyyy-MM-ddThh:mm:ss");
                    if (!string.IsNullOrEmpty(timezone))
                    {
                        x.Headers[HttpHeader.Names.Prefer] = new List<string> { $"outlook.timezone=\"{timezone}\"" };
                    }
                });
                return response.Value.ToList();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load events.");
                return new List<Event>();
            }
        }

        public async Task<SingleValueLegacyExtendedProperty> GetSingleValueExtendedProperties(string eventId, string propertyId)
        {
            var request = _client.Me.Events[eventId].ToGetRequestInformation(x =>
            {
                x.QueryParameters.Select = new[] { "id" };
            });
            request.QueryParameters["%24expand"] = new string[] { $"singleValueExtendedProperties($filter=id eq '{propertyId}')" };
            var index = request.UrlTemplate.Length;
            request.UrlTemplate = request.UrlTemplate.Insert(index - 1, ",%24expand");
            try
            {
                var result = await new EventItemRequestBuilder(request.URI.OriginalString, _client.RequestAdapter).GetAsync();
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

        public async Task<bool> SetSingleValueExtendedPropertyValueAsync(string eventId, string propertyId, string propertyValue)
        {
            var body = new Event
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
                await _client.Me.Events[eventId].PatchAsync(body);
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

        public async Task<MultiValueLegacyExtendedProperty> GetMultiValueExtendedProperties(string eventId, string propertyId)
        {
            var request = _client.Me.Events[eventId].ToGetRequestInformation(x =>
            {
                x.QueryParameters.Select = new[] { "id" };
            });
            request.QueryParameters["%24expand"] = new string[] { $"multiValueExtendedProperties($filter=id eq '{propertyId}')" };
            var index = request.UrlTemplate.Length;
            request.UrlTemplate = request.UrlTemplate.Insert(index - 1, ",%24expand");
            try
            {
                var result = await new EventItemRequestBuilder(request.URI.OriginalString, _client.RequestAdapter).GetAsync();
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

        public async Task<bool> SetMultiValueExtendedPropertyValueAsync(string eventId, string propertyId, List<string> propertyValues)
        {
            var body = new Event
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
                await _client.Me.Events[eventId].PatchAsync(body);
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
