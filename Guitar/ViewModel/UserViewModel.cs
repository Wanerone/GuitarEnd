using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Guitar.ViewModel
{
    public class UserViewModel
    {
        public int User_id { get; set; }
        [Required(ErrorMessage = "用户名不为空")]
        [MaxLength(8, ErrorMessage = "昵称不超过8个字符!")]
        public string User_name { get; set; }
        [Required(ErrorMessage = "密码必填!")]
        [Display(Name = "密码")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密码必须至少包含6个字符!")]
        [DataType(DataType.Password)]
        public string User_password { get; set; }
        [Display(Name = "确认密码")]
        [Required(ErrorMessage = "确认密码必填!")]
        [DataType(DataType.Password)]
        [Compare("User_password", ErrorMessage = "密码和确认密码不正确!")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "邮箱")]
        [Required(ErrorMessage = "邮箱必填!")]
        [RegularExpression(@"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$", ErrorMessage = "请输入正确的Email格式\n示例：abc@123.com")]
        public string User_email { get; set; }
        public string User_img { set; get; }
        public bool RememberMe { get; set; }
    }
}