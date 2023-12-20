using ctdisk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace 城通下载器
{
    public partial class Form1 : Form
    {
        private List<FileItem> fileItems = new List<FileItem>();
        public Form1()
        {
            InitializeComponent();
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting setting = new Setting();
            setting.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "config.json");
            if (!File.Exists(path))
            {
                MessageBox.Show("请先设置默认下载路径");
                JObject cfg = new JObject();
                cfg["path"] = "";
                File.WriteAllText(path, JsonConvert.SerializeObject(cfg));
                return;
            }
            oldcfg = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));
            listView1.Items.Clear();
            fileItems.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("链接不能为空");
                return;
            }
            string path = Path.Combine(Application.StartupPath, "config.json");
            if (!File.Exists(path))
            {
                MessageBox.Show("请先设置默认下载路径");
                return;
            }
            if (string.IsNullOrEmpty(oldcfg["path"].ToString()))
            {
                MessageBox.Show("请先设置默认下载路径");
                return;
            }
            AddUrl(textBox1.Text,textBox2.Text);
        }
        JObject oldcfg;
        void AddUrl(string url,string pass)
        {
            string[] parseUrl = url.Split('/');
            string[] parms = parseUrl[parseUrl.Length - 1].Split('?');
            string file = parms[0];
            CTDisk cTDisk = new CTDisk();
            JObject data = cTDisk.ParseLink(file, pass);
            if (data != null)
            {
                if (data["success"].ToString() == "1")
                {
                    FileItem fileItem = new FileItem();
                    fileItem.Name = data["name"].ToString();
                    fileItem.Path = oldcfg["path"].ToString();
                    fileItem.Size = (int.Parse(data["size"].ToString()) / 1024) + "KB";
                    fileItem.DownUrl = data["downurl"].ToString();
                    fileItem.DownLoadState = DownLoadState.Wait;
                    fileItems.Add(fileItem);
                    ListViewItem item = new ListViewItem();
                    item.Name = fileItem.Name;
                    item.Text = fileItem.Name;
                    item.SubItems.Add(fileItem.Size);
                    item.SubItems.Add("等待下载");
                    item.SubItems.Add(Path.Combine(fileItem.Path, fileItem.Name));
                    listView1.Items.Add(item);
                    UpdateList();
                }
                else
                {
                    MessageBox.Show("解析失败无法添加");
                }
            }
        }
        void UpdateList()
        {
            for (int i = 0; i < fileItems.Count; i++)
            {
                FileItem item = fileItems[i];
                if (item.DownLoadState == DownLoadState.Wait)
                {
                    item.DownLoadState = DownLoadState.Down;
                    listView1.Items[i].SubItems[2].Text = "正在下载";
                    CTDisk.DownFile(item.DownUrl, item.Name, item.Path, (progress) =>
                    {
                        item.DownLoadState = DownLoadState.Down;
                        listView1.Items[i].SubItems[2].Text = progress+"%";
                    } ,() =>
                    {
                        FileInfo fileInfo = new FileInfo(listView1.Items[i].SubItems[3].Text);
                        if(fileInfo.Length==0)
                        {
                            item.DownLoadState = DownLoadState.Wait;
                            listView1.Items[i].SubItems[2].Text = "等待下载";
                        }
                        else
                        {
                            item.DownLoadState = DownLoadState.Finshed;
                            listView1.Items[i].SubItems[2].Text = "下载完成";
                            UpdateList();
                        }  
                    });
                    return;
                }
            }   
        }
        //批量添加
        //private void button2_Click(object sender, EventArgs e)
        //{
        //    AddUrl addUrl = new AddUrl();
            
        //    addUrl.onAdd += (urls) =>
        //    {
        //        string[] urlArr = urls.Split('\n');
        //        for (int i = 0; i < urlArr.Length; i++)
        //        {
        //            string[] parms = urlArr[i].Split('?');
        //            string pass = parms[1].Split('=')[1];
        //            Thread th = new Thread(() =>
        //            {     
        //                AddUrl(urlArr[i], pass);
        //            });
        //            th.Start();
        //        }      
        //    };
        //    addUrl.ShowDialog();
        //}
    }
}
