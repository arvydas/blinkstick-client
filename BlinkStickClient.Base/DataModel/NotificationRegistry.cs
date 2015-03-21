using System;
using System.Collections.Generic;
using Gtk;
using Gdk;

namespace BlinkStickClient.DataModel
{
    public static class NotificationRegistry
    {
        public static List<NotificationRegistryEntry> NotificationTypes = new List<NotificationRegistryEntry>();

        public class NotificationRegistryEntry
        {
            public Type NotificationType;
			public Type EditorType;
            public String Category;
            public String Name;
            public String Description;
            public Boolean IsSupported;
            public Pixbuf Icon;

            public NotificationRegistryEntry(String category, String name, String description, Type type, Boolean isSupported, Type editorType, Pixbuf icon)
            {
                this.Category = category;
                this.Name = name;
                this.Description = description;
                this.NotificationType = type;
                this.IsSupported = isSupported;
				this.EditorType = editorType;
                this.Icon = icon;
            }
        }

        public static int Register(String category, String description, Type type, Type editorType, Pixbuf icon = null)
        {
            using (Notification notification = (Notification)Activator.CreateInstance(type))
            {
				NotificationTypes.Add(
					new NotificationRegistryEntry(
						category, 
						notification.GetTypeName(), 
						description, 
						type, 
						notification.IsSupported(), 
						editorType,
                    icon));
            }

            return NotificationTypes.Count - 1;
        }

		public static Type FindEditorType(Type notificationType)
		{
			foreach (NotificationRegistryEntry entry in NotificationTypes) {
				if (entry.NotificationType == notificationType) {
					return entry.EditorType;
				}
			}

			return null;
		}

        public static Pixbuf FindIcon(Type notificationType)
        {
            foreach (NotificationRegistryEntry entry in NotificationTypes) {
                if (entry.NotificationType == notificationType) {
                    return entry.Icon;
                }
            }

            return null;
        }
    }
}

