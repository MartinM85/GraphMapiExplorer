using MapiExplorer.Models;
using MapiExplorer.Services;
using MapiExplorer.UI.Extensions;
using Microsoft.Graph.Models;
using System.Windows.Input;

namespace MapiExplorer.UI.ViewModels
{
    public abstract class DriveItemsViewModel : ViewModelBase
    {
        protected IFilesService FilesService;

        private DriveItemDto _selectedDriveItem;
        public DriveItemDto SelectedDriveItem
        {
            get => _selectedDriveItem;
            set
            {
                _selectedDriveItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollectionEx<DriveItemDto> DriveItems { get; set; }

        public ICommand GoToDetailsCommand { get; set; }

        public DriveItemsViewModel(IFilesService filesService)
        {
            FilesService = filesService;

            GoToDetailsCommand = new Command(GoToDetailsAsync);

            DriveItems = new ObservableCollectionEx<DriveItemDto>();
        }

        protected override Task LoadDataAsync()
        {
            throw new NotImplementedException();
        }

        protected void SetDriveItems(List<DriveItem> items)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DriveItems.Clear();
                DriveItems = new ObservableCollectionEx<DriveItemDto>(items.Select(x => new DriveItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    DriveId = x.ParentReference.DriveId,
                    ParentDriveItemId = x.ParentReference.Id,
                    File = x.FileObject != null ? CreateFileDto(x.FileObject) : null,
                    Folder = x.Folder != null ? CreateFolderDto(x.Folder) : null
                }));
                OnPropertyChanged(nameof(DriveItems));
            });

            FileDto CreateFileDto(FileObject file)
            {
                return new FileDto
                {
                    MimeType = file.MimeType
                };
            }

            FolderDto CreateFolderDto(Folder folder)
            {
                return new FolderDto
                {
                    ChildCount = folder.ChildCount.GetValueOrDefault()
                };
            }
        }

        private async void GoToDetailsAsync()
        {
            if (SelectedDriveItem != null)
            {
                var parameters = new Dictionary<string, object>();
                parameters[QueryAttributes.EntityId] = SelectedDriveItem.Id;
                parameters[QueryAttributes.DriveId] = SelectedDriveItem.DriveId;
                var route = SelectedDriveItem.IsFile ? Routes.FileDetails : Routes.FolderDetails;
                await Shell.Current.GoToAsync(route, parameters);
            }
        }
    }
}
