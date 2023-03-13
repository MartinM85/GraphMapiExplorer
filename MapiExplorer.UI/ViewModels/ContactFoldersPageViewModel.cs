using MapiExplorer.Models;
using MapiExplorer.Services;
using MapiExplorer.UI.Extensions;
using System.Windows.Input;

namespace MapiExplorer.UI.ViewModels
{
    public class ContactFoldersPageViewModel : ViewModelBase
    {
        private readonly IContactFoldersService _contactFoldersService;

        public ObservableCollectionEx<ContactFolderDto> ContactFolders { get; set; }

        public ICommand InspectMapiCommand { get; set; }

        public ContactFoldersPageViewModel(IContactFoldersService contactFoldersService)
        {
            _contactFoldersService = contactFoldersService;
            InspectMapiCommand = new Command(InspectMapiProperties);

            ContactFolders = new ObservableCollectionEx<ContactFolderDto>();
        }

        protected override async Task LoadDataAsync()
        {
            var folders = await _contactFoldersService.GetContactFoldersAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ContactFolders.ClearAndAddRange(folders.Select(x => new ContactFolderDto
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName
                }));
            });
        }

        private async void InspectMapiProperties(object value)
        {
            if (value is string entityId && !string.IsNullOrEmpty(entityId))
            {
                var parameters = new Dictionary<string, object>();
                parameters[QueryAttributes.EntityId] = entityId;
                parameters[QueryAttributes.GraphResource] = GraphResource.ContactFolders;
                await Shell.Current.GoToAsync(Routes.MapiDetails, parameters);
            }
        }
    }
}
