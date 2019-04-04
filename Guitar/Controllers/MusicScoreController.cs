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
//using MvcPager;
using Webdiyer.WebControls.Mvc;
//using Guitar.Attributes;

namespace Guitar.Controllers
{
    public class MusicScoreController : Controller
    {
        private GuitarEntities db = new GuitarEntities();

        #region 吉他谱
        public ActionResult Index(int id = 1, int pageIndex = 1, int page = 1)
        {
            //var str1 = "古典吉他";
            var users = (from m in db.Users.OrderByDescending(p => p.User_addtime) select m).Take(6);

            var score1 = (from m in db.MusicScore.Where(p => p.Ms_label == "古典吉他").OrderByDescending(p => p.Ms_addtime) select m).Take(10);
            var score2 = (from m in db.MusicScore.Where(p => p.Ms_label == "民谣吉他").OrderByDescending(p => p.Ms_addtime) select m).Take(10);
            var score3 = (from m in db.MusicScore.Where(p => p.Ms_label == "电吉他").OrderByDescending(p => p.Ms_addtime) select m).Take(10);
            var score4 =( from m in db.MusicScore.Where(p => p.Ms_label == "尤克里里").OrderByDescending(p => p.Ms_addtime) select m).Take(10);
            var score5 = (from m in db.MusicScore.Where(p => p.Ms_label == "其他").OrderByDescending(p => p.Ms_addtime) select m).Take(10);
            var score6 = (from m in db.MusicScore.OrderByDescending(p => p.ReadCount) select m);
            const int pageSize = 20;
            if (Request.IsAjaxRequest())
            {
                var target = Request.QueryString["target"];
                if (target == "mscore")
                {
                    return PartialView("_Index1",score6.ToPagedList(id, pageSize));
                }
            }
          
            var index = new Guitar.ViewModel.MusicDetailsViewModel()
            {
                MScore1 = score1,
                MScore2 = score2,
                MScore3 = score3,
                MScore4 = score4,
                MScore5=score5,
                MScore6=score6.ToPagedList(page, pageSize),
                Us2 = users,
            };
            return View(index);
        }
        #endregion
        #region   点击量
        [HttpPost]
        public string AddClick(int MsId)
        {
            var ms = db.MusicScore.Find(MsId);
            var mscount= ms.ReadCount + 1;
            ms.ReadCount = ms.ReadCount + 1;
            db.SaveChanges();
            return mscount.ToString();
        }
        #endregion
        #region 发布乐谱
        // GET: MusicScores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MusicScores/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(MusicScore musicScore)
        {
            var sel1 = Request["sel"];
            var sel = sel1.Trim();
            HttpPostedFileBase postimageBase = Request.Files["Image1"];
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
                            musicScore.Ms_img = relativepath;
                        }
                        else
                        {
                            return Content("<script>;alert('请先上传封面！');history.go(-1)</script>");

                        }
                        musicScore.User_id = Convert.ToInt32(Session["Users_id"].ToString());
                        musicScore.Ms_addtime = System.DateTime.Now;
                        musicScore.Ms_label = sel;
                        db.MusicScore.Add(musicScore);
                        db.SaveChanges();
                        //return Content("<script>;alert('发布成功!');window.location.href='/Publish_Food/Index_PF'</script>");
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
        #region 删除乐谱
        // GET: MusicScore/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MusicScore musicScore = db.MusicScore.Find(id);
            if (musicScore == null)
            {
                return HttpNotFound();
            }
            return View(musicScore);
        }

