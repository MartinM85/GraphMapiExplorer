using MapiExplorer.UI.ViewModels;

namespace MapiExplorer.UI.Views;

public partial class MessagesPage : ContentPage
{
	public MessagesPage(MessagesPageViewModel viewModel)
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