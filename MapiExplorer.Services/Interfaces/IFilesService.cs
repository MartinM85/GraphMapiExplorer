using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    public interface IFilesService
    {
        Task<DriveItem> UploadFileAsync(string siteId, string path, Stream file);

        Task<List<DriveItem>> GetSiteItemsAsync(string siteId);

        Task<List<DriveItem>> GetMyItemsAsync();
        Task<List<DriveItem>> GetItemsAsync(string driveId, string driveItemId);
        Task<ItemPreviewInfo> GetFilePreviewAsync(string driveId, string driveItemId);
        Task<DriveItem> GetDriveItemAsync(string driveId, string driveItemId);
    }
}