using System;
using System.Collections.Generic;

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

            public NotificationRegistryEntry(String category, String name, String description, Type type)
            {
                this.Category = category;
                this.Name = name;
                this.Description = description;
                this.NotificationType = type;
            }
        }

        public static int Register(String category, String description, Type type)
        {
            using (Notification notification = (Notification)Activator.CreateInstance(type))
            {
                NotificationTypes.Add(new NotificationRegistryEntry(category, notification.GetTypeName(), description, type));
            }

            return NotificationTypes.Count - 1;
        }
    }
}

