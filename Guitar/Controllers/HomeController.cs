using Guitar.Models;
using Guitar.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Guitar.Controllers
{
    public class HomeController : Controller
    {
        private GuitarEntities db = new GuitarEntities();
        public ActionResult Index()
        {
            var scores = db.MusicScore.OrderByDescending(p => p.Ms_addtime).FirstOrDefault();
            var score1 = (from p in db.MusicScore select p).OrderByDescending(p => p.Ms_addtime).Take(10);
            var score2= (from p in db.MusicScore select p).OrderByDescending(p => p.ReadCount).Take(10);
            var video = from m in db.Video
                     join n in db.Users on m.User_id equals n.User_id
                     select new VideoUserViewModel
                     {
                         Vi_id=m.Vi_id,
                         Vi_title = m.Vi_title,
                         Vi_addtime = m.Vi_addtime,
                         Vi_label=m.Vi_label,
                         Vi_img=m.Vi_img,
                         User_id = n.User_id,
                         User_name = n.User_name,
                     };
            var users = from m in db.Users.OrderByDescending(p => p.User_addtime) select m;
            var video1 = (from m in video.Where(p=>p.Vi_label=="教学").OrderByDescending(p => p.Vi_addtime) select m).Take(8);
            var video2 = (from m in video.Where(p => p.Vi_label == "评测").OrderByDescending(p => p.Vi_addtime) select m).Take(10);
            var video3 = (from m in video.Where(p => p.Vi_label == "专辑").OrderByDescending(p => p.Vi_addtime) select m).Take(10);
            var post = from m in db.Post
                        join n in db.Users on m.User_id equals n.User_id
                        select new PostUserViewModel
                        {
                            Po_id = m.Po_id,
                            Po_title = m.Po_title,
                            Po_addtime = m.Po_addtime,
                            Po_label = m.Po_label,
                            Po_img = m.Po_img,
                            User_id = n.User_id,
                            User_name = n.User_name,
                        };
            var posts = (from m in post.OrderByDescending(p => p.Po_addtime) select m).Take(12);
            var index = new Guitar.ViewModel.IndexViewModel()
            {
                MScore = scores,
                Us=users,
                MScore1 = score1,
                MScore2=score2,
                Videos1=video1,
                Videos2=video2,
                Videos3=video3,
                Posts=posts,
            };

            return View(index);
        }

        //public IList<MusicViewModel> GetMusic()
        //{
        //    IList<MusicViewModel> list = (from s in (from p in db.MusicScore
        //                                         join o in db.MusicScoreStatistics on p.Ms_id equals o.Ms_id
        //                                         select new MusicViewModel
        //                                         {
        //                                             Ms_id = p.Ms_id,
        //                                             Ms_title = p.Ms_title,
        //                                             Ms_img = p.Ms_img,
        //                                             Ms_label = p.Ms_label,
        //                                             ReadCount = o.ReadCount
        //                                         })
        //                              select s).OrderByDescending(s => s.ReadCount).Take(3).ToList();
        //    return list;
        //}
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Test()
        {
            return View();
        }
    }
}