using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

using Snowball.Common;
using Tools;
using ToolsCXml;

namespace Tools
{
    public class CWeb
    {
        public CWeb()
        {
            strCookies = "";
            charset = "utf-8";
            charsettags = new BETags("[<head>-<title>][charset=->]");
        }

        public string GetOKUrl(string Url)
        {
            //if (charset == "utf-8")
            //    return GetWebClient(Url);
            return GetWebRequest_Charset(ref charset, Url);
        }
        public string GetWebRequest_Charset(ref string charset, string url)
        {
            string txt = GetWebRequest(url, charset);
            string cs = charsettags.Match(txt);
            if (cs.Contains("\""))
                cs = cs.Substring(0, cs.IndexOf("\""));
            if (cs == "")
            {
                charsettags = new BETags("[</title>-</head>][charset=->]");
                cs = charsettags.Match(txt);
            }
            if (cs == "")
            {
                charsettags = new BETags("[<head>-</head>][charset=->]");
                cs = charsettags.Match(txt);
            }
            if (cs!="" && !cs.Contains(charset))
            {
                if (!ValidTools.ValidName(cs))
                {
                    if (cs.StartsWith("\""))
                        cs = cs.Substring(1, cs.IndexOf('\"') - 1);
                    if (cs.StartsWith("'"))
                        cs = cs.Substring(1, cs.IndexOf('\'') - 1);
                    if (cs.Contains("\""))
                        cs = cs.Substring(0, cs.IndexOf('\"'));
                    if (cs.Contains("'"))
                        cs = cs.Substring(0, cs.IndexOf('\''));
                    if (cs.EndsWith("/"))
                        cs = cs.Substring(0,cs.IndexOf('/')).Trim();

                }
                if (ValidTools.ValidName(cs))
                {
                    charset = cs;
                    txt = GetWebRequest(url, charset);
                }
            }
            return txt;
        }
        public string GetHttpWebRequest(string url, out string strCookies)
        {
            Uri uri = new Uri(url);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(uri);
            myReq.UserAgent = "User-Agent:Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705";
            myReq.Accept = "*/*";
            myReq.KeepAlive = true;
            myReq.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            HttpWebResponse result = (HttpWebResponse)myReq.GetResponse();
            Stream receviceStream = result.GetResponseStream();
            StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("utf-8"));
            string strHTML = readerOfStream.ReadToEnd();
            readerOfStream.Close();
            receviceStream.Close();
            strCookies = FenxiCookie(result.Headers["Set-Cookie"]);
            result.Close();
            return strHTML;
        }
        public string GetHttpWebRequest(string url, string strCookies)
        {
            if (wl > 0)
            {
                strCookies += "WL=" + wl + ";";
            }
            Uri uri = new Uri(url);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(uri);
            myReq.UserAgent = "User-Agent:Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705";
            myReq.Accept = "*/*";
            myReq.KeepAlive = true;
            myReq.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            myReq.Headers.Add("Cookie", strCookies);
            HttpWebResponse result = (HttpWebResponse)myReq.GetResponse();
            Stream receviceStream = result.GetResponseStream();
            StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("utf-8"));
            string strHTML = readerOfStream.ReadToEnd();
            readerOfStream.Close();
            receviceStream.Close();
            wl = FenxiCookieWl(result.Headers["Set-Cookie"]);
            result.Close();
            return strHTML;
        }
        public static string GetWebClient(string url,string charset = "gb2312")
        {
            string strHTML = "";
            WebClient myWebClient = new WebClient();
            Stream myStream = myWebClient.OpenRead(url);
            StreamReader sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding(charset));
            strHTML = sr.ReadToEnd();
            myStream.Close();
            return strHTML;
        }
        public static string GetWebRequest(string url, string charset = "utf-8")
        {
            Uri uri = new Uri(url);
            WebRequest myReq = WebRequest.Create(uri);
            WebResponse result = myReq.GetResponse();
            Stream receviceStream = result.GetResponseStream();
            StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding(charset));
            string strHTML = readerOfStream.ReadToEnd();

            readerOfStream.Close();
            receviceStream.Close();
            result.Close();
            return strHTML;
        }
        public int FenxiCookieWl(string cookie)
        {
            if (cookie == null)
                return this.wl;
            int wl = Convert.ToInt32(StringTools.GetEqualValue(cookie, "WL=", ";"));
            return wl;
        }
        public string FenxiCookie(string cookie)
        {
            string ret = "jyean=" + StringTools.GetEqualValue(cookie, "jyean=", ";") + ";";
            return ret;
        }
        public static string PostHtml(string url, ref string strCookies)
        {
            PostSubmitter post = new PostSubmitter();
            System.Net.ServicePointManager.Expect100Continue = false;
            post.Url = url;
            post.Type = PostSubmitter.PostTypeEnum.Post;
            // 加入cookies，必须是这么写，一次性添加是不正确的。
            post.strCookies = strCookies;
            string ret = post.Post();
            strCookies = post.strCookies;
            return ret;
        }
        public static void DownLoadFile(String url, String FileName)
        {
            try
            {
                FileStream outputStream = new FileStream(FileName, FileMode.Create);
                WebRequest request = WebRequest.Create(url);
                request.Timeout = 3000;
                WebResponse wr = (HttpWebResponse)request.GetResponse();
                Stream httpStream = wr.GetResponseStream();
                httpStream = DealStreamZip(wr, httpStream);

                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = httpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);

                    readCount = httpStream.Read(buffer, 0, bufferSize);
                }
                httpStream.Close();
                outputStream.Close();
            }
            catch (Exception ex)
            {
                FileStream outputStream = new FileStream(Application.StartupPath + @"\downloaderror.log", FileMode.Append);
                StreamWriter sw = new StreamWriter(outputStream);
                String s = url + "\t" + FileName + "\t 文件下载失败错误为" + ex.Message.ToString() + "\r\n";
                sw.Write(s);
                sw.Close();
                outputStream.Close();
                //return "";
                //MessageBox.Show("文件下载失败错误为" + ex.Message.ToString(), "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public static Stream DealStreamZip(WebResponse wr, Stream httpStream)
        {
            if (wr.Headers["Content-Encoding"] == "gzip")//gzip解压处理
            {
                MemoryStream msTemp = new MemoryStream();
                GZipStream gzs = new GZipStream(httpStream, CompressionMode.Decompress);
                byte[] buf = new byte[1024];
                int len;
                while ((len = gzs.Read(buf, 0, buf.Length)) > 0)
                {
                    msTemp.Write(buf, 0, len);
                }
                msTemp.Position = 0;
                httpStream = msTemp;
            }
            else if (wr.Headers["Content-Encoding"] == "deflate")//gzip解压处理
            {
                MemoryStream msTemp = new MemoryStream();
                DeflateStream gzs = new DeflateStream(httpStream, CompressionMode.Decompress);
                byte[] buf = new byte[1024];
                int len;
                while ((len = gzs.Read(buf, 0, buf.Length)) > 0)
                {
                    msTemp.Write(buf, 0, len);
                }
                msTemp.Position = 0;
                httpStream = msTemp;
            }
            return httpStream;
        }
        public static bool DownGetWebRequest(string url, string FileName)//DownLoad  //for Del
        {
            try
            {
                string ext = Path.GetExtension(FileName);
                Uri uri = new Uri(url);
                WebClient wb = new WebClient();
                wb.DownloadFile(uri, FileName);
            }
            catch
            {
                return false;
            }
            return true;
        }
        
        public string strCookies;
        public int wl;

        private string charset;
        private BETags charsettags;
    }
}
