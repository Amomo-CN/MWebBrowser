using CefSharp; // 导入CefSharp命名空间
using CefSharp.WinForms;

using Cys_Controls.Code; // 导入Cys_Controls代码命名空间

using Cys_CustomControls.Controls; // 导入Cys_CustomControls控件命名空间

using Cys_Model.Tables; // 导入Cys_Model表格命名空间

using Cys_Resource.Code; // 导入Cys_Resource代码命名空间

using Cys_Services; // 导入Cys_Services服务命名空间

using MWebBrowser.Code; // 导入MWebBrowser代码命名空间
using MWebBrowser.Code.CefWebOperate; // 导入MWebBrowser代码中的CefWebOperate命名空间
using MWebBrowser.Code.Helpers; // 导入MWebBrowser代码中的Helpers命名空间
using MWebBrowser.ViewModel; // 导入MWebBrowser的视图模型命名空间

using System; // 导入System命名空间
using System.Diagnostics; // 导入System.Diagnostics命名空间
using System.Windows; // 导入System.Windows命名空间
using System.Windows.Controls; // 导入System.Windows.Controls命名空间
using System.Windows.Data; // 导入System.Windows.Data命名空间
using System.Windows.Input; // 导入System.Windows.Input命名空间
using System.Windows.Media; // 导入System.Windows.Media命名空间

namespace MWebBrowser.View
{
    /// <summary>
    /// Interaction logic for WebTabControlUc.xaml
    /// 与 WebTabControlUc.xaml 的交互逻辑
    /// </summary>
    public partial class WebTabControlUc : UserControl // 定义WebTabControlUc类，它继承自UserControl
    {
        private readonly WebTabControlViewModel viewModel; // 声明只读的WebTabControlViewModel对象
        private WebTabItemUc currentWebTabItem; // 声明当前的WebTabItemUc对象
        private HistoryServices historyServices; // 声明HistoryServices对象
        private CefWebZoom cefWebZoom; // 声明CefWebZoom对象
        private CefWebSearch cefWebSearch; // 声明CefWebSearch对象

        public WebTabControlUc() // 构造函数
        {
            InitializeComponent(); // 初始化组件
            InitWebTabControl(); // 初始化WebTabControl
            historyServices = new HistoryServices(); // 初始化HistoryServices
            viewModel = new WebTabControlViewModel(); // 初始化WebTabControlViewModel
            this.DataContext = viewModel; // 设置数据上下文为viewModel
            this.Loaded += MWebBrowserUc_Loaded; // 绑定Loaded事件处理方法
            WebTabControl.SelectionChanged += WebTabControl_SelectionChanged; // 绑定SelectionChanged事件处理方法
            FavoritesMenu.GetWebUrlEvent += () => viewModel; // 绑定FavoritesMenu获取URL事件
            FavoritesMenu.OpenNewTabEvent += TabItemAdd; // 绑定FavoritesMenu打开新标签页事件
            FavoritesMenu.RefreshFavoritesBarEvent += FavoritesBar.RefreshFavoritesBar; // 绑定FavoritesMenu刷新收藏栏事件
            FavoritesBar.GetWebUrlEvent += () => viewModel; // 绑定FavoritesBar获取URL事件
            FavoritesBar.OpenNewTabEvent += TabItemAdd; // 绑定FavoritesBar打开新标签页事件
        }

        private void MWebBrowserUc_Loaded(object sender, RoutedEventArgs e) // Loaded事件处理方法
        {
            if (this.IsInDesignMode()) // 如果处于设计模式
                return; // 返回
            InitCommand(); // 初始化命令
            InitData(); // 初始化数据
            TabItemAdd("http://192.168.100.216:8081/U9C/mvc/main/index"); // 添加新标签页并加载URL
            TabItemAdd("http://192.168.10.173:8088/Login.aspx?LastUrl=%2F"); // 添加新标签页并加载URL
            TabItemAdd("http://192.168.10.209:8080/webroot/decision/login"); // 添加新标签页并加载URL
            TabItemAdd("http://192.168.10.209:8080/webroot/decision/v10/entry/access/734b3268-55f8-4e48-9c03-0fa78a8fd17d?width=2160&height=1078"); // 添加新标签页并加载URL
        }

     

        #region InitData

        private void InitWebTabControl() // 初始化WebTabControl的方法
        {
            WebTabControl.CloseTabEvent += () => // 绑定关闭标签页事件
            {
                Dispatcher.Invoke(() => // 使用Dispatcher在主线程上执行
                {
                    if (Application.Current.MainWindow != null) // 如果主窗口不为空
                        Application.Current.MainWindow.Close(); // 关闭主窗口
                });
            };
        }

