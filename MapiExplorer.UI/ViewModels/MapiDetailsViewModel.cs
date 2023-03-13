using MapiExplorer.Models;
using MapiExplorer.Models.MAPI;
using MapiExplorer.Services;
using MapiExplorer.UI.Services;
using Microsoft.Graph.Models;
using System.Drawing.Text;
using System.Windows.Input;

namespace MapiExplorer.UI.ViewModels
{
    public class MapiDetailsViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly IMessagesService _messagesService;
        private readonly IEventsService _eventsService;
        private readonly ICalendarsService _calendarsService;
        private readonly IMailFoldersService _mailFoldersService;
        private readonly IContactsService _contactsService;
        private readonly IContactFoldersService _contactFoldersService;
        private readonly IPidPropertiesService _pidPropertiesService;
        private readonly IPidPropertiesSettings _pidPropertiesSettings;
        private readonly IAlertService _alertService;
        private readonly ITranslateIdsService _translateIdsService;

        private GraphResource _graphResource;
        private string _entityId;
        public string EntityId
        {
            get => _entityId;
            set
            {
                _entityId = value;
                OnPropertyChanged();
            }
        }

        private string _entryId;
        public string EntryId
        {
            get => _entryId;
            set
            {
                _entryId = value;
                OnPropertyChanged();
            }
        }

        private PidLidProperty _selectedPidLidProperty;
        public PidLidProperty SelectedPidLidProperty
        {
            get => _selectedPidLidProperty;
            set
            {
                _selectedPidLidProperty = value;
                GetPidLidDetail();
            }
        }

        private List<PidLidProperty> _pidLidSourceProperties;
        public List<PidLidProperty> PidLidSourceProperties
        {
            get => _pidLidSourceProperties;
            set
            {
                _pidLidSourceProperties = value;
                OnPropertyChanged();
            }
        }

        private PidTagProperty _selectedPidTagProperty;
        public PidTagProperty SelectedPidTagProperty
        {
            get => _selectedPidTagProperty;
            set
            {
                _selectedPidTagProperty = value;
                GetPidTagDetail();
            }
        }

        private List<PidTagProperty> _pidTagSourceProperties;
        public List<PidTagProperty> PidTagSourceProperties
        {
            get => _pidTagSourceProperties;
            set
            {
                _pidTagSourceProperties = value;
                OnPropertyChanged();
            }
        }

        private PidNameProperty _selectedPidNameProperty;
        public PidNameProperty SelectedPidNameProperty
        {
            get => _selectedPidNameProperty;
            set
            {
                _selectedPidNameProperty = value;
                GetPidNameDetail();
            }
        }

        private List<PidNameProperty> _pidNameSourceProperties;
        public List<PidNameProperty> PidNameSourceProperties
        {
            get => _pidNameSourceProperties;
            set
            {
                _pidNameSourceProperties = value;
                OnPropertyChanged();
            }
        }

