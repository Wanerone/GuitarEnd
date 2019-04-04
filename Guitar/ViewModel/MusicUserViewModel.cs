using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guitar.ViewModel
{
    public class MusicUserViewModel
    {
        public int Ms_id { set; get; }
        public string Ms_title { set; get; }
        public string Ms_img { set; get; }
        public DateTime Ms_addtime { set; get; }
        public string Ms_label { set; get; }
        public int User_id { set; get; }
        public string User_name { set; get; }
    }
}