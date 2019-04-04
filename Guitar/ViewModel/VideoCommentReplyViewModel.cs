using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guitar.ViewModel
{
    public class VideoCommentReplyViewModel
    {
        public int Vi_replyid { set; get; }
        public int Vi_commentid { set; get; }
        public string content { set; get; }
        public DateTime Addtime { set; get; }
        public int Vi_id { set; get; }
        public int User_id { set; get; }
        public string User_name { set; get; }
        public string User_img { set; get; }
    }
}