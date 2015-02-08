using System;
using System.Collections.Generic;
using Gtk;

namespace BlinkStickClient.DataModel
{
    public static class NotificationRegistry
    {
        public static List<NotificationRegistryEntry> NotificationTypes = new List<NotificationRegistryEntry>();

        public class NotificationRegistryEntry
        {
            public Type NotificationType;
            public String Category;
            public String Name;
            public String Description;
            public Boolean IsSupported;

            public NotificationRegistryEntry(String category, String name, String description, Type type, Boolean isSupported)
            {
                this.Category = category;
                this.Name = name;
                this.Description = description;
                this.NotificationType = type;
                this.IsSupported = isSupported;
            }
        }

        public static int Register(String category, String description, Type type)
        {
            using (Notification notification = (Notification)Activator.CreateInstance(type))
            {
                NotificationTypes.Add(new NotificationRegistryEntry(category, notification.GetTypeName(), description, type, notification.IsSupported()));
            }

            return NotificationTypes.Count - 1;
        }
    }
}

