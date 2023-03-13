using MapiExplorer.UI.ViewModels;

namespace MapiExplorer.UI.Views;

public partial class MapiDetailsPage : ContentPage
{
	public MapiDetailsPage(MapiDetailsViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}