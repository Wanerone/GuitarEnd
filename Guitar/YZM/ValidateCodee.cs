﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.UI;
using System.Drawing.Drawing2D;
using System.IO;

namespace Guitar.YZM
{
    public class ValidateCode
    {
        public ValidateCode()
        {
        }
        /// <summary>
        /// 验证码的最大长度
        /// </summary>
        public int MaxLength
        {
            get { return 10; }
        }
        /// <summary>
        /// 验证码的最小长度
        /// </summary>
        public int MinLength
        {
            get { return 1; }
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="length">指定验证码的长度</param>
        /// <returns></returns>
        public string CreateValidateCode(int length)
        {
            #region 生成数字
            //int[] randMembers = new int[length];
            //int[] validateNums = new int[length];
            //string validateNumberStr = "";
            ////生成起始序列值
            //int seekSeek = unchecked((int)DateTime.Now.Ticks);
            //Random seekRand = new Random(seekSeek);
            //int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - length * 10000);
            //int[] seeks = new int[length];
            //for (int i = 0; i < length; i++)
            //{
            //    beginSeek += 10000;
            //    seeks[i] = beginSeek;
            //}
            ////生成随机数字
            //for (int i = 0; i < length; i++)
            //{
            //    Random rand = new Random(seeks[i]);
            //    int pownum = 1 * (int)Math.Pow(10, length);
            //    randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            //}
            ////抽取随机数字
            //for (int i = 0; i < length; i++)
            //{
            //    string numStr = randMembers[i].ToString();
            //    int numLength = numStr.Length;
            //    Random rand = new Random();
            //    int numPosition = rand.Next(0, numLength - 1);
            //    validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            //}
            ////生成验证码
            //for (int i = 0; i < length; i++)
            //{
            //    validateNumberStr += validateNums[i].ToString();
            //}
            //return validateNumberStr;

            #endregion

            char[] constant =   
            {   
            '0','1','2','3','4','5','6','7','8','9',  
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',   
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'   
            };
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }

        #region 传统 asp.net
        /// <summary>
        /// 创建验证码的图片
        /// </summary>
        /// <param name="containsPage">要输出到的page对象</param>
        /// <param name="validateNum">验证码</param>
        //public void CreateValidateGraphic(string validateCode)
        //{
        //    Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 12.0), 22);
        //    Graphics g = Graphics.FromImage(image);
        //    try
        //    {
        //        //生成随机生成器
        //        Random random = new Random();
        //        //清空图片背景色
        //        g.Clear(Color.White);
        //        //画图片的干扰线
        //        for (int i = 0; i < 25; i++)
        //        {
        //            int x1 = random.Next(image.Width);
        //            int x2 = random.Next(image.Width);
        //            int y1 = random.Next(image.Height);
        //            int y2 = random.Next(image.Height);
        //            g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
        //        }
        //        Font font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
        //        LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
        //         Color.Blue, Color.DarkRed, 1.2f, true);
        //        g.DrawString(validateCode, font, brush, 3, 2);
        //        //画图片的前景干扰点
        //        for (int i = 0; i < 100; i++)
        //        {
        //            int x = random.Next(image.Width);
        //            int y = random.Next(image.Height);
        //            image.SetPixel(x, y, Color.FromArgb(random.Next()));
        //        }
        //        //画图片的边框线
        //        g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
        //        //保存图片数据
        //        MemoryStream stream = new MemoryStream();
        //        image.Save(stream, ImageFormat.Jpeg);
        //        //输出图片流
        //        containsPage.Response.Clear();
        //        containsPage.Response.ContentType = "image/jpeg";
        //        containsPage.Response.BinaryWrite(stream.ToArray());
        //    }
        //    finally
        //    {
        //        g.Dispose();
        //        image.Dispose();
        //    }
        //}
        #endregion

        /// <summary>
        /// 得到验证码图片的长度
        /// </summary>
        /// <param name="validateNumLength">验证码的长度</param>
        /// <returns></returns>
        public static int GetImageWidth(int validateNumLength)
        {
            return (int)(validateNumLength * 12.0);
        }
        /// <summary>
        /// 得到验证码的高度
        /// </summary>
        /// <returns></returns>
        public static double GetImageHeight()
        {
            return 22.5;
        }

        /// <summary>
        /// 创建验证码的图片
        /// </summary>
        /// <param name="containsPage">要输出到的page对象</param>
        /// <param name="validateNum">验证码</param>
        public byte[] CreateValidateGraphic(string validateCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 20.0), 30);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                Font font = new Font("Arial", 17, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                 Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);
                //画图片的前景干扰点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
        ///// <summary>
        ///// 入口方法
        ///// </summary>
        ///// <param name="length">验证码长度</param>
        ///// <returns></returns>
        //public static byte[] Factory(int length)
        //{
        //   string code = CreateValidateCode(length);
        //   return CreateValidateGraphic(code);
        //}
    }
}