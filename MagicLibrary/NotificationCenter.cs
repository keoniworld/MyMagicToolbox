using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicLibrary
{
    [Export(typeof(INotificationCenter))]
    public class NotificationCenter : INotificationCenter
    {
        /// <summary>
        /// Occurs when a notification should be raised.
        /// </summary>
        public event EventHandler<NotificationCenterEventArgs> NotificationFired;

        /// <summary>
        /// Fires the notification.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="message">The message.</param>
        public void FireNotification(string context, string message)
        {
            var eventToFire = NotificationFired;
            if (eventToFire != null)
            {
                eventToFire(this, new NotificationCenterEventArgs()
                {
                    TimeStampUtc = DateTime.UtcNow,
                    Context = context,
                    Message = message
                });
            }
        }
    }
}