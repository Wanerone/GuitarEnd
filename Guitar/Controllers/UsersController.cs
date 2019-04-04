using Guitar.Models;
using Guitar.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
//using BLL;
//using Model;

namespace Guitar.Controllers
{
    public class UsersController : Controller
    {
        private GuitarEntities db = new GuitarEntities();
        //public UsersBll usersBll = new UsersBll();
        // GET: Users
        public ActionResult Index(int? id,int nid = 1, int pageIndex = 1, int page = 1)
        {
            //var userid = 2;
            //if (id == null)
            //{
            //    id = Convert.ToInt32(Session["Users_id"].ToString());
            //}
            var user = db.Users.Find(id);
            var mscore = from m in db.MusicScore
                         join n in db.Users on m.User_id equals n.User_id
                         select new MusicUserViewModel
                         {
                             Ms_id = m.Ms_id,
                             Ms_title = m.Ms_title,
                             Ms_addtime = m.Ms_addtime,
                             Ms_label = m.Ms_label,
                             Ms_img = m.Ms_img,
                             User_id = n.User_id,
                             User_name = n.User_name,
                         };
            var mscore1 = from m in mscore.Where(p => p.User_id == id).OrderByDescending(p => p.Ms_addtime) select m;
            var mscount = mscore1.Count();
            ViewBag.mscount = mscount;
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
            var video1 = from m in video.Where(p => p.User_id == id).OrderByDescending(p => p.Vi_addtime) select m;
            var vicount = video1.Count();
            ViewBag.vicount = vicount;
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
            var post1 = from m in post.Where(p => p.User_id == id).OrderByDescending(p => p.Po_addtime) select m;
            var pocount = post1.Count();
            ViewBag.pocount = pocount;
            const int pageSize = 6;
            if (Request.IsAjaxRequest())
            {
                var target = Request.QueryString["target"];
                if (target == "articles")
                {
                    return PartialView("_AjaxArticles1",
                        mscore1.ToPagedList(nid, pageSize));
                }
                if (target == "articles2")
                {
                    return PartialView("_AjaxArticles2",
                        video1.ToPagedList(pageIndex, pageSize));
                }
                return PartialView("_AjaxArticles3",
                    post1.ToPagedList(page, pageSize));
            }
            var index = new Guitar.ViewModel.UsersViewModel()
            {
                Us = user,
                MS = mscore1.ToPagedList(nid, pageSize),
                VO = video1.ToPagedList(pageIndex, pageSize),
                PO = post1.ToPagedList(page, pageSize),
            };
            return View(index);
        }
        public ActionResult Headimg()
        {
            return View();
        }
        public ActionResult MyGuanzhu()
        {
            int id=Convert.ToInt32(Session["Users_id"].ToString());
            var fri = from m in db.Users join n in db.Friend on m.User_id equals n.UserA select m;
            return View(fri);
        }
        public ActionResult InfoSet()
        {
            if (Session["Users_id"] != null)
            {
                int id = Convert.ToInt32(Session["Users_id"]);
                var user = db.Users.Find(id);
                return View(user);
            }
            else
            {
                return Content("<script>;alert('未登录!');history.go(-1)</script>");
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavaInfo(Users user)
        {
            if(Session["Users_id"]!=null)
            {
                int id =Convert.ToInt32(Session["Users_id"]);
                user = db.Users.Find(id);
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                var birth = Request["birthday"];
                var address = Request["city"];
                var name = Request["usename"];
                var sign = Request["sign"];
                var sex = Request["browser"];
                //HttpPostedFileBase postimageBase = Request.Files["Image1"];

                if (ModelState.IsValid)
                {

                    //if (postimageBase != null)
                    //{
                    //    string filePath = postimageBase.FileName;
                    //    string filename = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                    //    string serverpath = Server.MapPath(@"/Images/headimg/") + filename;
                    //    string relativepath = @"/Images/headimg/" + filename;
                    //    postimageBase.SaveAs(serverpath);
                    //    user.User_img= relativepath;
                    //}

                    //else
                    //{
                    //    user.User_img = Session["User_img"].ToString();

                    //}
                    //user.User_name = name;
                    //user.User_sign = sign;
                    user.User_birthday = birth;
                    user.User_addr = address;
                    user.User_name = name;
                    user.User_sex = sex;
                    user.User_sign = sign;
                    //db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("<script>;alert('修改成功!');history.go(-1)</script>");
                }
                else
                {
                    return Content("<script>;alert('失败,请重试!');history.go(-1)</script>");
                }
            }
            else
            {
                return Content("<script>;alert('请先登录!');history.go(-1)</script>");
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavaHead(Users user)
        {
            int id = Convert.ToInt32(Session["Users_id"]);
            user = db.Users.Find(id);
            HttpPostedFileBase postimageBase = Request.Files["Image1"];
                    if (ModelState.IsValid)
                    {
                        if (postimageBase != null)
                        {
                            string filePath = postimageBase.FileName;
                            string filename = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                            string serverpath = Server.MapPath(@"/Images/headimg/") + filename;
                            string relativepath = @"/Images/headimg/" + filename;
                            postimageBase.SaveAs(serverpath);
                            user.User_img = relativepath;     
                            HttpContext.Session["User_img"]= user.User_img;
                        db.SaveChanges();
                         return Content("<script>;alert('修改成功!');history.go(-1)</script>");
                         }

                        else
                        {
                            return Content("<script>;alert('请先上传头像！');history.go(-1)</script>");

                        }
                        
                    }
                else
                {
                    return Content("<script>;alert('失败,请重试!');history.go(-1)</script>");
                }
        }
        //获得乐谱
        public ActionResult DisplayScore()
        {
          
            return View();
        }
        // GET: Users/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        public ActionResult Add()
        {
           
            return View();
        }
        [HttpPost]
        public ActionResult Add(Users users)
        {
            //usersBll.Add(users);
            return Redirect(Url.Action("Index"));
        }
        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Users/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Users/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
