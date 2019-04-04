using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guitar.ViewModel
{
    public class PostUserViewModel
    {
        public int Po_id { set; get; }
        public string Po_title { set; get; }
        public string Po_img { set; get; }
        public DateTime Po_addtime { set; get; }
        public string Po_label { set; get; }
        public int User_id { set; get; }
        public string User_name { set; get; }
    }
}