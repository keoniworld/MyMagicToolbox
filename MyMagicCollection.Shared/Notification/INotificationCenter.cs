using System;
using NLog;

namespace MyMagicCollection.Shared
{
    public interface INotificationCenter
    {
        /// <summary>
        /// Occurs when a notification should be raised.
        /// </summary>
        event EventHandler<NotificationCenterEventArgs> NotificationFired;

        /// <summary>
        /// Fires the notification.
        /// </summary>
        /// <param name="message">The message.</param>
        void FireNotification(LogLevel logLevel, string message);
    }
}