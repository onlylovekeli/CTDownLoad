using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 城通下载器
{
    class FileItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string DownUrl { get; set; }
        public string Size { get; set; }
        public DownLoadState DownLoadState { get; set; }
    }
    enum DownLoadState
    {
        Wait,
        Down,
        Finshed
    }
}
