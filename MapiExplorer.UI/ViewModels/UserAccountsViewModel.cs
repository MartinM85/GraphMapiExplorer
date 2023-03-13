using MapiExplorer.Models;
using MapiExplorer.Services;
using Microsoft.Graph.Models;
using System.Text;
using System.Windows.Input;

namespace MapiExplorer.UI.ViewModels
{
    public class UserAccountsViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly IUsersService _usersService;
        private readonly IApplicationService _applicationService;
        private readonly IAppSettings _appSettings;

        public UserAccountsViewModel(IUsersService usersService, IApplicationService applicationService, IAppSettings appSettings)
        {
            _usersService = usersService;
            _applicationService = applicationService;
            _appSettings = appSettings;

            BackCommand = new Command(BackToUsers);
            SaveCommand = new Command(SaveUser);
        }

        private string _userId;
        private ImageSource _photo;
        private string _displayName;
        private string _mail;
        private string _workAccount;
        private string _personalAccount;

        public ImageSource Photo
        {
            get { return _photo; }
            set
            {
                _photo = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName 
        { 
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public string Mail 
        { 
            get => _mail;
            set
            {
                _mail = value;
                OnPropertyChanged();
            }
        }

        public string WorkAccount 
        { 
            get => _workAccount;
            set
            {
                _workAccount = value;
                OnPropertyChanged();
            }
        }

        public string _managers;
        public string Managers
        {
            get => _managers;
            set
            {
                _managers = value;
                OnPropertyChanged();
            }
        }

        public string PersonalAccount 
        { 
            get => _personalAccount;
            set
            {
                _personalAccount = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue(QueryAttributes.UserDto, out var value))
            {
                var user = value as UserDto;
                if (user != null)
                {
                    _userId = user.Id;
                    _displayName = user.DisplayName;
                    _mail = user.Email;
                    _workAccount = user.GitHubWorkAccount;
                    _personalAccount = user.GitHubPersonalAccount;
                    _managers = null;
                    LoadData();
                }
            }
        }

        protected override async Task LoadDataAsync()
        {
            var photo = await _usersService.GetUserPhotoAsync(_userId);
            var manager = await _usersService.GetUserManagerAsync(_userId);

            var sb = new StringBuilder();
            if (manager?.Manager is User userManager)
            {              
                sb.Append(userManager.DisplayName);
                while (userManager.Manager is User upperManager)
                {
                    sb.Append(" => ").Append(upperManager.DisplayName);
                    userManager = upperManager;
                }
            }
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (photo != null)
                {
                    _photo = ImageSource.FromStream(() => photo);
                }                
                _managers = sb.ToString();
                OnPropertyChanged(nameof(Photo));
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(Mail));
                OnPropertyChanged(nameof(WorkAccount));
                OnPropertyChanged(nameof(PersonalAccount));
                OnPropertyChanged(nameof(Managers));
            });
        }

        private async void BackToUsers()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void SaveUser()
        {
            await SaveAndGoBackAsync();
        }

        private async Task SaveAndGoBackAsync()
        {
            var schemaExtensionName = await _applicationService.GetExtensionPropertyValueAsync<string>(_appSettings.ApplicationId, GitHubExtensionProperty.Id);
            if (!string.IsNullOrEmpty(schemaExtensionName))
            {
                await _usersService.SaveGitHubAccountsAsync(_userId, WorkAccount, PersonalAccount, schemaExtensionName);
                BackToUsers();
            }            
        }
    }
}
