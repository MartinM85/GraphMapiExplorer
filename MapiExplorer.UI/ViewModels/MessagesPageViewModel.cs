using MapiExplorer.Models;
using MapiExplorer.Services;
using MapiExplorer.UI.Extensions;
using System.Windows.Input;

namespace MapiExplorer.UI.ViewModels
{
    public class MessagesPageViewModel : ViewModelBase
    {
        // one second before midnight
        private const int NumberOfSecondsUntilMidnight = 86399;
        private readonly IMessagesService _messagesService;

        public MessagesPageViewModel(IMessagesService messagesService)
        {
            _messagesService = messagesService;

            FilterCommand = new Command(LoadData);
            InspectMapiCommand = new Command(InspectMapiProperties);

            Messages = new ObservableCollectionEx<MessageDto>();

            EndDateTime = DateTime.Now.Date.AddSeconds(NumberOfSecondsUntilMidnight);
            StartDateTime = EndDateTime.AddDays(-7);
        }

        public ObservableCollectionEx<MessageDto> Messages { get; set; }

        private DateTime _startDateTime;
        public DateTime StartDateTime
        {
            get => _startDateTime;
            set
            {
                _startDateTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime _endDateTime;
        public DateTime EndDateTime
        {
            get => _endDateTime;
            set
            {
                _endDateTime = value;
                if (_endDateTime.Hour == 0 && _endDateTime.Minute == 0 && _endDateTime.Second == 0)
                {
                    // data picker set only date component
                    _endDateTime = _endDateTime.AddSeconds(NumberOfSecondsUntilMidnight);
                }
                OnPropertyChanged();
            }
        }

        private bool _allowFilterStart;
        public bool AllowFilterStart
        {
            get => _allowFilterStart; 
            set
            {
                _allowFilterStart = value;
                OnPropertyChanged();
            }
        }

        private bool _allowFilterEnd;
        public bool AllowFilterEnd
        {
            get => _allowFilterEnd;
            set
            {
                _allowFilterEnd = value;
                OnPropertyChanged();
            }
        }

        public string FilterSubject { get; set; }
        public string FilterSender { get; set; }

        public ICommand FilterCommand { get; set; }

        public ICommand InspectMapiCommand { get; set; }

        protected override async Task LoadDataAsync()
        {
            var messages = await _messagesService.GetMessagesAsync(FilterSubject, FilterSender,
                AllowFilterStart ? StartDateTime : null, AllowFilterEnd ? EndDateTime : null);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Messages.ClearAndAddRange(messages.Select(x => new MessageDto
                {
                    Id = x.Id,
                    Subject = x.Subject,
                    Sender = x.Sender.EmailAddress.Address,
                    BodyPreview = x.BodyPreview,
                    ReceivedDateTime = x.ReceivedDateTime.GetValueOrDefault()
                }));
            });
        }

        private async void InspectMapiProperties(object value)
        {
            if (value is string entityId && !string.IsNullOrEmpty(entityId))
            {
                var parameters = new Dictionary<string, object>();
                parameters[QueryAttributes.EntityId] = entityId;
                parameters[QueryAttributes.GraphResource] = GraphResource.Messages;
                await Shell.Current.GoToAsync(Routes.MapiDetails, parameters);
            }
        }
    }
}
