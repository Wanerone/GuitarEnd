using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Guitar
{
    public class MVCSiteMap
    {
        /// <summary>
        ///获取siteMap 
        /// </summary>
        /// <param name="CurrentPageUrl">当前页面url，区分大小写，也可以是 Request.Url,但格式要处理好</param>
        /// <returns>返回一串html代码，直接读取到html页面就可以了</returns>
        public static string GetMvcSiteMap(string CurrentPageUrl)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(HttpContext.Current.Server.MapPath("~/Web.sitemap"));//加载导航xml
            DataRow[] drs = ds.Tables[0].Select("url='" + CurrentPageUrl + "'");//根据url寻找当前节点
            int rowIndex = drs.Count();//考虑到父节点也有可能同样的url,所以打算取最下面的节点
            string url = "";
            if (rowIndex > 0)
            {
                rowIndex = rowIndex - 1;
                url = urlLink(ds.Tables[0], drs[rowIndex]["siteMapNode_Id_0"].ToString()) + string.Format("<a>{0}</a>", drs[rowIndex]["title"]);
            }
            return url;
        }
        /// <summary>
        /// 递归当前节点的父节点
        /// </summary>
        private static string urlLink(DataTable dt, string parentid)
        {
            string Html = "";
            if (!string.IsNullOrEmpty(parentid))
            {
                DataRow[] drs = dt.Select("siteMapNode_Id=" + parentid);
                if (drs.Count() > 0)
                {
                    string url = drs[0]["url"].ToString();
                    if (!string.IsNullOrEmpty(url))
                    {
                        url = string.Format("href='{0}'", url);
                    }
                    Html = string.Format("<a {0}>{1}</a>>", url, drs[0]["title"]);
                    Html = urlLink(dt, drs[0]["siteMapNode_Id_0"].ToString()) + Html;
                }
            }
            return Html;
        }
    }
}