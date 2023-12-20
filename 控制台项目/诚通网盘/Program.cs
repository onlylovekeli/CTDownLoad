using ctdisk;
using Newtonsoft.Json.Linq;
using System.Net;

namespace 诚通网盘
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("请输入下载路径：");
            string filepath = Console.ReadLine();   
            Console.Write("请默认密码：");
            string defaultPW=Console.ReadLine();
            while(true)
            {
                Console.Write("链接:");
                string url = Console.ReadLine();
                // string url = " https://url74.ctfile.com/f/26012674-498377278-7d5fc8?p=6195";
                string[] parseUrl = url.Split('/');
                string[] parms = parseUrl[parseUrl.Length - 1].Split('?');
                string file = parms[0];
                Console.Write("密码:");
                string pass ="";
                if(string.IsNullOrEmpty(defaultPW))
                {
                    if (parms.Length == 2)
                    {
                        pass = parms[1].Split('=')[1];
                    }
                    else
                    {
                        pass = Console.ReadLine();
                    }
                }
                else
                {
                    pass = defaultPW;
                }
                Console.WriteLine(pass);
                CTDisk disk = new CTDisk("");
                JObject data = disk.ParseLink(file, pass);
                if (data["success"].ToString() == "1")
                {
                    Console.WriteLine("开始下载："+data["name"].ToString()+" 大小：" +int.Parse(data["size"].ToString())/1024+"KB");
                    Console.WriteLine("下载地址："+ data["downurl"].ToString());
                    CTDisk.DownFile(data["downurl"].ToString(), data["name"].ToString(), filepath);
                }
                else
                {
                    Console.WriteLine(data["message"].ToString());
                }
            } 
        }
        static int rep = 0;
        static string GenerateCheckCode(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }
    }
}
