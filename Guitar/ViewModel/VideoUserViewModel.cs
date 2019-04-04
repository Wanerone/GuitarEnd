using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guitar.ViewModel
{
    public class VideoUserViewModel
    {
        public int Vi_id { set; get; }
        public string Vi_title { set; get; }
        public string Vi_img { set; get; }
        public DateTime Vi_addtime { set; get; }
        public string Vi_label { set; get; }
        public int User_id { set; get; }
        public string User_name { set; get; }
    }
}