        private string _selectedPidNamePropertyValue;
        public string SelectedPidNamePropertyValue
        {
            get => _selectedPidNamePropertyValue;
            set
            {
                _selectedPidNamePropertyValue = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackCommand { get; set; }

        public ICommand UpdateNamedPropertyValueCommand { get; set; }

        public ICommand CancelUpdateNamedPropertyValueCommand { get; set; }

        public ICommand CopyToClipboardCommand { get; set; }

        public MapiDetailsViewModel(IMessagesService messagesService, IEventsService eventsService, ICalendarsService calendarsService, 
            IMailFoldersService mailFoldersService, IContactsService contactsService, IContactFoldersService contactFoldersService,
            IPidPropertiesService pidPropertiesService, IPidPropertiesSettings pidPropertiesSettings, IAlertService alertService,
            ITranslateIdsService translateIdsService)
        {
            _messagesService = messagesService;
            _eventsService = eventsService;
            _calendarsService = calendarsService;
            _mailFoldersService = mailFoldersService;
            _contactsService = contactsService;
            _contactFoldersService = contactFoldersService;
            _pidPropertiesService = pidPropertiesService;
            _pidPropertiesSettings = pidPropertiesSettings;
            _alertService = alertService;
            _translateIdsService = translateIdsService;

            BackCommand = new Command(GoBackAsync);
            UpdateNamedPropertyValueCommand = new Command(UpdateNamePropertyValueAsync);
            CancelUpdateNamedPropertyValueCommand = new Command(CancelUpdateNamedPropertyValue);
            CopyToClipboardCommand = new Command(CopyToClipboard);            
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue(QueryAttributes.EntityId, out var id) && id is string entityId && !string.IsNullOrEmpty(entityId))
            {
                if (query.TryGetValue(QueryAttributes.GraphResource, out var resource) && resource is GraphResource graphResource)
                {
                    EntityId = entityId;
                    _graphResource = graphResource;
                    LoadData();
                }
            }
        }

        protected override async Task LoadDataAsync()
        {
            using var customPropStream = await FileSystem.Current.OpenAppPackageFileAsync(_pidPropertiesSettings.CustomNamedPropertiesResourceFileName);
            await _pidPropertiesService.LoadCustomNamedPropertiesAsync(customPropStream);
            using var pidPropStream = await FileSystem.Current.OpenAppPackageFileAsync(_pidPropertiesSettings.PidPropertiesResourceFileName);
            await _pidPropertiesService.LoadPidPropertiesMappingAsync(pidPropStream);

            _pidTagSourceProperties = _pidPropertiesService.GetPidTagProperties(_graphResource);
            OnPropertyChanged(nameof(PidTagSourceProperties));

            _pidLidSourceProperties = _pidPropertiesService.GetPidLidProperties(_graphResource);
            OnPropertyChanged(nameof(PidLidSourceProperties));

            _pidNameSourceProperties = _pidPropertiesService.GetPidNamedProperties(_graphResource);
            _pidNameSourceProperties.AddRange(_pidPropertiesService.GetCustomNamedProperties(_graphResource));
            OnPropertyChanged(nameof(PidNameSourceProperties));

            var convertIdRes = await _translateIdsService.TranslateRestIdToEntryIdAsync(EntityId);
            EntryId = convertIdRes?.TargetId;
        }

        private async void GetPidTagDetail()
        {
            if (SelectedPidTagProperty != null)
            {
                SelectedPidTagProperty.GraphValue = await GetExtendedPropertyValue(_graphResource, EntityId, SelectedPidTagProperty.GraphId, SelectedPidTagProperty.Type);
                OnPropertyChanged(nameof(SelectedPidTagProperty));
            }
        }

        private async void GetPidNameDetail()
        {
            if(SelectedPidNameProperty != null)
            {
                SelectedPidNameProperty.GraphValue = await GetExtendedPropertyValue(_graphResource, EntityId, SelectedPidNameProperty.GraphId, SelectedPidNameProperty.Type);
                SelectedPidNamePropertyValue = SelectedPidNameProperty.GraphValue;
                OnPropertyChanged(nameof(SelectedPidNameProperty));
            }
        }

        private async void GetPidLidDetail()
        {
            if (SelectedPidLidProperty != null)
            {
                SelectedPidLidProperty.GraphValue = await GetExtendedPropertyValue(_graphResource, EntityId, SelectedPidLidProperty.GraphId, SelectedPidLidProperty.Type);
                OnPropertyChanged(nameof(SelectedPidLidProperty));
            }
        }

        private async Task<string> GetExtendedPropertyValue(GraphResource graphResource, string entityId, string graphId, string type)
        {
            SingleValueLegacyExtendedProperty singleValueProperty = null;
            MultiValueLegacyExtendedProperty multiValueProperty = null;
            var isMultiValue = type.Contains("Array");
            switch (graphResource)
            {
                case GraphResource.Messages:
                    if (isMultiValue)
                    {
                        multiValueProperty = await _messagesService.GetMultiValueExtendedProperties(entityId, graphId);
                    }
                    else
                    {
                        singleValueProperty = await _messagesService.GetSingleValueExtendedProperties(entityId, graphId);
                    }
                    break;
                case GraphResource.Events:
                    if (isMultiValue)
                    {
                        multiValueProperty = await _eventsService.GetMultiValueExtendedProperties(entityId, graphId);
                    }
                    else
                    {
                        singleValueProperty = await _eventsService.GetSingleValueExtendedProperties(entityId, graphId);
                    }
                    break;
                case GraphResource.Calendars:
                    if (isMultiValue)
                    {
                        multiValueProperty = await _calendarsService.GetMultiValueExtendedProperties(entityId, graphId);
                    }
                    else
                    {
                        singleValueProperty = await _calendarsService.GetSingleValueExtendedProperties(entityId, graphId);
                    }
                    break;
                case GraphResource.Contacts:
                    if (isMultiValue)
                    {
                        multiValueProperty = await _contactsService.GetMultiValueExtendedProperties(entityId, graphId);
                    }
                    else
                    {
                        singleValueProperty = await _contactsService.GetSingleValueExtendedProperties(entityId, graphId);
                    }
                    break;
                case GraphResource.ContactFolders:
                    if (isMultiValue)
                    {
                        multiValueProperty = await _contactsService.GetMultiValueExtendedProperties(entityId, graphId);
                    }
                    else
                    {
                        singleValueProperty = await _contactFoldersService.GetSingleValueExtendedProperties(entityId, graphId);
                    }
                    break;
                case GraphResource.MailFolders:
                    if (isMultiValue)
                    {
                        multiValueProperty = await _mailFoldersService.GetMultiValueExtendedProperties(entityId, graphId);
                    }
                    else
                    {
                        singleValueProperty = await _mailFoldersService.GetSingleValueExtendedProperties(entityId, graphId);
                    }
                    break;
            }
            if (isMultiValue)
            {
                return multiValueProperty != null ? string.Join(";", multiValueProperty.Value) : null;
            }
            else
            {
                return singleValueProperty?.Value;
            }
        }

        private async void GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void UpdateNamePropertyValueAsync()
        {
            var isMultiValue = SelectedPidNameProperty.Type.Contains("Array");
            var updated = false;
            switch (_graphResource)
            {
                case GraphResource.Messages:
                    if (isMultiValue)
                    {
                        updated = await _messagesService.SetMultiValueExtendedPropertyValueAsync(EntityId, SelectedPidNameProperty.GraphId, GetPropertyValues());
                    }
                    else
                    {
                        updated = await _messagesService.SetSingleValueExtendedPropertyValueAsync(EntityId, SelectedPidNameProperty.GraphId, SelectedPidNamePropertyValue);
                    }
                    break;
                case GraphResource.Events:
                    if (isMultiValue)
                    {
                        updated = await _eventsService.SetMultiValueExtendedPropertyValueAsync(EntityId, SelectedPidNameProperty.GraphId, GetPropertyValues());
                    }
                    else
                    {
                        updated = await _eventsService.SetSingleValueExtendedPropertyValueAsync(EntityId, SelectedPidNameProperty.GraphId, SelectedPidNamePropertyValue);
                    }
                    break;
                case GraphResource.Calendars:
                    if (isMultiValue)
                    {
                        updated = await _calendarsService.SetMultiValueExtendedPropertyValueAsync(EntityId, SelectedPidNameProperty.GraphId, GetPropertyValues());
                    }
                    else
                    {
                        updated = await _calendarsService.SetSingleValueExtendedPropertyValueAsync(EntityId, SelectedPidNameProperty.GraphId, SelectedPidNamePropertyValue);
                    }
                    break;
                case GraphResource.Contacts:
                    if (isMultiValue)
                    {
                        updated = await _contactsService.SetMultiValueExtendedPropertyValueAsync(EntityId, SelectedPidNameProperty.GraphId, GetPropertyValues());
                    }
                    else
                    {
                        updated = await _contactsService.SetSingleValueExtendedPropertyValueAsync(EntityId, SelectedPidNameProperty.GraphId, SelectedPidNamePropertyValue);
                    }
                    break;
                case GraphResource.ContactFolders:
                    if (isMultiValue)
                    {
                        updated = await _contactFoldersService.SetMultiValueExtendedPropertyValueAsync(EntityId, SelectedPidNameProperty.GraphId, GetPropertyValues());
                    }
                    else
                    {
                        updated = await _contactFoldersService.SetSingleValueExtendedPropertyValueAsync(EntityId, SelectedPidNameProperty.GraphId, SelectedPidNamePropertyValue);
                    }
                    break;
                case GraphResource.MailFolders:
                    if (isMultiValue)
                    {
                        updated = await _mailFoldersService.SetMultiValueExtendedPropertyValueAsync(EntityId, SelectedPidNameProperty.GraphId, GetPropertyValues());
                    }
                    else
                    {
                        updated = await _mailFoldersService.SetSingleValueExtendedPropertyValueAsync(EntityId, SelectedPidNameProperty.GraphId, SelectedPidNamePropertyValue);
                    }
                    break;
            }
            if (updated)
            {
                SelectedPidNameProperty.GraphValue = SelectedPidNamePropertyValue;
            }
            else
            {
                _alertService.ShowAlert("Error", "An attempt to update value of extended property failed. Check logs for details.");
            }

            List<string> GetPropertyValues()
            {
                return SelectedPidNamePropertyValue.Split(';').ToList();
            }
        }

        private void CancelUpdateNamedPropertyValue()
        {
            SelectedPidNamePropertyValue = SelectedPidNameProperty.GraphValue;
        }

        private async void CopyToClipboard(object data)
        {
            await Clipboard.SetTextAsync(data as string);
        }
    }
}
