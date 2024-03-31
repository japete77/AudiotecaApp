using System.Collections.ObjectModel;

namespace fonoteca.ViewModels
{
    public class Grouping<K, T> : ObservableCollection<T>
    {
        public K Key { get; private set; }

        public Grouping(K key, IEnumerable<T> items) : base(items)
        {
            Key = key;
        }
    }
}
