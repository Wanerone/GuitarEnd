using Guitar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guitar.ViewModel
{
    public class UsersViewModel
    {
        public Users Us { set; get; }
        public IEnumerable<MusicUserViewModel> MS { set; get; }
        public IEnumerable<PostUserViewModel> PO { set; get; }
        public IEnumerable<Friend> FD { set; get; }
        public IEnumerable<VideoUserViewModel> VO { set; get; }
    }
}