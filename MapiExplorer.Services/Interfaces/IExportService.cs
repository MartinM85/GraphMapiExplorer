using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    public interface IExportService
    {
        Task<DriveItem> ExportGitHubAccountsToCsvAsync(string filter, string schemaExtension);
    }
}