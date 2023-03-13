using MapiExplorer.Models;
using MapiExplorer.Services;
using MapiExplorer.UI.Extensions;
using System.Windows.Input;

namespace MapiExplorer.UI.ViewModels
{
    public class MailFoldersPageViewModel : ViewModelBase
    {
        private readonly IMailFoldersService _mailFoldersService;

        public ObservableCollectionEx<MailFolderDto> MailFolders { get; set; }

        public ICommand InspectMapiCommand { get; set; }

        public MailFoldersPageViewModel(IMailFoldersService mailFoldersService)
        {
            _mailFoldersService = mailFoldersService;
            InspectMapiCommand = new Command(InspectMapiProperties);

            MailFolders = new ObservableCollectionEx<MailFolderDto>();
        }

        protected override async Task LoadDataAsync()
        {
            var folders = await _mailFoldersService.GetMailFoldersAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                MailFolders.ClearAndAddRange(folders.Select(x => new MailFolderDto
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    ChildFolderCount = x.ChildFolderCount.GetValueOrDefault(),
                    TotalItemCount = x.TotalItemCount.GetValueOrDefault(),
                    UnreadItemCount = x.UnreadItemCount.GetValueOrDefault()
                }));
            });
        }

        private async void InspectMapiProperties(object value)
        {
            if (value is string entityId && !string.IsNullOrEmpty(entityId))
            {
                var parameters = new Dictionary<string, object>();
                parameters[QueryAttributes.EntityId] = entityId;
                parameters[QueryAttributes.GraphResource] = GraphResource.MailFolders;
                await Shell.Current.GoToAsync(Routes.MapiDetails, parameters);
            }
        }
    }
}
