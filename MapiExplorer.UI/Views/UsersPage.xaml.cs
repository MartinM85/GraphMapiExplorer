using MapiExplorer.UI.ViewModels;

namespace MapiExplorer.UI.Views;

public partial class UsersPage : ContentPage
{
	public UsersPage(UsersPageViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		((ViewModelBase)BindingContext).LoadData();
    }
}