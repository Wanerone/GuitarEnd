using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Guitar.Models;
using Guitar.ViewModel;
using Webdiyer.WebControls.Mvc;
namespace Guitar.Controllers
{
    public class VideoController : Controller
    {
        private GuitarEntities db = new GuitarEntities();
        #region 视频主页
        // GET: Video
        public ActionResult Index(int id = 1, int pageIndex = 1, int page = 1)
        {
            var video = from m in db.Video
                        join n in db.Users on m.User_id equals n.User_id
                        select new VideoUserViewModel
                        {
                            Vi_id = m.Vi_id,
                            Vi_title = m.Vi_title,
                            Vi_addtime = m.Vi_addtime,
                            Vi_label = m.Vi_label,
                            Vi_img = m.Vi_img,
                            User_id = n.User_id,
                            User_name = n.User_name,
                        };
            var video1 = from m in video.OrderByDescending(p => p.Vi_addtime) select m;
            const int pageSize = 12;
            if (Request.IsAjaxRequest())
            {
                var target = Request.QueryString["target"];
                if (target == "video")
                {
                    return PartialView("_Index1", video1.ToPagedList(id, pageSize));
                }
            }
            return View(video1.ToPagedList(page, pageSize));
        }
        #endregion
        #region 视频细节
        public ActionResult Details(int? id, int md = 1, int pageIndex = 1, int page = 1)
        {
           
             if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var vid = db.Video.Find(id);
            var user = db.Users.Find(vid.User_id);
            if (vid == null && user == null)
            {
                return HttpNotFound();
            }
            var vid1 = (from m in db.Video.Where(p => p.User_id == vid.User_id).OrderByDescending(p => p.Vi_addtime) select m).Take(8);
            TempData["Vi_id2"] = id;
            TempData["Vi_idd"] = id;
            if (Session["Users_id"] != null)
            {
                var userid = Convert.ToInt32(Session["Users_id"]);
                var coll =(from m in db.VideoCollection.Where(p => p.User_id == userid).Where(p => p.Vi_id == id) select m).FirstOrDefault();
                if (coll!=null&&coll.State ==1)
                {
                    ViewBag.rel = "unlike";
                }
                else
                {
                    ViewBag.rel = "like";
                }
                var distribute =(from m in db.Friend.Where(p => p.UserA == userid) select m).FirstOrDefault();
                if(distribute!=null)
                {
                 if (distribute.UserB ==vid.User_id)
                    {
                        ViewBag.dis = "已关注";
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    ViewBag.dis = "关注";
                }
               
            }
            else
            {
                ViewBag.dis = "关注";
                ViewBag.rel = "like";
            }
            var comment = from m in db.VideoComment.Where(p => p.Vi_id == id).OrderByDescending(p => p.Addtime) select m;
            var comment1 = from m in db.VideoComment
                          join n in db.Users on m.User_id equals n.User_id
                          select new VideoCommentViewModel
                          {
                              Vi_commentid = m.Vi_commentid,
                              Vi_id = m.Vi_id,
                              content = m.content,
                              Addtime = m.Addtime,
                              User_id = n.User_id,
                              User_name = n.User_name,
                              User_img = n.User_img,
                          };
            //var comment1 = .ToPagedList(idd, pageSize);
            var vidreply = (from n in db.VideoReply
                       join m in comment1 on n.Vi_commentid equals m.Vi_commentid
                       join q in db.Users on n.User_id equals q.User_id
                       select new VideoCommentReplyViewModel
                       {
                           Vi_replyid = n.Vi_replyid,
                           Vi_commentid = m.Vi_commentid,
                           content = n.content,
                           Addtime = n.Addtime,
                           Vi_id = m.Vi_id,
                           User_id = n.User_id,
                           User_name = q.User_name,
                           User_img = q.User_img
                       });
            var vidcomment = from m in comment1.Where(p => p.Vi_id == id).OrderByDescending(p => p.Addtime) select m;
            //分页数据
            const int pageSize = 5;
            if (Request.IsAjaxRequest())
            {
                var target = Request.QueryString["target"];
                if (target == "articles")
                {
                    return PartialView("_details1", vidcomment.ToPagedList(md, pageSize));
                }
            }
            var index = new Guitar.ViewModel.VideoDetailsViewModel()
            {
                Us = user,
                Vid = vid,
                Vid1 = vid1,
                Vidcomment = vidcomment.ToPagedList(page, pageSize),
                Vidreply = vidreply,
            };
            return View(index);
        }
        #endregion
        #region 发布视频
        public ActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Video video)
        {
            var sel = Request["sel"];
            HttpPostedFileBase postimageBase = Request.Files["Image1"];
            HttpPostedFileBase postvideoBase = Request.Files["VideoFile"];
            if (ModelState.IsValid)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (postimageBase != null)
                        {
                            string filePath = postimageBase.FileName;
                            string filename = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                            string serverpath = Server.MapPath(@"/Images/FM/") + filename;
                            string relativepath = @"/Images/FM/" + filename;
                            postimageBase.SaveAs(serverpath);
                            video.Vi_img = relativepath;
                        }

                        else
                        {
                            return Content("<script>;alert('请先上传封面！');history.go(-1)</script>");

                        }
                        if (postvideoBase != null)
                        {
                            string filePath = postvideoBase.FileName;
                            string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                            string serverpath = Server.MapPath(@"/Videos/") + fileName;
                            string relativepath = @"/Videos/" + fileName;
                            postvideoBase.SaveAs(serverpath);
                            video.Vi_url = relativepath;
                        }

                        else
                        {
                            return Content("<script>;alert('视频没有上传！');history.go(-1)</script>");

                        }
                        video.User_id = Convert.ToInt32(Session["Users_id"].ToString());
                        //publish_food.Amount = 0;
                        video.Vi_addtime = System.DateTime.Now;
                        video.Vi_label = sel;
                        db.Video.Add(video);
                        db.SaveChanges();
                        //return Content("<script>;alert('分享成功!');window.location.href='/Publish_Food/Index_PF'</script>");
                        return Content("<script>;alert('发布成功!');history.go(-1)</script>");
                    }
                    else
                    {
                        return Content("<script>;alert('发布失败！');history.go(-1)</script>");
                    }
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }

            }
            return View();
        }
        #endregion
        #region 收藏
        [HttpPost]
        public string Collection(VideoCollection vi)
        {
            if (Session["Users_id"] != null)
            {
                int Vi_id = Convert.ToInt32(Request["vid"]);
                int User_id = Convert.ToInt32(Session["Users_id"]);
                var vi1 = db.VideoCollection.Where(o => o.User_id == User_id).Where(o => o.Vi_id == Vi_id).FirstOrDefault();
                if (vi1 == null)
                {
                    if (ModelState.IsValid)
                    {
                        vi.Vi_id = Vi_id;
                        vi.User_id = User_id;
                        vi.State = 1;
                        db.VideoCollection.Add(vi);
                        db.SaveChanges();
                        var collection = db.Video.Find(Vi_id);

                        collection.Collection = collection.Collection + 1;
                        var coll = collection.Collection;
                        db.SaveChanges();
                    //return Json("");
                    //return Content("<script>;alert('收藏成功!');</script>");
                    //return new EmptyResult();
                    return "11";
                }
                }
                else
                {
                    if (vi1.State == 0)
                    {
                        var collection = db.Video.Find(Vi_id);
                        collection.Collection = collection.Collection + 1;
                        var coll1 = collection.Collection;
                        db.SaveChanges();
                        vi1.State = 1;
                        db.SaveChanges();
                    //return Json("");
                    //return Content("<script>;alert('收藏成功!');</script>");
                    //return new EmptyResult();
                    return "22";
                }
                    if (vi1.State == 1)
                    {
                        var collection = db.Video.Find(Vi_id);
                        collection.Collection = collection.Collection - 1;
                        var coll2 = collection.Collection;
                        db.SaveChanges();
                        vi1.State = 0;
                        db.SaveChanges();
                    //return Content("<script>;alert('取消收藏成功!');</script>");
                    //return Json("");
                    //return new EmptyResult();
                    return "33";
                }
                }
            }
            else
            {
                return "55"; ;
            }
            //return RedirectToAction("Details", "Video");
            return "44";
        }
        #endregion
        #region 关注
        [HttpPost]
        public string Guanzhu(Friend fri)
        {
            if (Session["Users_id"] != null)
            {
                int upid = Convert.ToInt32(Request["User_id"]);
                int userid = Convert.ToInt32(Session["Users_id"]);
                if (upid == userid)
                {
                    return "a";
                }
                var f1 = db.Friend.Where(p => p.UserA == userid).Where(p => p.UserB == upid).FirstOrDefault();

                if (f1 == null)
                {
                    if (ModelState.IsValid)
                    {
                        fri.UserA = userid;
                        fri.UserB = upid;
                        db.Friend.Add(fri);
                        db.SaveChanges();
                        ViewBag.dis = "已关注";
                        return "b";
                    }
                    else
                    {
                        return "c";
                    }
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        fri = f1;
                        db.Friend.Remove(fri);
                        db.SaveChanges();
                        ViewBag.dis = "关注";
                        return "d";
                    }
                    else
                    {
                        return "c";
                    }
                }
            }
            else
            {
                return "e";
            }
        }
        #endregion
        #region 评论视频
        public string Comment(VideoComment vid)
        {
            string pingluntextarea = Request["pingluntextarea"];
            int Vi_id = Convert.ToInt32(Request["Vi_id1"]);
            if (pingluntextarea == "")
            {
                return "kong";
            }
            if (Session["Users_id"] != null)
            {
                if (ModelState.IsValid)
                            {
                                vid.Vi_id = Vi_id;
                                vid.content = pingluntextarea;
                                vid.User_id = Convert.ToInt32(Session["Users_id"].ToString());
                                vid.Addtime = System.DateTime.Now;
                                db.VideoComment.Add(vid);
                                db.SaveChanges();
                                //return Content("<script>;alert('评论成功!');");
                                //return RedirectToAction("Display", "MusicScore");
                                return "aa";
                            }
                            else
                            {
                                //return Content("bb");
                                //return RedirectToAction("Display", "MusicScore");
                                return "bb";
                            }
            }
            else
            {
                return "nn";
            }

            //return RedirectToAction("Display", "MusicScore");
        }
        #endregion
        #region 回复视频
        [HttpPost]
        public string Reply(VideoReply msr)
        {
            string pingluntextarea = Request["pingluntextarea2"];
            var se = TempData["Vi_id2"];
            int Vi_id = Convert.ToInt32(se);
            //int Ms_id = Convert.ToInt32(Request["Ms_id2"]);
            int commentid = Convert.ToInt32(Request["Commentid"]);
            if (Session["Users_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    msr.Vi_id = Vi_id;
                    msr.content = pingluntextarea;
                    msr.Vi_commentid = commentid;
                    msr.User_id = Convert.ToInt32(Session["Users_id"].ToString());
                    msr.Addtime = System.DateTime.Now;
                    db.VideoReply.Add(msr);
                    db.SaveChanges();
                    //return Content("<script>;alert('回复成功!');history.go(-1)</script>");
                    return "aa";
                }
                else
                {
                    //return Content("bb");
                    //return RedirectToAction("Display", "MusicScore");
                    return "kong";
                }
            }
            else
            {
                return "nn";
            }
           
            //return RedirectToAction("Display", "MusicScore");
        }
        #endregion
        #region 回复显示
        [ChildActionOnly]
        public ActionResult VidReply(int param1, int md = 1)
        {
            //var mdd = ViewData["md"];
            var se = TempData["Vi_idd"];
            ViewData["mscommentid"] = param1;
            int Vi_id1 = Convert.ToInt32(se);
            var msc = from m in db.VideoComment.Where(p => p.Vi_id == Vi_id1).OrderByDescending(p => p.Addtime) select m;
            var msr = (from n in db.VideoReply
                       join m in msc on n.Vi_commentid equals m.Vi_commentid
                       join q in db.Users on n.User_id equals q.User_id
                       select new VideoCommentReplyViewModel
                       {
                           Vi_replyid = n.Vi_replyid,
                           Vi_commentid = m.Vi_commentid,
                           content = n.content,
                           Addtime = n.Addtime,
                           Vi_id = m.Vi_id,
                           User_id = n.User_id,
                           User_name = q.User_name,
                           User_img = q.User_img
                       });
            var reply = (from m in msr.Where(p => p.Vi_commentid == param1).OrderByDescending(p => p.Addtime) select m);
            return PartialView(reply.ToPagedList(md, 5));
        }
        #endregion
        #region   点击量
        [HttpPost]
        public string AddClick(int ViId)
        {
            var ms = db.Video.Find(ViId);
            var mscount = ms.ReadCount + 1;
            ms.ReadCount = ms.ReadCount + 1;
            db.SaveChanges();
            return mscount.ToString();
        }
        #endregion
        // GET: Video/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = db.Video.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            ViewBag.User_id = new SelectList(db.Users, "User_id", "User_name", video.User_id);
            return View(video);
        }

        // POST: Video/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Vi_id,User_id,Vi_url,Vi_title,Vi_description,Vi_img,Vi_addtime")] Video video)
        {
            if (ModelState.IsValid)
            {
                db.Entry(video).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.User_id = new SelectList(db.Users, "User_id", "User_name", video.User_id);
            return View(video);
        }

        // GET: Video/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = db.Video.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // POST: Video/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Video video = db.Video.Find(id);
            db.Video.Remove(video);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
