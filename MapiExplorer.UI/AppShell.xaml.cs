using MapiExplorer.UI.Views;

namespace MapiExplorer.UI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(Routes.UserDetails, typeof(UserAccountsPage));
		Routing.RegisterRoute(Routes.MapiDetails, typeof(MapiDetailsPage));
		Routing.RegisterRoute(Routes.SchemaExtensionDetail, typeof(SchemaExtensionDetailsPage));
		Routing.RegisterRoute(Routes.FileDetails, typeof(FileDetailPage));
        Routing.RegisterRoute(Routes.FolderDetails, typeof(FolderDetailPage));
    }
}
