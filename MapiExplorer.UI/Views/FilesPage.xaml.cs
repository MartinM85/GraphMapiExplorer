using MapiExplorer.UI.ViewModels;

namespace MapiExplorer.UI.Views;

public partial class FilesPage : ContentPage
{
	public FilesPage(FilesPageViewModel viewModel)
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