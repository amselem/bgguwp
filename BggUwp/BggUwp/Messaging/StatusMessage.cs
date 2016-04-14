using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BggUwp.Messaging
{
    public class StatusMessage
    {
        public enum StatusType
        {
            Success,
            Error
        }

        public StatusType Status;
        public string Message;
    }
}
