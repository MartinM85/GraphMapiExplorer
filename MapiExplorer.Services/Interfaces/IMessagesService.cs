using MapiExplorer.Models;
using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    public interface IMessagesService
    {
        Task<bool> SetSingleValueExtendedPropertyValueAsync(string messageId, string propertyId, string propertyValue);
        Task<List<Message>> GetMessagesAsync(string subject, string senderMail, DateTime? start, DateTime? end);
        Task<SingleValueLegacyExtendedProperty> GetSingleValueExtendedProperties(string messageId, string propertyId);
        Task<bool> SendToMailAsync(SendMessageDto message);
        Task<MultiValueLegacyExtendedProperty> GetMultiValueExtendedProperties(string messageId, string propertyId);
        Task<bool> SetMultiValueExtendedPropertyValueAsync(string messageId, string propertyId, List<string> propertyValues);
    }
}