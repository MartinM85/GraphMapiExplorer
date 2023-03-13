using MapiExplorer.UI.ViewModels;

namespace MapiExplorer.UI.Views;

public partial class MailFoldersPage : ContentPage
{
	public MailFoldersPage(MailFoldersPageViewModel viewModel)
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