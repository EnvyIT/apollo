using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Apollo.Util
{
    public static class ObservableCollectionExtension
    {

        public static void AddRange<T>(this ObservableCollection<T> observableCollection, ICollection<T> collection)
        {
            collection.ToList().ForEach(observableCollection.Add);
        }
    }
}
