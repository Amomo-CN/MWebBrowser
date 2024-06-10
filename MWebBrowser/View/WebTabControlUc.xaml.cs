using CefSharp;

using Cys_Controls.Code;

using Cys_CustomControls.Controls;

using Cys_Model.Tables;

using Cys_Resource.Code;

using Cys_Services;

using MWebBrowser.Code;
using MWebBrowser.Code.CefWebOperate;
using MWebBrowser.Code.Helpers;
using MWebBrowser.ViewModel;

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MWebBrowser.View
{
    /// <summary>
    /// Interaction logic for WebTabControlUc.xaml
    /// </summary>
    public partial class WebTabControlUc : UserControl
    {
        private readonly WebTabControlViewModel viewModel;
        private WebTabItemUc currentWebTabItem;
        private HistoryServices historyServices;
        private CefWebZoom cefWebZoom;
        private CefWebSearch cefWebSearch;
        public WebTabControlUc()
        {
            InitializeComponent();
            InitWebTabControl();
            historyServices = new HistoryServices();
            viewModel = new WebTabControlViewModel();
            this.DataContext = viewModel;
            this.Loaded += MWebBrowserUc_Loaded;
            WebTabControl.SelectionChanged += WebTabControl_SelectionChanged;
            FavoritesMenu.GetWebUrlEvent += () => viewModel;
            FavoritesMenu.OpenNewTabEvent += TabItemAdd;
            FavoritesMenu.RefreshFavoritesBarEvent += FavoritesBar.RefreshFavoritesBar;
            FavoritesBar.GetWebUrlEvent += () => viewModel;
            FavoritesBar.OpenNewTabEvent += TabItemAdd;
        }

        private void MWebBrowserUc_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsInDesignMode())
                return;
            InitCommand();
            InitData();
            TabItemAdd("http://192.168.100.216:8081/U9C/mvc/main/index");
            TabItemAdd("http://192.168.10.173:8088/Login.aspx?LastUrl=%2F");
            TabItemAdd("http://192.168.10.209:8080/webroot/decision/login");
            TabItemAdd("http://192.168.10.209:8080/webroot/decision/v10/entry/access/734b3268-55f8-4e48-9c03-0fa78a8fd17d?width=2160&height=1078");
        }

        #region InitData

        private void InitWebTabControl()
        {
            WebTabControl.CloseTabEvent += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (Application.Current.MainWindow != null)
                        Application.Current.MainWindow.Close();
                });
            };
        }
        private void InitData()
        {
            if (Application.Current.MainWindow is MMainWindow mw)
            {
                WebTabControl.PartHeaderParentGrid.MouseLeftButtonDown += mw.HeaderClickOrDragMove;
            }
            cefWebZoom = new CefWebZoom(WebMenu, viewModel, SearchText);
            cefWebSearch = new CefWebSearch(viewModel);
            DownloadTool.ShowDownloadTabEvent += ShowDownloadTab;
            WebMenu.ExecuteMenuEvent += ExecuteMenuFunction;
        }

        private void InitCommand()
        {
            WebTabControl.TabItemAddCommand = new BaseCommand<object>(TabItemAdd);
            WebTabControl.TabItemRemoveCommand = new BaseCommand<object>(RemoveItemCommand);
        }
        #endregion

        #region DownloadTool

        private void ShowDownloadTab()
        {
            GlobalControl.DownloadShowAll ??= new DownloadShowAllUc();
            var item = new TabItem { Content = GlobalControl.DownloadShowAll };
            item.SetValue(HeaderedContentControl.HeaderProperty, "下载");
            var source = Application.Current.FindResource($"DrawingImage.File") as ImageSource;
            item.SetValue(AttachedPropertyClass.ImageSourceProperty, source);
            WebTabControl.Items.Add(item);
            WebTabControl.SelectedItem = item;
            WebTabControl.SetHeaderPanelWidth();
        }
        #endregion

        private void Print() => currentWebTabItem.CefWebBrowser.Print();

        #region SettingTool

        private void ShowSettingTab()
        {
            bool having = false;
            foreach (var temp in WebTabControl.Items)
            {
                if (temp is TabItem)
                {
                    if (((TabItem)temp).Content is SettingUc)
                    {
                        having = true;
                        WebTabControl.SelectedItem = temp;
                        break;
                    }
                }
            }
            if (!having)
            {
                SettingUc Setting = new SettingUc();
                var item = new TabItem { Content = Setting };
                item.SetValue(HeaderedContentControl.HeaderProperty, "设置");
                WebTabControl.Items.Add(item);
                WebTabControl.SelectedItem = item;
                WebTabControl.SetHeaderPanelWidth();
            }
        }

        #endregion

        #region TabControl

        /// <summary>
        /// TabControl KeyDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebTabControlUc_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(WebTabControl.SelectedItem is TabItem item)) return;
            if (!(item.Content is WebTabItemUc webTabItemUc)) return;
            int virtualKey = KeyInterop.VirtualKeyFromKey(e.Key);
            webTabItemUc.CefWebBrowser_PreviewKeyDown(virtualKey);
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

        public void RemoveItemCommand(object obj)
        {
            if (obj is TabItem item)
            {
                WebTabControl.Items.Remove(item);

                if (item.Content is WebTabItemUc webTabItem)
                {
                    webTabItem.Dispose();
                }
            }
            WebTabControl.SetHeaderPanelWidth();

            if (WebTabControl.Items.Count <= 0)
            {
                WebTabControl.CloseTabEvent?.Invoke();
            }
        }
        private async void AfterLoad()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    //_viewModel.Title = _currentWebTabItem.CefWebBrowser.Title;
                    viewModel.CurrentUrl = currentWebTabItem.CefWebBrowser.Address;
                });

                var model = new HistoryModel { Url = viewModel.CurrentUrl, VisitTime = DateTime.Now, FormVisit = 0, Title = viewModel.Title };
                await historyServices.AddHistory(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"发生异常：{ex}");
            }
        }
        private void ExecuteMenuFunction(string obj)
        {
            switch (obj)
            {
                case "0":
                    TabItemAdd("http://www.baidu.com");
                    break;
                case "4":
                    FavoritesMenu.FavoritesButton.IsChecked = true;
                    break;
                case "5":
                    History.HistoryButton.IsChecked = true;
                    break;
                case "6":
                    ShowDownloadTab();
                    break;
                case "10":
                    Print();
                    break;
                case "15":
                    ShowSettingTab();
                    break;
            }
        }
        /// <summary>
        /// TabControl 选中值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WebTabControl.SelectedItem is TabItem { Content: WebTabItemUc webTabItemUc })
            {
                currentWebTabItem = webTabItemUc;
                cefWebZoom.SetWebTabItem(currentWebTabItem);
                cefWebSearch.SetWebTabItem(currentWebTabItem);
                SetCurrentSelectedInfo();
            }
        }

        /// <summary>
        /// 设置当前选中信息
        /// </summary>
        private void SetCurrentSelectedInfo()
        {
            viewModel.CurrentUrl = currentWebTabItem.ViewModel.CurrentUrl;
            viewModel.Title = currentWebTabItem.ViewModel.Title;
        }

        #endregion

        private void WebMouseWheel(int delta) => cefWebZoom.WebMouseWheelZoom(delta);

        #region search box
        private void NavigationBack_OnClick(object sender, RoutedEventArgs e) => cefWebSearch.NavigationBack();
        private void NavigationForward_OnClick(object sender, RoutedEventArgs e) => cefWebSearch.NavigationForward();
        private void NavigationRefresh_OnClick(object sender, RoutedEventArgs e) => cefWebSearch.NavigationRefresh();
        private void Search_OnKeyDown(object sender, KeyEventArgs e) => cefWebSearch.SearchOnKeyDown(e);
        #endregion
    }
}