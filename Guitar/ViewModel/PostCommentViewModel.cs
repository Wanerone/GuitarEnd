using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guitar.ViewModel
{
    public class PostCommentViewModel
    {
        public int Po_commentid { get; set; }
        public int Po_id { get; set; }
        public string content { get; set; }
        public System.DateTime Addtime { get; set; }
        public int User_id { get; set; }
        public string User_name { set; get; }
        public string User_img { set; get; }
    }
}