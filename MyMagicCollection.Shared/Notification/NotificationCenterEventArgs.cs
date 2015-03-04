﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMagicCollection.Shared
{
    public class NotificationCenterEventArgs : EventArgs
    {
        public DateTime TimeStampUtc { get; set; }
        public string Context { get; set; }
        public string Message { get; set; }
    }
}