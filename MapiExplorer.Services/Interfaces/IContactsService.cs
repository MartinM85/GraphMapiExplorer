using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    public interface IContactsService
    {
        Task<bool> SetSingleValueExtendedPropertyValueAsync(string contactId, string propertyId, string propertyValue);
        Task<List<Contact>> GetContactAsync();
        Task<SingleValueLegacyExtendedProperty> GetSingleValueExtendedProperties(string messageId, string propertyId);
        Task<MultiValueLegacyExtendedProperty> GetMultiValueExtendedProperties(string contactId, string propertyId);
        Task<bool> SetMultiValueExtendedPropertyValueAsync(string contactId, string propertyId, List<string> propertyValues);
    }
}