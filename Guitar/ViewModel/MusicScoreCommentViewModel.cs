using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guitar.ViewModel
{
    public class MusicScoreCommentViewModel
    {
        public int Ms_commentid { get; set; }
        public int Ms_id { get; set; }
        public string content { get; set; }
        public System.DateTime Addtime { get; set; }
        public int User_id { get; set; }
        public string User_name { set; get; }
        public string User_img { set; get; }
    }
}