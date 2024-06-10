using CefSharp.WinForms;
using MWinFormsCore.CustomCef;
using System.Windows.Forms;

namespace MWinFormsCore
{
    public partial class BrowserUserControl : UserControl
    {
        public CustomWebBrowser CefWebBrowser;

        public BrowserUserControl()
        {
            InitializeComponent();
            InitData();
        }

        private void InitData()
        {
            // 初始化自定义浏览器对象
            CefWebBrowser = new CustomWebBrowser();
            // 设置浏览器控件填充布局
            CefWebBrowser.Dock = DockStyle.Fill;
            // 将浏览器控件添加到 SplitContainer 的 Panel1
            browserSplitContainer.Panel1.Controls.Add(CefWebBrowser);
        }

        
        public void ShowDevToolsDocked()// 显示开发者工具并停靠在面板中
        {
            this.Invoke(() =>
            {
               
                if (browserSplitContainer.Panel2Collapsed) // 如果 Panel2 被折叠，则展开
                {
                    browserSplitContainer.Panel2Collapsed = false;
                }

              
                Control devToolsControl = GetDevToolsControl();  // 获取开发者工具控件

               
                if (devToolsControl == null || devToolsControl.IsDisposed) // 如果开发者工具控件为空或已释放，则创建并显示新的开发者工具
                {
                    CefWebBrowser.ShowDevToolsDocked(browserSplitContainer.Panel2, nameof(devToolsControl), DockStyle.Fill);
                }
            });
        }

        
        public void CloseDevToolsDocked()// 关闭停靠的开发者工具
        {
           
            Control devToolsControl = GetDevToolsControl(); // 获取开发者工具控件

            browserSplitContainer.Panel2.Controls.Remove(devToolsControl);  // 从 Panel2 中移除开发者工具控件并释放资源
            devToolsControl?.Dispose();

           
            if (!browserSplitContainer.Panel2Collapsed) // 如果 Panel2 未折叠，则将其折叠
            {
                browserSplitContainer.Panel2Collapsed = true;
            }
        }

       
        private Control GetDevToolsControl() // 获取开发者工具控件
        {
            // 查找名为 devToolsControl 的控件并返回
            Control devToolsControl = browserSplitContainer.Panel2.Controls.Find(nameof(devToolsControl), false).FirstOrDefault();
            return devToolsControl;
        }
    }
}
