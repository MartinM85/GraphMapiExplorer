using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    public interface IContactFoldersService
    {
        Task<bool> SetSingleValueExtendedPropertyValueAsync(string contactFolderId, string propertyId, string propertyValue);
        Task<List<ContactFolder>> GetContactFoldersAsync();
        Task<SingleValueLegacyExtendedProperty> GetSingleValueExtendedProperties(string messageId, string propertyId);
        Task<MultiValueLegacyExtendedProperty> GetMultiValueExtendedProperties(string contactFolderId, string propertyId);
        Task<bool> SetMultiValueExtendedPropertyValueAsync(string contactFolderId, string propertyId, List<string> propertyValues);
    }
}