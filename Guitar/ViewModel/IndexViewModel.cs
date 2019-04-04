using Guitar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guitar.ViewModel
{
    public class IndexViewModel
    {
        public MusicScore MScore { set; get; }
        public IEnumerable<Users> Us { set; get; }
        public IEnumerable<MusicScore> MScore1 { get; set; }
        public IEnumerable<MusicScore> MScore2 { get; set; }
        public IEnumerable<VideoUserViewModel> Videos1 { get; set; }
        public IEnumerable<VideoUserViewModel> Videos2 { get; set; }
        public IEnumerable<VideoUserViewModel> Videos3 { get; set; }
        public IEnumerable<PostUserViewModel> Posts { get; set; }
    }
}