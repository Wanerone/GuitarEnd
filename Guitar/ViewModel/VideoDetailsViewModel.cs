using Guitar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Guitar.ViewModel
{
    public class VideoDetailsViewModel
    {
        public Users Us { set; get; }
        public Video Vid { set; get; }
        public IEnumerable<Video> Vid1 { get; set; }
        public PagedList<VideoCommentViewModel> Vidcomment { get; set; }
        public IEnumerable<VideoCommentReplyViewModel> Vidreply { get; set; }
    }
}