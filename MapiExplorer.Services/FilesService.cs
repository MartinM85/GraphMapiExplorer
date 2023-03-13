using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Drives.Item.Items.Item.Preview;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Kiota.Http.HttpClientLibrary.Middleware.Options;

namespace MapiExplorer.Services
{
    public class FilesService : IFilesService
    {
        private readonly GraphServiceClient _client;
        private readonly ILogger _logger;

        public FilesService(GraphServiceClient client, ILogger<FilesService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<DriveItem>> GetMyItemsAsync()
        {
            try
            {
                var drive = await _client.Me.Drive.GetAsync(x=>
                {
                    x.QueryParameters.Select = new string[] { "id" };
                });
                var rootItem = await _client.Drives[drive.Id].Root.GetAsync(x =>
                {
                    x.QueryParameters.Select = new string[] { "id" };
                });
                return await GetItemsAsync(drive.Id, rootItem.Id);
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to read user items.");
                return new List<DriveItem>();
            }
        }

        public async Task<List<DriveItem>> GetItemsAsync(string driveId, string driveItemId)
        {
            try
            {
                var items = await _client.Drives[driveId].Items[driveItemId].Children.GetAsync();
                return items.Value.ToList();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to drive items.");
                return new List<DriveItem>();
            }
        }

        public async Task<List<DriveItem>> GetSiteItemsAsync(string siteId)
        {
            try
            {
                var siteDrive = await _client.Sites[siteId].Drive.GetAsync();
                var siteDriveRootItem = await _client.Drives[siteDrive.Id].Root.GetAsync();
                return await GetItemsAsync(siteDrive.Id, siteDriveRootItem.Id);
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to read user items.");
                return new List<DriveItem>();
            }
        }

        public async Task<DriveItem> UploadFileAsync(string siteId, string path, Stream file)
        {
            try
            {
                var drive = await _client.Sites[siteId].Drive.GetAsync();
                // it should return new driveItem but unfortunately is not supported by SDK
                await _client.Drives[drive.Id].Root.ItemWithPath(path).Content.PutAsync(file);
                // make extra call to get the driveItem
                var driveItem = await _client.Drives[drive.Id].Root.ItemWithPath(path).GetAsync();

                return driveItem;
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to upload file to SharePoint.");
                return null;
            }
        }

        public async Task<ItemPreviewInfo> GetFilePreviewAsync(string driveId, string driveItemId)
        {
            var body = new PreviewPostRequestBody
            {
            };
            try
            {
                return await _client.Drives[driveId].Items[driveItemId].Preview.PostAsync(body);
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load item preview info.");
                return null;
            }
        }

        public async Task<DriveItem> GetDriveItemAsync(string driveId, string driveItemId)
        {
            try
            {
                return await _client.Drives[driveId].Items[driveItemId].GetAsync();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load drive item.");
                return null;
            }
        }
    }
}