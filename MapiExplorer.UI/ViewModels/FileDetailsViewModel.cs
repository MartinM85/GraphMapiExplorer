using MapiExplorer.Services;

namespace MapiExplorer.UI.ViewModels
{
    public class FileDetailsViewModel : ViewModelBase, IQueryAttributable
    {
        private IFilesService _filesService;

        private string _driveItemId;
        private string _driveId;

        public FileDetailsViewModel(IFilesService filesService)
        {
            _filesService = filesService;
        }

        private string _previewUrl;
        public string PreviewUrl
        {
            get => _previewUrl;
            set
            {
                _previewUrl = value;
                OnPropertyChanged();
            }
        }

        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
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
            var previewInfo = await _filesService.GetFilePreviewAsync(_driveId, _driveItemId);
            var driveItem = await _filesService.GetDriveItemAsync(_driveId, _driveItemId);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PreviewUrl = previewInfo?.GetUrl;
                FileName = driveItem?.Name;
            });
        }
    }
}
