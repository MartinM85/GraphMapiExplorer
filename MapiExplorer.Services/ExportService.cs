using MapiExplorer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using System.Text;
using System.Text.Json;

namespace MapiExplorer.Services
{
    public class ExportService : IExportService
    {
        private readonly IUsersService _usersService;
        private readonly IFilesService _filesService;
        private readonly ISitesService _sitesService;
        private readonly ILogger _logger;

        public ExportService(IUsersService usersService, IFilesService filesService, ISitesService sitesService, ILogger<ExportService> logger)
        {
            _usersService = usersService;
            _filesService = filesService;
            _sitesService = sitesService;
            _logger = logger;
        }

        public async Task<DriveItem> ExportGitHubAccountsToCsvAsync(string filter, string schemaExtension)
        {
            var rootSite = await _sitesService.FindRootSiteAsync();
            if (rootSite != null)
            {
                // get users based on the filter
                var users = await _usersService.GetUsersAsync(filter, schemaExtension);

                var sb = new StringBuilder()
                    .AppendLine("Id,Name,Email,GitHub work account,GitHub personal account");
                if (users != null || users.Count > 0)
                {
                    foreach (var user in users)
                    {
                        string workAccount = null;
                        string personalAccount = null;
                        if (!string.IsNullOrEmpty(schemaExtension) && user.AdditionalData.TryGetValue(schemaExtension, out var property))
                        {
                            if (property is JsonElement jsonElement)
                            {
                                workAccount = jsonElement.GetProperty(GitHubExtensionProperty.WorkAccount).GetString();
                                personalAccount = jsonElement.GetProperty(GitHubExtensionProperty.PersonalAccount).GetString();
                            }
                        }
                        sb.AppendLine($"{user.Id},{user.DisplayName},{user.Mail},{workAccount},{personalAccount}");
                    }
                }
                else
                {
                    _logger.LogWarning("No users returned.");
                }
                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                using var memoryStream = new MemoryStream(bytes);

                var fileName = $"GitHubAccounts_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
                return await _filesService.UploadFileAsync(rootSite.Id, fileName, memoryStream);
            }
            return null;
        }
    }
}