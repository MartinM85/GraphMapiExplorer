using MapiExplorer.Services;
using Microsoft.IdentityModel.Logging;

namespace MapiExplorer.UI.ViewModels
{
    public class FolderDetailsViewModel : DriveItemsViewModel, IQueryAttributable
    {
        private string _driveItemId;
        private string _driveId;

        public string Name { get; set; }

        public FolderDetailsViewModel(IFilesService filesService) : base(filesService)
        {
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue(QueryAttributes.EntityId, out var id) && id is string entityId && !string.IsNullOrEmpty(entityId))
            {
                if (query.TryGetValue(QueryAttributes.DriveId, out var resource) && resource is string driveId && !string.IsNullOrEmpty(driveId))
                {
                    _driveItemId = entityId;
                    _driveId = driveId;
                    LoadData();
                }
            }
        }

        protected override async Task LoadDataAsync()
        {
            var item = await FilesService.GetDriveItemAsync(_driveId, _driveItemId);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Name = item?.Name;
                OnPropertyChanged(nameof(Name));
            });
            var items = await FilesService.GetItemsAsync(_driveId, _driveItemId);
            SetDriveItems(items);
        }
    }
}
