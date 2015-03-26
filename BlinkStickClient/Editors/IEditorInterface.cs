using System;
using BlinkStickClient.DataModel;

namespace BlinkStickClient
{
    public interface IEditorInterface
    {
        void SetNotification(CustomNotification notification);

        Boolean IsValid(Gtk.Window window);

        void UpdateNotification();
    }
}

