using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using Guitar.Models;
using System.Data.Entity.Validation;
using System.Text;
using Guitar.ViewModel;

namespace Guitar.Controllers
{
    public class LoginRegisterController : Controller
    {
        // GET: LoginRegister
        GuitarEntities db = new GuitarEntities();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult LogOff()
        {
            Session["User_name"] = null;

            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Users user, string returnUrl)
        {
            string ValidateCode = Request.Form["txtverifcode"];
            if (ValidateCode != Session["ValidateCode"].ToString())
            {
                return Content("<script>;alert('验证码错误！');history.go(-1)</script>");
            }
            try
            {
                var users = db.Users.Where(o => o.User_email == user.User_email).FirstOrDefault();
                var user2 = db.Users.Where(o => o.User_email == user.User_email).Where(o => o.User_password == user.User_password).FirstOrDefault();
                if (users == null)
                {
                    return Content("<script>;alert('该账号不存在!');history.go(-1)</script>");
                }
                if (user2== null)
                {
                    return Content("<script>;alert('密码不正确!');history.go(-1)</script>");
                    //以下代码将权限保存到Session
                    // var current_user = db.Users.Where(o => o.UserName == user.UserName).FirstOrDefault();
                    

                }
                else
                {
                    //return Content("<script>;alert('该账号不存在!');history.go(-1)</script>");
                    HttpContext.Session["Users_id"] = users.User_id;
                    HttpContext.Session["User_name"] = users.User_name;
                    HttpContext.Session["User_email"] = users.User_email;
                    HttpContext.Session["User_img"] = users.User_img;

                    return Content("<script>;alert('登录成功!返回首页!');window.location.href='/Home/Index'</script>");
                }

                //}
                //else
                //{

                //}

                //return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Users userInfo, HttpPostedFileBase file)
        {
            string checkPwd = Request.Form["ChkUserPwd"];
            string vCode = Request.Form["txtverifcode1"];

            if (string.IsNullOrEmpty(checkPwd))
            {
                ModelState.AddModelError("ChkUserPwd", "确认密码不能为空");
            }
            else
            {
                if ((checkPwd) != (userInfo.User_password).ToString())
                {
                    ModelState.AddModelError("PwdRepeatError", "确认密码不正确");
                }
            }
            if (vCode != Session["ValidateCode"].ToString())
            {
                return Content("<script>;alert('验证码错误！');history.go(-1)</script>");
            }
            var user1 = from m in db.Users
                        select new UserViewModel
                        {
                            User_id = m.User_id,
                            User_email = m.User_email,
                            User_name = m.User_name,
                            User_password = m.User_password,
                            User_img = m.User_img,
                        };
            var isUserExists = user1.Where(a => a.User_name == userInfo.User_name).FirstOrDefault();
            var isEmailExists = user1.Where(a => a.User_email == userInfo.User_email).FirstOrDefault();

            if (isUserExists != null)
                return Content("<script>;alert('用户名已被占用！');history.go(-1)</script>");
            if (isEmailExists != null)
                return Content("<script>;alert('邮箱已被注册！');history.go(-1)</script>");
            userInfo.User_addtime = DateTime.Now;
            userInfo.User_password = userInfo.User_password;
            userInfo.User_img = "/Images/headimg/head.jpg";
            try
            {
                db.Users.Add(userInfo);
                db.SaveChanges();
                HttpContext.Session["Users_id"] = userInfo.User_id;
                HttpContext.Session["User_name"] = userInfo.User_name;
                HttpContext.Session["User_email"] = userInfo.User_email;
                HttpContext.Session["User_img"] = userInfo.User_img;
                return Content("<script>;alert('注册成功!正在跳转到首页..');window.location.href='/Home/Index'</script>");
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Trace.TraceInformation("Property: {0} Error: {1}",
                        validationError.PropertyName,
                        validationError.ErrorMessage);
                    }
                }
                throw;
            }
        }
    }
}