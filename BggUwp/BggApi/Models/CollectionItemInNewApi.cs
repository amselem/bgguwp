using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BggApi.Models
{
    public class UserInfo
    {
        public string username { get; set; }
        public string avatar { get; set; }
        public string avatarfile { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string flagimgurl { get; set; }
    }

    public class Images
    {
        public string micro { get; set; }
        public string thumb { get; set; }
    }

    public class Privatecomment
    {
        public object value { get; set; }
        public string rendered { get; set; }
    }

    public class Comment
    {
        public object value { get; set; }
        public string tstamp { get; set; }
        public string rendered { get; set; }
    }

    public class Conditiontext
    {
        public object value { get; set; }
        public object tstamp { get; set; }
        public string rendered { get; set; }
    }

    public class Wantpartslist
    {
        public object value { get; set; }
        public object tstamp { get; set; }
        public string rendered { get; set; }
    }

    public class Haspartslist
    {
        public object value { get; set; }
        public object tstamp { get; set; }
        public string rendered { get; set; }
    }

    public class Wishlistcomment
    {
        public object value { get; set; }
        public object tstamp { get; set; }
        public string rendered { get; set; }
    }

    public class Customname
    {
        public object value { get; set; }
        public object tstamp { get; set; }
        public string rendered { get; set; }
    }

    public class Textfield
    {
        public Privatecomment privatecomment { get; set; }
        public Comment comment { get; set; }
        public Conditiontext conditiontext { get; set; }
        public Wantpartslist wantpartslist { get; set; }
        public Haspartslist haspartslist { get; set; }
        public Wishlistcomment wishlistcomment { get; set; }
        public Customname customname { get; set; }
    }

    public class Item
    {
        public string collid { get; set; }
        public object versionid { get; set; }
        public string objecttype { get; set; }
        public string objectid { get; set; }
        public object imageid { get; set; }
        public object publisherid { get; set; }
        public object languageid { get; set; }
        public object year { get; set; }
        public object other { get; set; }
        public object barcode { get; set; }
        [JsonIgnore]
        public User user { get; set; }
        public string objectname { get; set; }
        public object sunglasses { get; set; }
        [JsonIgnore]
        public Images images { get; set; }
        public object status { get; set; }
        public int wishlistpriority { get; set; }
        public object pp_currency { get; set; }
        public object pricepaid { get; set; }
        public object cv_currency { get; set; }
        public object currvalue { get; set; }
        public object quantity { get; set; }
        public object acquisitiondate { get; set; }
        public object acquiredfrom { get; set; }
        public object privatecomment { get; set; }
        public object invdate { get; set; }
        public object invlocation { get; set; }
        [JsonIgnore]
        public Textfield textfield { get; set; }
        public int rating { get; set; }
        public string rating_tstamp { get; set; }
        public string tstamp { get; set; }
        public string postdate { get; set; }
        public string lastmodified { get; set; }
        public string comment_tstamp { get; set; }
        public object review_tstamp { get; set; }
    }

    public class CollectionItemInNewApi
    {
        public List<Item> items { get; set; }
    }

}
