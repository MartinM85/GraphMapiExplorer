namespace MapiExplorer.UI;

public partial class App : Application
{
	public App(AppShell mainPage)
	{
		InitializeComponent();

		MainPage = mainPage;
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        const int newHeight = 850;
        window.Height = newHeight;
        return window;
    }
}
