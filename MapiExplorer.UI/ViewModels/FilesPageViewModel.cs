using MapiExplorer.Models;
using MapiExplorer.Services;
using Microsoft.Graph.Models;

namespace MapiExplorer.UI.ViewModels
{
    public class FilesPageViewModel : DriveItemsViewModel
    {
        private readonly ISitesService _sitesService;

        private List<DriveItemSource> _itemsSource;
        public List<DriveItemSource> ItemsSource
        {
            get => _itemsSource;
            set
            {
                _itemsSource = value;
                OnPropertyChanged();
            }
        }

        private DriveItemSource _selectedItemsSource;
        public DriveItemSource SelectedItemsSource
        {
            get => _selectedItemsSource;
            set
            {
                _selectedItemsSource = value;
                LoadData();
            }
        }

        public FilesPageViewModel(IFilesService filesService, ISitesService sitesService) : base(filesService)
        {
            _sitesService = sitesService;
            ItemsSource = new List<DriveItemSource>
            {
                new DriveItemSource { Id = Models.ItemsSource.OneDrive, Name = "User OneDrive" },
                new DriveItemSource { Id = Models.ItemsSource.SharePoint, Name = "Root SharePoint site" }
            };
            SelectedItemsSource = ItemsSource.First();
        }

        protected override async Task LoadDataAsync()
        {
            var items = new List<DriveItem>();
            if(SelectedItemsSource.Id == Models.ItemsSource.OneDrive)
            {
                items = await FilesService.GetMyItemsAsync();
            }
            else
            {
                var rootSite = await _sitesService.FindRootSiteAsync();
                if (rootSite != null)
                {
                    items = await FilesService.GetSiteItemsAsync(rootSite.Id);
                }
            }
            SetDriveItems(items);
        }
    }
}
