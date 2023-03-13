using MapiExplorer.Models;
using MapiExplorer.Services;
using MapiExplorer.UI.Extensions;
using System.Windows.Input;

namespace MapiExplorer.UI.ViewModels
{
    public class ContactsPageViewModel : ViewModelBase
    {
        private readonly IContactsService _contactsService;

        public ObservableCollectionEx<ContactDto> Contacts { get; set; }

        public ICommand InspectMapiCommand { get; set; }

        public ContactsPageViewModel(IContactsService eventsService)
        {
            _contactsService = eventsService;
            InspectMapiCommand = new Command(InspectMapiProperties);

            Contacts = new ObservableCollectionEx<ContactDto>();
        }

        protected override async Task LoadDataAsync()
        {
            var contacts = await _contactsService.GetContactAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Contacts.ClearAndAddRange(contacts.Select(x => new ContactDto
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    CompanyName = x.CompanyName,
                    MobilePhone = x.MobilePhone,
                    EmailAddresses = x.EmailAddresses.Select(x => x.Address).ToList()
                }));
            });
        }

        private async void InspectMapiProperties(object value)
        {
            if (value is string entityId && !string.IsNullOrEmpty(entityId))
            {
                var parameters = new Dictionary<string, object>();
                parameters[QueryAttributes.EntityId] = entityId;
                parameters[QueryAttributes.GraphResource] = GraphResource.Contacts;
                await Shell.Current.GoToAsync(Routes.MapiDetails, parameters);
            }
        }
    }
}
