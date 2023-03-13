using MapiExplorer.Models;
using MapiExplorer.Services;
using System.Windows.Input;

namespace MapiExplorer.UI.ViewModels
{
    public class SchemaExtensionDetailsViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly ISchemaExtensionsService _schemaExtensionsService;

        private string _entityId;

        public ICommand BackCommand { get; set; }

        private SchemaExtensionDto _schemaExtension;
        public SchemaExtensionDto SchemaExtension
        {
            get => _schemaExtension;
            set
            {
                _schemaExtension = value;
                OnPropertyChanged();
            }
        }

        public SchemaExtensionDetailsViewModel(ISchemaExtensionsService schemaExtensionsService)
        {
            _schemaExtensionsService = schemaExtensionsService;
            BackCommand = new Command(GoBackAsync);
        }

        protected override async Task LoadDataAsync()
        {
            var result = await _schemaExtensionsService.GetSchemaExtensionAsync(_entityId);
            if (result != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SchemaExtension = new SchemaExtensionDto
                    {
                        Id = result.Id,
                        Description = result.Description,
                        TargetTypes = result.TargetTypes,
                        Properties = result.Properties.Select(x =>
                                                new SchemaExtensionPropertyDto
                                                {
                                                    Name = x.Name,
                                                    Type = x.Type
                                                }).ToList()
                    };
                });
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue(QueryAttributes.EntityId, out var id) && id is string entityId && !string.IsNullOrEmpty(entityId))
            {
                _entityId = entityId;
                LoadData();
            }
        }

        private async void GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
