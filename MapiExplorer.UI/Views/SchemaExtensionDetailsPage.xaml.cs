using MapiExplorer.UI.ViewModels;

namespace MapiExplorer.UI.Views;

public partial class SchemaExtensionDetailsPage : ContentPage
{
	public SchemaExtensionDetailsPage(SchemaExtensionDetailsViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}