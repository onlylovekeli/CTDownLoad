using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 城通下载器
{
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result=folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                textBox1.Text = path;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string path =Path.Combine(Application.StartupPath,"config.json");
            JObject cfg = new JObject();
            cfg["path"] = textBox1.Text;
            File.WriteAllText(path,JsonConvert.SerializeObject(cfg));
            Close();
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "config.json");
            JObject cfg = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));
            textBox1.Text = cfg["path"].ToString();
        }
    }
}