        // POST: MusicScore/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MusicScore musicScore = db.MusicScore.Find(id);
            db.MusicScore.Remove(musicScore);
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
        #endregion
        #region  乐谱展示
        public ActionResult Display(int? id, int md = 1, int pageIndex = 1, int page = 1)
        {
            //const int pageSize = 5;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempData["Ms_idd"] = id;
            ViewBag.Msid= id;
            TempData["Ms_id2"] = id;
            MusicScore ms = db.MusicScore.Find(id);
            //MusicScoreStatistics msta = db.MusicScoreStatistics.Find(id);
            var usid = (from m in db.MusicScore.Where(p => p.Ms_id == id) select m.User_id).FirstOrDefault();
            Users us = db.Users.Find(usid);
            //MusicScoreStatistics msta = db.MusicScoreStatistics.Find(id);

            if (ms == null  && us == null)
            {
                return HttpNotFound();
            }

            //var us=from m in db.Users.Where(p=>p.User_id==usid) select m;
            var ms1 = (from m in db.MusicScore.Where(p => p.User_id == usid).OrderByDescending(p => p.Ms_addtime) select m).Take(5);
            var msc = from m in db.MusicScoreComment.Where(p => p.Ms_id == id).OrderByDescending(p => p.Addtime) select m;
            var comment = from m in db.MusicScoreComment
                          join n in db.Users on m.User_id equals n.User_id
                          select new MusicScoreCommentViewModel
                          {
                              Ms_commentid = m.Ms_commentid,
                              Ms_id = m.Ms_id,
                              content = m.content,
                              Addtime = m.Addtime,
                              User_id = n.User_id,
                              User_name = n.User_name,
                              User_img = n.User_img,
                          };
            //var comment1 = .ToPagedList(idd, pageSize);
            var msr = (from n in db.MusicScoreReply
                       join m in msc on n.Ms_commentid equals m.Ms_commentid
                       join q in db.Users on n.User_id equals q.User_id
                       select new MusicCommentReplyViewModel
                       {
                           Ms_replyid = n.Ms_replyid,
                           Ms_commentid = m.Ms_commentid,
                           content = n.content,
                           Addtime = n.Addtime,
                           Ms_id = m.Ms_id,
                           User_id = n.User_id,
                           User_name = q.User_name,
                           User_img = q.User_img
                       });
            int commentid = Convert.ToInt32(Request["Commentid1"]);
            var msrr = from m in db.MusicScoreReply.Where(p => p.Ms_commentid == commentid).OrderByDescending(p => p.Addtime) select m;
          
            var comment1 = from m in comment.Where(p => p.Ms_id == id).OrderByDescending(p => p.Addtime) select m;
            //var msta = from m in db.MusicScoreStatistics.Where(p => p.Ms_id == id) select m;
            //var msta = from m in db.MusicScoreStatistics where m.Ms_id == id select m;
            //分页数据
            const int pageSize = 5;
            if (Request.IsAjaxRequest())
            {
                var target = Request.QueryString["target"];
                if (target == "articles")
                {
                    return PartialView("_display1",comment1.ToPagedList(md, pageSize));
                }
            }
            var index = new Guitar.ViewModel.MusicDetailsViewModel()
            {
                Us = us,
                MScore = ms,
                MScore1 = ms1,
                MSC = msc,
                MSR = msr,
                Msr = msrr,
                msc11=comment1.ToPagedList(page, pageSize),
            };
            //if (Request.IsAjaxRequest())
            //    return PartialView("_PartialPage1", comment.OrderByDescending(a => a.Addtime).ToPagedList(md, 3));
            return View(index);
        }
        #endregion 

