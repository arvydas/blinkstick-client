using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace BlinkStickClient
{
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        public event EventHandler<ItemUpdatedEventArgs> ItemUpdated;

        protected void OnItemUpdated(T item)
        {
            if (ItemUpdated != null && Contains(item))
            {
                ItemUpdated(this, new ItemUpdatedEventArgs(item));
            }
        }

        public void NotifyUpdate(T item)
        {
            OnItemUpdated(item);
        }
    }

    public class ItemUpdatedEventArgs : EventArgs
    {
        public object Item;

        public ItemUpdatedEventArgs(object item)
        {
            this.Item = item;
        }
    }
}

