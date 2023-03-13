using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    public interface ICalendarsService
    {
        Task<bool> SetSingleValueExtendedPropertyValueAsync(string calendarId, string propertyId, string propertyValue);
        Task<List<Calendar>> GetCalendarsAsync();
        Task<SingleValueLegacyExtendedProperty> GetSingleValueExtendedProperties(string messageId, string propertyId);
        Task<MultiValueLegacyExtendedProperty> GetMultiValueExtendedProperties(string calendarId, string propertyId);
        Task<bool> SetMultiValueExtendedPropertyValueAsync(string calendarId, string propertyId, List<string> propertyValues);
    }
}