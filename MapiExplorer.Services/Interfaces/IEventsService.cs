using MapiExplorer.Models;
using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    public interface IEventsService
    {
        Task<bool> SetSingleValueExtendedPropertyValueAsync(string eventId, string propertyId, string propertyValue);
        Task<List<Event>> GetEventsAsync(string calendarId, MeetingDuration durationFilter, string timezone, DateTime startDateTimeUtc, DateTime endDateTimeUtc);
        Task<SingleValueLegacyExtendedProperty> GetSingleValueExtendedProperties(string eventId, string propertyId);
        Task<MultiValueLegacyExtendedProperty> GetMultiValueExtendedProperties(string eventId, string propertyId);
        Task<bool> SetMultiValueExtendedPropertyValueAsync(string eventId, string propertyId, List<string> propertyValues);
    }
}