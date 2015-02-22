using System;

namespace MagicLibrary
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
        /// <param name="context">The context.</param>
        /// <param name="message">The message.</param>
        void FireNotification(string context, string message);
    }
}