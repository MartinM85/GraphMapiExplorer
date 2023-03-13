using MapiExplorer.UI.ViewModels;

namespace MapiExplorer.UI.Views;

public partial class FileDetailPage : ContentPage
{
	public FileDetailPage(FileDetailsViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}