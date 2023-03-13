using MapiExplorer.UI.ViewModels;

namespace MapiExplorer.UI.Views;

public partial class CalendarsPage : ContentPage
{
	public CalendarsPage(CalendarsPageViewModel viewModel)
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