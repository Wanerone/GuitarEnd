using Guitar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;
namespace Guitar.ViewModel
{
    public class PostDetailsViewModel
    {
        public Users Us { set; get; }
        public Post Po { set; get; }
        public IEnumerable<Post> Po1 { get; set; }
        public PagedList<PostCommentViewModel> Pocomment { get; set; }
        public IEnumerable<PostCommentReplyViewModel> Poreply { get; set; }
    }
}