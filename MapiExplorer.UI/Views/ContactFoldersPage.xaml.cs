using MapiExplorer.UI.ViewModels;

namespace MapiExplorer.UI.Views;

public partial class ContactFoldersPage : ContentPage
{
	public ContactFoldersPage(ContactFoldersPageViewModel viewModel)
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