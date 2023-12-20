using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml;
using static System.Net.WebRequestMethods;

namespace ctdisk
{
    public class CTDisk
    {
        string token = "0";
        public CTDisk(string token="0") 
        { 
            this.token = token;
        }
        /// <summary>
        /// 解析下载直链
        /// </summary>
        /// <param name="file">文件id</param>
        /// <param name="pass">文件密码</param>
        /// <returns></returns>
        public JObject ParseLink(string file = "", string pass = "")
        {
            return GetFile(file, pass);
        }
        JObject GetFile(string fileid, string passcode)
        {
            JObject result = null;
            string path = fileid.Split('-').Length == 2 ? "file" : "f";
            string url = "https://webapi.ctfile.com/getfile.php?path="+ path+"&f="+fileid+ "&passcode="+passcode+ "&token="+ token+"&r="+new Random().NextDouble()+"&ref=https://ctfile.qinlili.workers.dev";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8"));
            string content = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            JObject data=JsonConvert.DeserializeObject<JObject>(content);
            if (data["code"].ToString()== "200")
            {
                result= GetFileUrl(data["file"]["userid"].ToString(), data["file"]["file_id"].ToString(), data["file"]["file_chk"].ToString());
            }
            return result;
        }
        JObject GetFileUrl(string uid, string fid,string file_chk)
        {
            JObject result=new JObject();
            string url = "https://webapi.ctfile.com/get_file_url.php?uid=" + uid + "&fid=" + fid + "&file_chk=" + file_chk + "&app=0&acheck=2&rd="+ new Random().NextDouble();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8"));
            string content = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            JObject data = JsonConvert.DeserializeObject<JObject>(content);
            if (data["code"].ToString() == "200")
            {
                result["name"]= data["file_name"].ToString();
                result["downurl"] = data["downurl"].ToString();
                result["size"] = data["file_size"].ToString();
                result["success"]="1";
            }
            else if (data["code"].ToString() == "302")
            {
                result["success"] = "0";
                result["message"] = "需要登录";
            }
            else
            {
                result["success"] = "0";
                result["message"] = data["message"].ToString();
            }
            return result;
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="filename">文件名</param>
        /// <param name="filepath">本地文件夹</param>
        public static void DownFile(string url,string filename,string filepath,Action<int> progressChanged=null,Action completed=null)
        {
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            WebClient client = new WebClient();
            client.DownloadProgressChanged += (s, e) =>
            {
                if(progressChanged != null)
                {
                    progressChanged(e.ProgressPercentage);
                }
            };
            client.DownloadFileCompleted += (s, e) => {
                if(completed != null)
                {
                    completed();
                }
            };
            client.DownloadFileAsync(new Uri(url), Path.Combine(filepath, filename));
        }
    }
}
