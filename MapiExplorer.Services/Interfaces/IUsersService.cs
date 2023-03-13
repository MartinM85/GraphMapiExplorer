using Microsoft.Graph.Communications.GetPresencesByUserId;
using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    public interface IUsersService
    {
        Task CreateExtension(string userId);
        Task<User> GetMyProfileAsync();
        Task<User> GetUserManagerAsync(string userId);
        Task<Stream> GetUserPhotoAsync(string userId);
        Task<Presence> GetUserPresenceAsync(string userId);
        Task<List<User>> GetUsersAsync(string filter, string select);
        Task<GetPresencesByUserIdResponse> GetUsersPresenceAsync(List<string> usersIds);
        Task SaveGitHubAccountsAsync(string userId, string work, string personal, string schemaExtension);
    }
}