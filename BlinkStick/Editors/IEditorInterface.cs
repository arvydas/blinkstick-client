using System;
using BlinkStickClient.DataModel;

namespace BlinkStickClient
{
    public interface IEditorInterface
    {
        void SetNotification(Notification notification);

        Boolean IsValid(Gtk.Window window);

        void UpdateNotification();
    }
}

