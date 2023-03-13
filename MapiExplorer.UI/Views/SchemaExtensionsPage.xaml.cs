using MapiExplorer.UI.ViewModels;

namespace MapiExplorer.UI.Views;

public partial class SchemaExtensionsPage : ContentPage
{
	public SchemaExtensionsPage(SchemaExtensionsPageViewModel viewModel)
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