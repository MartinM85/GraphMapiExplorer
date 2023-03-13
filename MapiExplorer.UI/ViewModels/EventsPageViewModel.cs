using MapiExplorer.Models;
using MapiExplorer.Services;
using MapiExplorer.UI.Extensions;
using Microsoft.Graph.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MapiExplorer.UI.ViewModels
{
    public class EventsPageViewModel : ViewModelBase
    {
        private readonly IEventsService _eventsService;
        private readonly ICalendarsService _calendarsService;

        public ReadOnlyCollection<string> TimeZones { get; internal set; }

        private string _selectedTimeZone;
        public string SelectedTimeZone
        {
            get => _selectedTimeZone;
            set
            {
                _selectedTimeZone = value;
                if (!string.IsNullOrEmpty(_selectedTimeZone))
                {
                    CalculateCurrentWeek();
                    LoadData();
                }
            }
        }

        public ObservableCollectionEx<EventDto> Events { get; set; }

        public ICommand InspectMapiCommand { get; set; }

        public ICommand SetCurrentWeekCommand { get; set; }
        public ICommand SetNextWeekCommand { get; set; }
        public ICommand SetPreviousWeekCommand { get; set; }

        private List<MeetingDuration> _meetingDurations;
        public List<MeetingDuration> MeetingDurations
        {
            get => _meetingDurations;
            set
            {
                _meetingDurations = value;
                OnPropertyChanged();
            }
        }

        private MeetingDuration _selectedMeetingDuration;
        public MeetingDuration SelectedMeetingDuration
        {
            get => _selectedMeetingDuration;
            set
            {
                _selectedMeetingDuration = value;
                LoadData();
            }
        }

        private List<CalendarDto> _calendars;
        public List<CalendarDto> Calendars
        {
            get => _calendars;
            set
            {
                _calendars = value;
                OnPropertyChanged();
            }
        }

        private CalendarDto _selectedCalendar;
        public CalendarDto SelectedCalendar
        {
            get => _selectedCalendar;
            set
            {
                _selectedCalendar = value;
                LoadData();
            }
        }

        public EventsPageViewModel(IEventsService eventsService, ICalendarsService calendarsService)
        {
            _eventsService = eventsService;
            _calendarsService = calendarsService;
            InspectMapiCommand = new Command(InspectMapiProperties);
            SetCurrentWeekCommand = new Command(SetCurrentWeek);
            SetNextWeekCommand = new Command(SetNextWeek);
            SetPreviousWeekCommand = new Command(SetPreviousWeek);
            TimeZones = new ReadOnlyCollection<string>(TimeZoneInfo.GetSystemTimeZones().Select(x => x.Id).ToList());
            SelectedTimeZone = TimeZoneInfo.Local.Id;

            Events = new ObservableCollectionEx<EventDto>();
        }

        protected override async Task LoadDataAsync()
        {
            if (_calendars == null)
            {
                var calendars = await _calendarsService.GetCalendarsAsync();
                _calendars = calendars.Select(x => new CalendarDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsDefaultCalendar = x.IsDefaultCalendar.GetValueOrDefault(),
                    Color = $"{x.Color}"
                }).ToList();
                _selectedCalendar = Calendars.FirstOrDefault(x => x.IsDefaultCalendar);
                OnPropertyChanged(nameof(Calendars));
                OnPropertyChanged(nameof(SelectedCalendar));
            }
            if (_meetingDurations == null)
            {
                var defaultDuration = new MeetingDuration
                {
                    DisplayName = "No filter",
                    DurationMinutes = 0,
                    FilterOperator = string.Empty
                };
                _meetingDurations = new List<MeetingDuration>
                {
                    defaultDuration,
                    new MeetingDuration
                    {
                        DisplayName = "Up to 10 minutes",
                        DurationMinutes = 10,
                        FilterOperator = "le"
                    },
                    new MeetingDuration 
                    {
                        DisplayName = "Up to 30 minutes",
                        DurationMinutes = 30,
                        FilterOperator = "le"
                    },
                    new MeetingDuration
                    {
                        DisplayName = "Over 30 minutes",
                        DurationMinutes = 30,
                        FilterOperator = "gt"
                    }
                };
                _selectedMeetingDuration = defaultDuration;
                OnPropertyChanged(nameof(MeetingDurations));
            }
            if (SelectedCalendar != null)
            {
                var startUtc = TimeZoneInfo.ConvertTimeToUtc(StartDateTime, TimeZoneInfo.FindSystemTimeZoneById(SelectedTimeZone));
                var endUtc = TimeZoneInfo.ConvertTimeToUtc(EndDateTime, TimeZoneInfo.FindSystemTimeZoneById(SelectedTimeZone));
                var events = await _eventsService.GetEventsAsync(SelectedCalendar.Id, SelectedMeetingDuration, SelectedTimeZone, startUtc, endUtc);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Events.ClearAndAddRange(events.Select(x => new EventDto
                    {
                        Id = x.Id,
                        Subject = x.Subject,
                        Start = DateTimeTimeZoneExtensions.ToDateTime(x.Start),
                        End = DateTimeTimeZoneExtensions.ToDateTime(x.End),
                        Location = x.Location?.DisplayName,
                        Organizer = x.Organizer.EmailAddress.Name,
                        BodyPreview = x.BodyPreview
                    }));
                });
            }
        }

        private async void InspectMapiProperties(object value)
        {
            if (value is string entityId && !string.IsNullOrEmpty(entityId))
            {
                var parameters = new Dictionary<string, object>();
                parameters[QueryAttributes.EntityId] = entityId;
                parameters[QueryAttributes.GraphResource] = GraphResource.Events;
                await Shell.Current.GoToAsync(Routes.MapiDetails, parameters);
            }
        }

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
                OnPropertyChanged();
            }
        }

        private void CalculateCurrentWeek()
        {
            var date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(SelectedTimeZone));
            StartDateTime = date.AddDays(-GetOffset(date.DayOfWeek));
            EndDateTime = StartDateTime.AddDays(6);

            int GetOffset(DayOfWeek dayOfWeek)
            {
                return dayOfWeek switch
                {
                    DayOfWeek.Monday => 0,
                    DayOfWeek.Tuesday => 1,
                    DayOfWeek.Wednesday => 2,
                    DayOfWeek.Thursday => 3,
                    DayOfWeek.Friday => 4,
                    DayOfWeek.Saturday => 5,
                    DayOfWeek.Sunday => 6,
                    _ => 6
                };
            }
        }

        private void SetCurrentWeek()
        {
            CalculateCurrentWeek();
            LoadData();
        }

        private void SetNextWeek()
        {
            StartDateTime = StartDateTime.AddDays(7);
            EndDateTime = StartDateTime.AddDays(6);
            LoadData();
        }

        private void SetPreviousWeek()
        {
            StartDateTime = StartDateTime.AddDays(-7);
            EndDateTime = StartDateTime.AddDays(6);
            LoadData();
        }
    }
}
