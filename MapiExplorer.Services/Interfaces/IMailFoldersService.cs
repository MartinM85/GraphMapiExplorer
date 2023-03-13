using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    public interface IMailFoldersService
    {
        Task<List<MailFolder>> GetMailFoldersAsync();
        Task<MultiValueLegacyExtendedProperty> GetMultiValueExtendedProperties(string mailFolderId, string propertyId);
        Task<SingleValueLegacyExtendedProperty> GetSingleValueExtendedProperties(string messageId, string propertyId);
        Task<bool> SetMultiValueExtendedPropertyValueAsync(string mailFolderId, string propertyId, List<string> propertyValues);
        Task<bool> SetSingleValueExtendedPropertyValueAsync(string mailFolderId, string propertyId, string propertyValue);
    }
}