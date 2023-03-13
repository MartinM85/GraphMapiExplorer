using MapiExplorer.UI.ViewModels;

namespace MapiExplorer.UI.Views;

public partial class EventsPage : ContentPage
{
	public EventsPage(EventsPageViewModel viewModel)
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