        #region 评论乐谱
        [HttpPost]
        //[Login]
        public string Comment(MusicScoreComment msc)
        {
            string pingluntextarea = Request["pingluntextarea"];
            int Ms_id = Convert.ToInt32(Request["Ms_id"]);
            if (pingluntextarea == "")
            {
                return "kong";
            }
            if (ModelState.IsValid)
            {
                msc.Ms_id = Ms_id;
                msc.content = pingluntextarea;
                msc.User_id = Convert.ToInt32(Session["Users_id"].ToString());
                msc.Addtime = System.DateTime.Now;
                db.MusicScoreComment.Add(msc);
                db.SaveChanges();
                //return Content("<script>;alert('评论成功!');");
                //return RedirectToAction("Display", "MusicScore");
                return "aa";
            }
            else {
                //return Content("bb");
                //return RedirectToAction("Display", "MusicScore");
                return "bb";
            }

            //return RedirectToAction("Display", "MusicScore");
        }
        #endregion
        #region 回复乐谱
        [HttpPost]
        public string Reply(MusicScoreReply msr)
        {
            string pingluntextarea = Request["pingluntextarea2"];
            var se = TempData["Ms_id2"];
            int Ms_id = Convert.ToInt32(se);
            //int Ms_id = Convert.ToInt32(Request["Ms_id2"]);
            int commentid = Convert.ToInt32(Request["Commentid"]);
            //if (pingluntextarea =="")
            //{
            //    return "kong";
            //}
            if (ModelState.IsValid)
            {
                msr.Ms_id = Ms_id;
                msr.content = pingluntextarea;
                msr.Ms_commentid = commentid;
                msr.User_id = Convert.ToInt32(Session["Users_id"].ToString());
                msr.Addtime = System.DateTime.Now;
                db.MusicScoreReply.Add(msr);
                db.SaveChanges();
                //return Content("<script>;alert('回复成功!');history.go(-1)</script>");
                return "aa";
            }
            else
            {
                //return Content("bb");
                //return RedirectToAction("Display", "MusicScore");
                return "bb";
            }
            //return RedirectToAction("Display", "MusicScore");
        }
        #endregion
        #region 收藏乐谱
        [HttpPost]
        public ActionResult Collection(MusicScoreCollection msc)
        {
            if (Session["Users_id"] != null)
            {
                     int Ms_id = Convert.ToInt32(Request["Ms_id4"]);
                    int User_id = Convert.ToInt32(Session["Users_id"]);
                    var msc1=db.MusicScoreCollection.Where(o => o.User_id==User_id).Where(o => o.Ms_id== Ms_id).FirstOrDefault();
                    if (msc1 == null)
                    {
                        if (ModelState.IsValid)
                                    {
                                        msc.Ms_id = Ms_id;
                                        msc.User_id = User_id;
                                        msc.State = 1;
                                        db.MusicScoreCollection.Add(msc);
                                        db.SaveChanges();
                                        var collection = db.MusicScore.Find(Ms_id);
                                        collection.Collection = collection.Collection + 1;
                                        db.SaveChanges();
                            return Content("<script>;alert('收藏成功!');history.go(-1)</script>");
                      
                    }
                    }
                    else
                    {
                        if (msc1.State == 0)
                        {
                            var collection = db.MusicScore.Find(Ms_id);
                            collection.Collection = collection.Collection + 1;
                            db.SaveChanges();
                            msc1.State = 1;
                            db.SaveChanges();
                            return Content("<script>;alert('收藏成功!');history.go(-1)</script>");
                        }
                        if (msc1.State == 1)
                        {
                            var collection = db.MusicScore.Find(Ms_id);
                            collection.Collection = collection.Collection - 1;
                            db.SaveChanges();
                            msc1.State = 0;
                            db.SaveChanges();
                            //return Content("<script>;alert('收藏成功!');history.go(-1)</script>");
                            //return Content("<script>alert('请收藏成功');window.location.href='~/MusicScore/Display';</script>");
                            return Content("<script>;alert('取消收藏成功!');history.go(-1)</script>");
                        }
                    }
            }
            else
            {
                return Content("<script>;alert('请先登录!');history.go(-1)</script>");
            }
            
            
            //UpdateModel(owner); 
            //更新submit所有字段db.SaveChanges();
            return RedirectToAction("Display", "MusicScore");
        }
        #endregion
        #region 回复显示
        [ChildActionOnly]
        public ActionResult MsReply(int param1, int md = 1)
        {
            //var mdd = ViewData["md"];
            var se = TempData["Ms_idd"];
            ViewData["mscommentid"] = param1;
            int Ms_id1 = Convert.ToInt32(se);
            var msc = from m in db.MusicScoreComment.Where(p => p.Ms_id == Ms_id1).OrderByDescending(p => p.Addtime) select m;
            var msr = (from n in db.MusicScoreReply
                       join m in msc on n.Ms_commentid equals m.Ms_commentid
                       join q in db.Users on n.User_id equals q.User_id
                       select new MusicCommentReplyViewModel
                       {
                           Ms_replyid = n.Ms_replyid,
                           Ms_commentid = m.Ms_commentid,
                           content = n.content,
                           Addtime = n.Addtime,
                           Ms_id = m.Ms_id,
                           User_id = n.User_id,
                           User_name = q.User_name,
                           User_img = q.User_img
                       });
            var reply = (from m in msr.Where(p=>p.Ms_commentid== param1).OrderByDescending(p => p.Addtime) select m);
            return PartialView(reply.ToPagedList(md,5));
        }
        #endregion
        #region 评论展示
        public ActionResult GetAllComment(int id = 1)
        {
            var se = TempData["Ms_idd"];
            int Ms_id1 = Convert.ToInt32(se);
            ViewBag.Ms_id = Ms_id1;
            var comment = from m in db.MusicScoreComment
                            join n in db.Users on m.User_id equals n.User_id
                            select new MusicScoreCommentViewModel
                            {
                                Ms_commentid = m.Ms_commentid,
                                Ms_id = m.Ms_id,
                                content = m.content,
                                Addtime = m.Addtime,
                                User_id = n.User_id,
                                User_name = n.User_name,
                                User_img = n.User_img,
                            };
            var comment1 = from o in comment.Where(p=>p.Ms_id==Ms_id1).OrderByDescending(p => p.Addtime) select o;
            //int pageSize = 3;
            //int pageNumber = (page ?? 1);
            //return PartialView(comment1.ToPagedList(pageNumber, pageSize));
            //var model = comment.OrderByDescending(a => a.Addtime).ToPagedList(id, 5);
            //if (Request.IsAjaxRequest())
            //    return PartialView("_PartialPage2", model);
            //return View(model);
            return View(comment.OrderByDescending(a => a.Addtime).ToPagedList(id, 5));
        }
        #endregion
        public ActionResult UserIndex(int User_Id)
        {
            Users users = db.Users.Find(User_Id);
            var index = new Guitar.ViewModel.UsersViewModel
            {
                Us = users,

            };

            return View(index);
        }

    }
}
