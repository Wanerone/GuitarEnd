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
    public class PostsController : Controller
    {
        private GuitarEntities db = new GuitarEntities();

        // GET: Posts
        public ActionResult Index()
        {
            var post = db.Post.Include(p => p.Users);
            return View(post.ToList());
        }
        #region 帖子详情
        // GET: Posts/Details/5
        public ActionResult Details(int? id, int md = 1, int pageIndex = 1, int page = 1)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var vid = db.Post.Find(id);
            var user = db.Users.Find(vid.User_id);
            if (vid == null && user == null)
            {
                return HttpNotFound();
            }
            var vid1 = (from m in db.Post.Where(p => p.User_id == vid.User_id).OrderByDescending(p => p.Po_addtime) select m).Take(8);
            TempData["Vi_id2"] = id;
            TempData["Vi_idd"] = id;
            if (Session["Users_id"] != null)
            {
                var userid = Convert.ToInt32(Session["Users_id"]);
                var coll = (from m in db.PostCollection.Where(p => p.User_id == userid).Where(p => p.Po_id == id) select m).FirstOrDefault();
                if (coll != null && coll.State == 1)
                {
                    ViewBag.rel = "unlike";
                }
                else
                {
                    ViewBag.rel = "like";
                }
                var distribute = (from m in db.Friend.Where(p => p.UserA == userid) select m).FirstOrDefault();
                if (distribute != null)
                {
                    if (distribute.UserB == vid.User_id)
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
            var comment = from m in db.PostComment.Where(p => p.Po_id == id).OrderByDescending(p => p.Addtime) select m;
            var comment1 = from m in db.PostComment
                           join n in db.Users on m.User_id equals n.User_id
                           select new PostCommentViewModel
                           {
                               Po_commentid = m.Po_commentid,
                               Po_id = m.Po_id,
                               content = m.content,
                               Addtime = m.Addtime,
                               User_id = n.User_id,
                               User_name = n.User_name,
                               User_img = n.User_img,
                           };
            //var comment1 = .ToPagedList(idd, pageSize);
            var vidreply = (from n in db.PostReply
                            join m in comment1 on n.Po_commentid equals m.Po_commentid
                            join q in db.Users on n.User_id equals q.User_id
                            select new PostCommentReplyViewModel
                            {
                                Po_replyid = n.Po_replyid,
                                Po_commentid = m.Po_commentid,
                                content = n.content,
                                Addtime = n.Addtime,
                                Po_id = m.Po_id,
                                User_id = n.User_id,
                                User_name = q.User_name,
                                User_img = q.User_img
                            });
            var vidcomment = from m in comment1.Where(p => p.Po_id == id).OrderByDescending(p => p.Addtime) select m;
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
            var index = new Guitar.ViewModel.PostDetailsViewModel()
            {
                Us = user,
               Po= vid,
                Po1 = vid1,
                Pocomment = vidcomment.ToPagedList(page, pageSize),
                Poreply = vidreply,
            };
            return View(index);
        }
        #endregion
        #region 发布帖子
        // GET: Posts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Post post)
        {
            var label= Request["label"];
            HttpPostedFileBase postimageBase = Request.Files["Image1"];
            //HttpFileCollectionBase files = Request.Files;
            //HttpPostedFileBase postimageBase = files["Image1"];//获取上传的文件
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
                            post.Po_img = relativepath;
                        }

                        else
                        {
                            return Content("<script>;alert('请先上传封面！');history.go(-1)</script>");

                        }
                        post.User_id = Convert.ToInt32(Session["Users_id"].ToString());
                        post.Po_addtime = System.DateTime.Now;
                        post.Po_label = label;
                        db.Post.Add(post);
                        db.SaveChanges();
                        //return Content("<script>;alert('发布成功!');window.location.href='/Publish_Food/Index_PF'</script>");
                        return Content("<script>;alert('发布成功!');history.go(-1)</script>");
                    }
                    else
                    {
                        return Content("<script>;alert('发布失败');history.go(-1)</script>");
                    }
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }

            return View();
            //if (ModelState.IsValid)
            //{
            //    db.Post.Add(post);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            //ViewBag.User_id = new SelectList(db.Users, "User_id", "User_name", post.User_id);
            //return View(post);
        }
        #endregion
        #region   点击量
        [HttpPost]
        public string AddClick(int PoId)
        {
            var ms = db.Post.Find(PoId);
            var mscount = ms.ReadCount + 1;
            ms.ReadCount = ms.ReadCount + 1;
            db.SaveChanges();
            return mscount.ToString();
        }
        #endregion
        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Post.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.User_id = new SelectList(db.Users, "User_id", "User_name", post.User_id);
            return View(post);
        }

        // POST: Posts/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Po_id,User_id,Po_title,Po_content,Po_img,Po_addtime,Po_label")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.User_id = new SelectList(db.Users, "User_id", "User_name", post.User_id);
            return View(post);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Post.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Post.Find(id);
            db.Post.Remove(post);
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
