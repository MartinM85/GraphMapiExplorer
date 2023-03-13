using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MapiExplorer.UI.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected abstract Task LoadDataAsync();

        private Task _loadDataTask;

        public void LoadData()
        {
            if (_loadDataTask != null)
            {
                if (_loadDataTask.IsCompleted)
                {
                    _loadDataTask.Dispose();
                    _loadDataTask = null;
                }
                else
                {
                    return;
                }
            }
            _loadDataTask = LoadDataAsync();
        }
    }
}