        private void InitData() // 初始化数据的方法
        {
            if (Application.Current.MainWindow is MMainWindow mw) // 如果主窗口是MMainWindow的实例
            {
                WebTabControl.PartHeaderParentGrid.MouseLeftButtonDown += mw.HeaderClickOrDragMove; // 绑定鼠标左键按下事件
            }
            cefWebZoom = new CefWebZoom(WebMenu, viewModel, SearchText); // 初始化CefWebZoom
            cefWebSearch = new CefWebSearch(viewModel); // 初始化CefWebSearch
            DownloadTool.ShowDownloadTabEvent += ShowDownloadTab; // 绑定显示下载标签页事件
            WebMenu.ExecuteMenuEvent += ExecuteMenuFunction; // 绑定执行菜单功能事件
        }

        private void InitCommand() // 初始化命令的方法
        {
            WebTabControl.TabItemAddCommand = new BaseCommand<object>(TabItemAdd); // 设置添加标签页命令
            WebTabControl.TabItemRemoveCommand = new BaseCommand<object>(RemoveItemCommand); // 设置移除标签页命令
        }
        #endregion

        #region DownloadTool

        private void ShowDownloadTab() // 显示下载标签页的方法
        {
            GlobalControl.DownloadShowAll ??= new DownloadShowAllUc(); // 如果DownloadShowAll为null，则初始化
            var item = new TabItem { Content = GlobalControl.DownloadShowAll }; // 创建新标签页并设置内容为DownloadShowAll
            item.SetValue(HeaderedContentControl.HeaderProperty, "下载"); // 设置标签页的标题为"下载"
            var source = Application.Current.FindResource($"DrawingImage.File") as ImageSource; // 获取图标资源
            item.SetValue(AttachedPropertyClass.ImageSourceProperty, source); // 设置标签页的图标
            WebTabControl.Items.Add(item); // 将标签页添加到TabControl
            WebTabControl.SelectedItem = item; // 设为选中标签页
            WebTabControl.SetHeaderPanelWidth(); // 设置标签页头部面板宽度
        }
        #endregion

        private void Print() => currentWebTabItem.CefWebBrowser.Print(); // 打印当前标签页的方法

        #region SettingTool

        private void ShowSettingTab() // 显示设置标签页的方法
        {
            bool having = false; // 标记是否已有设置标签页
            foreach (var temp in WebTabControl.Items) // 遍历所有标签页
            {
                if (temp is TabItem && ((TabItem)temp).Content is SettingUc) // 如果标签页内容是SettingUc
                {
                    having = true; // 已有设置标签页
                    WebTabControl.SelectedItem = temp; // 设为选中标签页
                    break; // 结束循环
                }
            }
            if (!having) // 如果没有设置标签页
            {
                SettingUc Setting = new SettingUc(); // 创建新的SettingUc
                var item = new TabItem { Content = Setting }; // 创建新标签页并设置内容为SettingUc
                item.SetValue(HeaderedContentControl.HeaderProperty, "设置"); // 设置标签页的标题为"设置"
                WebTabControl.Items.Add(item); // 将标签页添加到TabControl
                WebTabControl.SelectedItem = item; // 设为选中标签页
                WebTabControl.SetHeaderPanelWidth(); // 设置标签页头部面板宽度
            }
        }
        #endregion

        #region TabControl

        /// <summary>
        /// TabControl KeyDown
        /// TabControl键盘按下事件处理方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebTabControlUc_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(WebTabControl.SelectedItem is TabItem item)) return; // 如果选中的标签页不是TabItem则返回
            if (!(item.Content is WebTabItemUc webTabItemUc)) return; // 如果标签页的内容不是WebTabItemUc则返回
            int virtualKey = KeyInterop.VirtualKeyFromKey(e.Key); // 获取虚拟键值
            webTabItemUc.CefWebBrowser_PreviewKeyDown(virtualKey); // 处理键盘按下事件
        }

