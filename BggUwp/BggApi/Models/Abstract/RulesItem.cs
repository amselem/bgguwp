using System.Collections.Generic;
using Newtonsoft.Json;

namespace BGGAPI.Models
{
    public class RulesItem
    {
        [JsonIgnore]
        public Config ConfigHeader { get; set; }

        public List<Weblink> WebLinks { get; set; }

        public class Language
        {
            public object ObjectId { get; set; }
            public string Name { get; set; }
        }

        public class Category
        {
            public object ObjectId { get; set; }
            public string Name { get; set; }
        }

        public class Config
        {
            public List<Language> Languages { get; set; }
            public List<Category> Categories { get; set; }
            public int Endpage { get; set; }
            public bool ShowControls { get; set; }
        }

        public class Weblink
        {
            public string Url { get; set; }
            public string Name { get; set; }
            public string Postdate { get; set; }
            public string LinkId { get; set; }
            public string LinkType { get; set; }
            public string ObjectType { get; set; }
            public string ObjectId { get; set; }
            public string ItemState { get; set; }
            public string RepImageId { get; set; }
            public string ObjectLink { get; set; }
            public List<string> Languages { get; set; }
            public List<string> Categories { get; set; }
        }
    }
}