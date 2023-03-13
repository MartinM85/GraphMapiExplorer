using MapiExplorer.UI.ViewModels;

namespace MapiExplorer.UI.Views;

public partial class FolderDetailPage : ContentPage
{
	public FolderDetailPage(FolderDetailsViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}