using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Service
{
    public class Enums
    {
        public enum NotificationType
        {
            error,
            success,
            warning,
            info,
            question
        }

        public enum NotificationToastr
        {
            error,
            success,
            warning,
            info
        }
    }
}
