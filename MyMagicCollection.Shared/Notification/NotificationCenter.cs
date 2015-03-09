using System;
using NLog;

namespace MyMagicCollection.Shared
{
    public class NotificationCenter : INotificationCenter
    {
        static NotificationCenter()
        {
            Instance = new NotificationCenter();
        }

        /// <summary>
        /// Occurs when a notification should be raised.
        /// </summary>
        public event EventHandler<NotificationCenterEventArgs> NotificationFired;

        /// <summary>
        /// Fires the notification.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="message">The message.</param>
        public void FireNotification(LogLevel logLevel, string message)
        {
            var eventToFire = NotificationFired;
            if (eventToFire != null)
            {
                eventToFire(this, new NotificationCenterEventArgs()
                {
                    TimeStampUtc = DateTime.UtcNow,
                    LogLevel = logLevel ?? LogLevel.Info,
                    Message = message
                });
            }
        }

        public static INotificationCenter Instance { get; private set; }
    }
}