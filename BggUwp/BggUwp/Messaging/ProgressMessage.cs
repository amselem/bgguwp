using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BggUwp.Messaging
{
    public class ProgressMessage
    {
        public enum ProgressState
        {
            Started,
            Finished
        }

        public ProgressState State;
        public string Message;
    }
}
