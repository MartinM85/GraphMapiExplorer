using MapiExplorer.Models;
using MapiExplorer.Services;
using MapiExplorer.UI.Extensions;
using MapiExplorer.UI.Services;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace MapiExplorer.UI.ViewModels
{
    public class UsersPageViewModel : ViewModelBase
    {
        private readonly IUsersService _usersService;
        private readonly IExportService _exportService;
        private readonly IMessagesService _messagesService;
        private readonly IApplicationService _applicationService;
        private readonly IAlertService _alertService;
        private readonly IAppSettings _appSettings;

        public UsersPageViewModel(IUsersService usersService, IExportService exportService, IMessagesService messagesService,
            IApplicationService applicationService, IAlertService alertService, IAppSettings appSettings)
        {
            _usersService = usersService;
            _exportService = exportService;
            _messagesService = messagesService;
            _applicationService = applicationService;
            _alertService = alertService;
            _appSettings = appSettings;

            FilterCommand = new Command(LoadData);
            ExportToCsvCommand = new Command(ExportData);
            SendToMailCommand = new Command(SendDataToMail);
            EditGitHubAccountsCommand = new Command(EditUserAccounts);

            Users = new ObservableCollectionEx<UserDto>();
        }

        public ObservableCollectionEx<UserDto> Users { get; set; }

        private bool _exportingData;
        public bool ExportingData
        {
            get => _exportingData;
            set
            {
                _exportingData = value;
                OnPropertyChanged();
            }
        }

        public string Filter { get; set; }

        public ICommand FilterCommand { get; set; }

        public ICommand ExportToCsvCommand { get; set; }

        public ICommand SendToMailCommand { get; set; }

        public ICommand EditGitHubAccountsCommand { get; set; }

        private string _schemaExtensionName;

        protected override async Task LoadDataAsync()
        {
            if (string.IsNullOrEmpty(_schemaExtensionName))
            {
                _schemaExtensionName = await _applicationService.GetExtensionPropertyValueAsync<string>(_appSettings.ApplicationId, GitHubExtensionProperty.Id);
            }
            var data = await _usersService.GetUsersAsync(Filter, _schemaExtensionName);
            var userPresence = await _usersService.GetUsersPresenceAsync(data.Select(x => x.Id).ToList());
            var presence = userPresence?.Value.ToDictionary(x => x.Id, y => (y.Activity, y.Availability));
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Users.ClearAndAddRange(data.Select(x =>
                {
                    string workAccount = null;
                    string personalAccount = null;
                    if (!string.IsNullOrEmpty(_schemaExtensionName) && x.AdditionalData.TryGetValue(_schemaExtensionName, out var property))
                    {
                        if (property is JsonElement jsonElement)
                        {
                            workAccount = jsonElement.GetProperty(GitHubExtensionProperty.WorkAccount).GetString();
                            personalAccount = jsonElement.GetProperty(GitHubExtensionProperty.PersonalAccount).GetString();
                        }
                    }
                    return new UserDto
                    {
                        Id = x.Id,
                        DisplayName = x.DisplayName,
                        Email = x.Mail,
                        GitHubWorkAccount = workAccount,
                        GitHubPersonalAccount = personalAccount,
                        Activity = presence != null ? Enum.Parse<UserActivity>(presence[x.Id].Activity, true) : UserActivity.PresenceUnknown,
                        Availability = presence != null ? Enum.Parse<UserAvailability>(presence[x.Id].Availability, true) : UserAvailability.PresenceUnknown
                    };
                }));
            });

        }

        private Task _exportDataTask, _sendMailTask;
        private void ExportData()
        {
            if (_exportDataTask != null && _exportDataTask.IsCompleted)
            {
                _exportDataTask.Dispose();
                _exportDataTask = null;
            }
            _exportDataTask = ExportDataAsync();
        }

        private async Task ExportDataAsync()
        {
            ExportingData = true;
            var item = await _exportService.ExportGitHubAccountsToCsvAsync(Filter, _schemaExtensionName);
            if (item != null)
            {
                _alertService.ShowAlert("Export", $"Exported to file {item.Name}.");
            }
            else
            {
                _alertService.ShowAlert("Export", "Export failed. Check logs for details.");
            }
            ExportingData = false;
        }

        private void SendDataToMail()
        {
            if (_sendMailTask != null && _sendMailTask.IsCompleted)
            {
                _sendMailTask.Dispose();
                _sendMailTask = null;
            }
            _sendMailTask = SendDataToMailAsync();
        }

        private async Task SendDataToMailAsync()
        {
            ExportingData = true;
            var result = false;
            if (Users != null && Users.Count() > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("<h1>Report of GitHub account associated with users</h1>")
                    .AppendLine("<br/>")
                    .AppendLine("<table>")
                    .AppendLine("<tr>")
                    .AppendLine("<th>Id</th>")
                    .AppendLine("<th>Name</th>")
                    .AppendLine("<th>Email</th>")
                    .AppendLine("<th>GitHub work account</th>")
                    .AppendLine("<th>GitHub personal account</th>")
                    .AppendLine("</tr>");
                foreach (var user in Users)
                {
                    sb.AppendLine("<tr>")
                        .AppendLine($"<td>{user.Id}</td>")
                        .AppendLine($"<td>{user.DisplayName}</td>")
                        .AppendLine($"<td>{user.Email}</td>")
                        .AppendLine($"<td>{user.GitHubWorkAccount}</td>")
                        .AppendLine($"<td>{user.GitHubPersonalAccount}</td>")
                        .AppendLine("</tr>");
                }
                sb.AppendLine("</table>");

                var myProfile = await _usersService.GetMyProfileAsync();

                if (myProfile != null)
                {
                    var messageDto = new SendMessageDto
                    {
                        Subject = "GitHub accounts",
                        Sender = myProfile.Mail,
                        ToRecipients = new List<string> { myProfile.Mail },
                        Content = sb.ToString()
                    };
                    result = await _messagesService.SendToMailAsync(messageDto);
                }
            }
            _alertService.ShowAlert("Send mail", result ? "Data sent to mail" : "Data not sent. Check logs for error.");
            ExportingData = false;
        }

        private async void EditUserAccounts(object id)
        {
            var userId = id as string;
            var user = Users.FirstOrDefault(x => x.Id == userId);
            if (user != null)
            {
                var parameters = new Dictionary<string, object>();
                parameters[QueryAttributes.UserDto] = user;
                await Shell.Current.GoToAsync(Routes.UserDetails, parameters);
            }
        }
    }
}
