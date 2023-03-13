using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Communications.GetPresencesByUserId;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace MapiExplorer.Services
{
    public class UsersService : IUsersService
    {
        private readonly GraphServiceClient _client;
        private readonly ILogger _logger;

        public UsersService(GraphServiceClient client, ILogger<UsersService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<User>> GetUsersAsync(string filter, string select)
        {
            try
            {
                var result = await _client.Users.GetAsync(x =>
                {
                    var selectProperties = new List<string> { "id", "displayName", "mail" };
                    if (!string.IsNullOrEmpty(select))
                    {
                        selectProperties.Add(select);
                    }
                    x.QueryParameters.Select = selectProperties.ToArray();
                    if (!string.IsNullOrEmpty(filter))
                    {
                        x.QueryParameters.Filter = $"startsWith(displayName,'{filter}')";
                        x.QueryParameters.Count = true;
                        x.Headers["ConsistencyLevel"] = new[] { "eventual" };
                    }
                    // return only 50 users, use filter to get specific subset of users
                    x.QueryParameters.Top = 50;
                });

                return result.Value;
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load users.");
                return new List<User>();
            }
        }

        public async Task CreateExtension(string userId)
        {
            var extension = new Extension()
            {
                AdditionalData = new Dictionary<string, object> {
                 {
                     "",""
                 } }
            };
            await _client.Users[userId].Extensions.PostAsync(extension);
        }

        public async Task<User> GetMyProfileAsync()
        {
            try
            {
                return await _client.Me.GetAsync();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load user profile.");
                return null;
            }
        }

        public async Task<Stream> GetUserPhotoAsync(string userId)
        {
            try
            {
                return await _client.Users[userId].Photo.Content.GetAsync();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load user photo.");
                return null;
            }
        }

        public async Task<Presence> GetUserPresenceAsync(string userId)
        {
            try
            {
                return await _client.Users[userId].Presence.GetAsync();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load user presence.");
                return null;
            }
        }

        public async Task<User> GetUserManagerAsync(string userId)
        {
            try
            {
                return await _client.Users[userId].GetAsync(x =>
                {
                    x.QueryParameters.Select = new[] { "id" };
                    x.QueryParameters.Expand = new[] { "manager($levels=max;$select=id,displayName)" };
                    x.Headers["ConsistencyLevel"] = new[] { "eventual" };
                });
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load user manager.");
                return null;
            }
        }

        public async Task<GetPresencesByUserIdResponse> GetUsersPresenceAsync(List<string> usersIds)
        {
            try
            {
                var body = new GetPresencesByUserIdPostRequestBody
                {
                    Ids = usersIds
                };
                return await _client.Communications.GetPresencesByUserId.PostAsync(body);
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load users presence.");
                return null;
            }
        }

        public async Task SaveGitHubAccountsAsync(string userId, string work, string personal, string schemaExtension)
        {
            var body = new User
            {
                AdditionalData = new Dictionary<string, object>
                {
                    {
                        schemaExtension,
                        new
                        {
                            GitHubWorkAccount = work,
                            GitHubPersonalAccount = personal
                        }
                    }
                }
            };
            try
            {
                await _client.Users[userId].PatchAsync(body);
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to save schema extension.");
            }
        }
    }
}