        /// <summary>
        /// 添加新的TabItem
        /// </summary>
        /// <param name="obj"></param>
        public void TabItemAdd(object obj)  // 定义一个方法，用于添加新的标签页
        {
            try
            {
                DispatcherHelper.UIDispatcher.Invoke(() => // 使用 DispatcherHelper 在主线程上执行 UI 更新
                {
                    var uc = new WebTabItemUc { ViewModel = { CurrentUrl = obj?.ToString() } };  // 创建新的 WebTabItemUc 实例，并设置当前网址
                    uc.SetCurrentEvent += SetCurrentSelectedInfo;  // 绑定事件，用于设置当前选中的信息
                    uc.CefWebBrowser.DownloadCallBackEvent += DownloadTool.DownloadFile;  // 绑定下载回调事件
                    uc.CefWebBrowser.AfterLoadEvent += AfterLoad;  // 绑定加载完成后的事件
                    uc.CefWebBrowser.OpenNewTabEvent += TabItemAdd;  // 绑定打开新标签页的事件
                    uc.CefWebBrowser.MouseWheelEvent += WebMouseWheel;  // 绑定鼠标滚轮事件

                    uc.CefWebBrowser.Load(obj?.ToString());// 开始加载新标签页的内容

                    var item = new TabItem { Content = uc };  // 创建新的 TabItem，并将内容设置为 uc
                    var titleBind = new Binding { Source = uc.DataContext, Path = new PropertyPath("Title") };  // 绑定标题
                    item.SetBinding(HeaderedContentControl.HeaderProperty, titleBind);  // 将标题绑定到 TabItem 的 Header 属性
                    var faviconBind = new Binding { Source = uc.DataContext, Path = new PropertyPath("Favicon") };  // 绑定图标
                    item.SetBinding(AttachedPropertyClass.ImageSourceProperty, faviconBind);  // 将图标绑定到自定义的 AttachedProperty

                    WebTabControl.Items.Add(item);// 将新的标签项添加到 TabControl

                    WebTabControl.UpdateLayout();// 强制更新布局，确保内容立即显示
                    WebTabControl.SelectedItem = item;  // 将新的标签页设为选中状态
                    WebTabControl.SetHeaderPanelWidth();  // 设置标签页头部面板的宽度
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"发生异常：{ex}");  // 捕获异常并输出调试信息
            }
        }

        public void RemoveItemCommand(object obj) // 移除标签页命令的方法
        {
            if (obj is TabItem item) // 如果参数是TabItem
            {
                WebTabControl.Items.Remove(item); // 从TabControl中移除

                if (item.Content is WebTabItemUc webTabItem) // 如果标签页内容是WebTabItemUc
                {
                    webTabItem.Dispose(); // 释放资源
                }
            }
            WebTabControl.SetHeaderPanelWidth(); // 设置标签页头部面板宽度

            if (WebTabControl.Items.Count <= 0) // 如果没有标签页
            {
                WebTabControl.CloseTabEvent?.Invoke(); // 触发关闭事件
            }
        }
        private async void AfterLoad() // 加载完成后执行的方法
        {
            try
            {
                Dispatcher.Invoke(() => // 使用Dispatcher在主线程上执行
                {
                    //_viewModel.Title = _currentWebTabItem.CefWebBrowser.Title;
                    viewModel.CurrentUrl = currentWebTabItem.CefWebBrowser.Address; // 更新当前URL
                });

                var model = new HistoryModel { Url = viewModel.CurrentUrl, VisitTime = DateTime.Now, FormVisit = 0, Title = viewModel.Title }; // 创建历史记录模型
                await historyServices.AddHistory(model); // 添加到历史记录
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"发生异常：{ex}"); // 捕获异常并输出调试信息
            }
        }
        private void ExecuteMenuFunction(string obj) // 执行菜单功能的方法
        {
            switch (obj)
            {
                case "0":
                    TabItemAdd("http://www.baidu.com"); // 添加新标签页并加载百度
                    break;
                case "4":
                    FavoritesMenu.FavoritesButton.IsChecked = true; // 勾选收藏夹按钮
                    break;
                case "5":
                    History.HistoryButton.IsChecked = true; // 勾选历史记录按钮
                    break;
                case "6":
                    ShowDownloadTab(); // 显示下载标签页
                    break;
                case "10":
                    Print(); // 打印
                    break;
                case "15":
                    ShowSettingTab(); // 显示设置标签页
                    break;
            }
        }
        /// <summary>
        /// TabControl 选中值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) // TabControl选中项改变事件处理方法
        {
            if (WebTabControl.SelectedItem is TabItem { Content: WebTabItemUc webTabItemUc }) // 如果选中项是TabItem且内容是WebTabItemUc
            {
                currentWebTabItem = webTabItemUc; // 设置当前WebTabItem
                cefWebZoom.SetWebTabItem(currentWebTabItem); // 设置CefWebZoom的当前项
                cefWebSearch.SetWebTabItem(currentWebTabItem); // 设置CefWebSearch的当前项
                SetCurrentSelectedInfo(); // 设置当前选中信息
            }
        }

        /// <summary>
        /// 设置当前选中信息
        /// </summary>
        private void SetCurrentSelectedInfo() // 设置当前选中信息的方法
        {
            viewModel.CurrentUrl = currentWebTabItem.ViewModel.CurrentUrl; // 更新当前URL
            viewModel.Title = currentWebTabItem.ViewModel.Title; // 更新标题
        }

        #endregion

        private void WebMouseWheel(int delta) => cefWebZoom.WebMouseWheelZoom(delta); // 鼠标滚轮事件处理方法

        #region search box
        private void NavigationBack_OnClick(object sender, RoutedEventArgs e) => cefWebSearch.NavigationBack(); // 后退按钮点击事件
        private void NavigationForward_OnClick(object sender, RoutedEventArgs e) => cefWebSearch.NavigationForward(); // 前进按钮点击事件
        private void NavigationRefresh_OnClick(object sender, RoutedEventArgs e) => cefWebSearch.NavigationRefresh(); // 刷新按钮点击事件
        private void Search_OnKeyDown(object sender, KeyEventArgs e) => cefWebSearch.SearchOnKeyDown(e); // 搜索框键盘按下事件
        #endregion
    }
}
