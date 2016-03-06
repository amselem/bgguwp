using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGGAPI.Models.Abstract
{
    public abstract class BasicItem 
    {
        public int BoardGameId { get; set; }
        public string Name { get; set; }
        public int YearPublished { get; set; }
        public string ThumbnailWeb { get; set; }
    }
}
