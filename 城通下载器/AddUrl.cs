using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 城通下载器
{
    public partial class AddUrl : Form
    {
        public Action<string> onAdd=null;
        public AddUrl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(onAdd!=null)
            {
                onAdd(textBox1.Text);
                Close();
            }
        }

    }
}
