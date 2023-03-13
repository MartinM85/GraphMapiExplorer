using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using MapiExplorer.UI.Views;
using MapiExplorer.UI.ViewModels;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using MapiExplorer.Services;
using MapiExplorer.UI.Services;

namespace MapiExplorer.UI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .Configuration.AddJsonFile("appsettings.json");
        builder.Logging.AddNLog("NLog.config");
        builder.Services.AddSingleton(x =>
            {
                if (OperatingSystem.IsWindows())
                {
                    var clientId = builder.Configuration["AzureAd:ClientId"];
                    var tenantId = builder.Configuration["AzureAd:TenantId"];
                    var scopes = builder.Configuration["AzureAd:Scopes"]?.Split(' ');
                    var redirectUri = builder.Configuration["AzureAd:RedirectUri"];
                    var options = new InteractiveBrowserCredentialOptions
                    {
                        ClientId = clientId,
                        TenantId = tenantId,
                        AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                        RedirectUri = new Uri(redirectUri)    
                    };

                    InteractiveBrowserCredential interactiveCredential = new(options);
                    return new GraphServiceClient(interactiveCredential, scopes);
                }
                else
                {
                    throw new NotSupportedException();
                }
            });
        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddSingleton<IAppSettings, AppSettings>();
        builder.Services.AddSingleton<IPidPropertiesSettings, PidPropertiesSettings>();
        builder.Services.AddSingleton<IUsersService, UsersService>();
        builder.Services.AddSingleton<IExportService, ExportService>();
        builder.Services.AddSingleton<IFilesService, FilesService>();
        builder.Services.AddSingleton<IExcelService, ExcelService>();
        builder.Services.AddSingleton<IMessagesService, MessagesService>();
        builder.Services.AddSingleton<ISchemaExtensionsService, SchemaExtensionsService>();
        builder.Services.AddSingleton<IApplicationService, ApplicationService>();
        builder.Services.AddSingleton<ISitesService, SitesService>();
        builder.Services.AddSingleton<ICalendarsService, CalendarsService>();
        builder.Services.AddSingleton<IContactFoldersService, ContactFoldersService>();
        builder.Services.AddSingleton<IEventsService, EventsService>();
        builder.Services.AddSingleton<IMailFoldersService, MailFoldersService>();
        builder.Services.AddSingleton<IContactsService, ContactsService>();
        builder.Services.AddSingleton<ITranslateIdsService, TranslateIdsService>();
        builder.Services.AddSingleton<IPidPropertiesService, PidPropertiesService>();
        builder.Services.AddSingleton<IAlertService, AlertService>();

        builder.Services.AddSingleton<UsersPage>();
        builder.Services.AddSingleton<UsersPageViewModel>();
        builder.Services.AddTransient<UserAccountsPage>();
        builder.Services.AddTransient<UserAccountsViewModel>();

        builder.Services.AddSingleton<MessagesPage>();
        builder.Services.AddSingleton<MessagesPageViewModel>();
        

        builder.Services.AddTransient<MapiDetailsPage>();
        builder.Services.AddTransient<MapiDetailsViewModel>();

        builder.Services.AddSingleton<SchemaExtensionsPage>();
        builder.Services.AddSingleton<SchemaExtensionsPageViewModel>();

        builder.Services.AddTransient<SchemaExtensionDetailsPage>();
        builder.Services.AddTransient<SchemaExtensionDetailsViewModel>();

        builder.Services.AddSingleton<FilesPage>();
        builder.Services.AddSingleton<FilesPageViewModel>();
        builder.Services.AddTransient<FileDetailPage>();
        builder.Services.AddTransient<FileDetailsViewModel>();
        builder.Services.AddTransient<FolderDetailPage>();
        builder.Services.AddTransient<FolderDetailsViewModel>();

        builder.Services.AddSingleton<CalendarsPage>();
        builder.Services.AddSingleton<CalendarsPageViewModel>();
        builder.Services.AddSingleton<ContactFoldersPage>();
        builder.Services.AddSingleton<ContactFoldersPageViewModel>();
        builder.Services.AddSingleton<ContactsPage>();
        builder.Services.AddSingleton<ContactsPageViewModel>();
        builder.Services.AddSingleton<EventsPage>();
        builder.Services.AddSingleton<EventsPageViewModel>();
        builder.Services.AddSingleton<MailFoldersPage>();
        builder.Services.AddSingleton<MailFoldersPageViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
