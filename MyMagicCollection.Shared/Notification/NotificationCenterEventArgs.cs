using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace MyMagicCollection.Shared
{
    public class NotificationCenterEventArgs : EventArgs
    {
        public DateTime TimeStampUtc { get; set; }

        public LogLevel LogLevel { get; set; }

        public string Message { get; set; }
    }
}