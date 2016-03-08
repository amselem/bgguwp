using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BggApi.Models.Abstract;

namespace BggApi.Models
{
    public class HotItem : BasicItem
    {
        public int Rank { get; set; }
    }
}
