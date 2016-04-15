using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BggUwp.Messaging
{
    public class RefreshDataMessage
    {
        public enum RefreshScope
        {
            All,
            Collection,
            Plays,
            BoardGame
        }

        public RefreshScope RequestedRefreshScope;

        public enum RefreshType
        {
            Local,
            Web
        }

        public RefreshType RequestedRefreshType;
    }
}
