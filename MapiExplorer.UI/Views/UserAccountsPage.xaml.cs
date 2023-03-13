using MapiExplorer.UI.ViewModels;

namespace MapiExplorer.UI.Views;

public partial class UserAccountsPage : ContentPage
{
	public UserAccountsPage(UserAccountsViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}