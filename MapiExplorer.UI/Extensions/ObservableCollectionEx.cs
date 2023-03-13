using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MapiExplorer.UI.Extensions
{
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        public ObservableCollectionEx():base()
        {
        }

        public ObservableCollectionEx(IEnumerable<T> items) : base(items)
        {
        }

        public void ClearAndAddRange(IEnumerable<T> items)
        {
            ClearItems();
            foreach (var item in items)
            {
                Items.Add(item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
