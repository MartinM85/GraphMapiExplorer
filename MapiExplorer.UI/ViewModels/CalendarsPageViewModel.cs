using MapiExplorer.Models;
using MapiExplorer.Services;
using MapiExplorer.UI.Extensions;
using System.Windows.Input;

namespace MapiExplorer.UI.ViewModels
{
    public class CalendarsPageViewModel : ViewModelBase
    {
        private readonly ICalendarsService _calendarsService;

        public ObservableCollectionEx<CalendarDto> Calendars { get; set; }

        public ICommand InspectMapiCommand { get; set; }

        public CalendarsPageViewModel(ICalendarsService calendarsService)
        {
            _calendarsService = calendarsService;
            InspectMapiCommand = new Command(InspectMapiProperties);

            Calendars = new ObservableCollectionEx<CalendarDto>();
        }

        protected override async Task LoadDataAsync()
        {
            var calendars = await _calendarsService.GetCalendarsAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Calendars.ClearAndAddRange(calendars.Select(x => new CalendarDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Color = $"{x.Color}",
                    HexColor = x.HexColor,
                    IsDefaultCalendar = x.IsDefaultCalendar.GetValueOrDefault()
                }));
            });
        }

        private async void InspectMapiProperties(object value)
        {
            if (value is string entityId && !string.IsNullOrEmpty(entityId))
            {
                var parameters = new Dictionary<string, object>();
                parameters[QueryAttributes.EntityId] = entityId;
                parameters[QueryAttributes.GraphResource] = GraphResource.Calendars;
                await Shell.Current.GoToAsync(Routes.MapiDetails, parameters);
            }
        }
    }
}
