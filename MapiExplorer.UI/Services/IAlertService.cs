namespace MapiExplorer.UI.Services
{
    public interface IAlertService
    {
        void ShowAlert(string title, string message, string cancel = "OK");
        Task ShowAlertAsync(string title, string message, string cancel = "OK");
        void ShowConfirmation(string title, string message, Action<bool> callback, string accept = "Yes", string cancel = "No");
        Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No");
    }
}