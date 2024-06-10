using CefSharp;
using CefSharp.WinForms;

using System;
using System.IO;

namespace MWinFormsCore
{
    public class InitCefSharp
    {
        public static void InitializeCefSharp()
        {

            var settings = new CefSettings()
            {
                // 默认情况下CefSharp会使用内存缓存，需要指定缓存文件夹以持久化数据
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
                Locale = "zh-CN", // 设置默认语言为中文
                AcceptLanguageList = "zh-CN,zh;q=0.9,en;q=0.8" // 设置接受的语言列表
            };

            string bit = Environment.Is64BitProcess ? "x64" : "x86"; // 判断当前进程是64位还是32位
            settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, $"runtimes\\win-{bit}\\native",
                                                   "CefSharp.BrowserSubprocess.exe"); // 设置子进程路径

            settings.CefCommandLineArgs.Add("ppapi-flash-path", AppDomain.CurrentDomain.BaseDirectory + $"RefDLL\\{bit}\\pepflashplayer.dll"); // 设置Flash插件路径
            settings.CefCommandLineArgs.Add("ppapi-flash-version", "99.0.0.999"); // 设置Flash插件版本
            // http://ssfw.njfy.gov.cn/ssfwzx/ext/flash/flash.jsp Flash测试页面
            settings.CefCommandLineArgs.Add("disable-gpu", "1"); // 禁用GPU
            settings.CefCommandLineArgs.Add("no-proxy-server", "1"); // 禁用代理服务器
            settings.IgnoreCertificateErrors = true; // 忽略证书错误
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null); // 初始化CefSharp
        }
    }
}
