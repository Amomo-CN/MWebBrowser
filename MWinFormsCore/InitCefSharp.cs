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

                // 设置持久化缓存文件夹
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
                Locale = "zh-CN", // 设置默认语言为中文
                AcceptLanguageList = "zh-CN,zh;q=0.9,en;q=0.8", // 设置接受的语言列表
              //  UserDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\UserData") // 设置用户数据目录
          
            };
            settings.Locale = "zh-CN";

            string bit = Environment.Is64BitProcess ? "x64" : "x86"; // 判断当前进程是64位还是32位
            settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, $"runtimes\\win-{bit}\\native", "CefSharp.BrowserSubprocess.exe"); // 设置子进程路径

            settings.CefCommandLineArgs.Add("ppapi-flash-path", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"RefDLL\\{bit}\\pepflashplayer.dll")); // 设置Flash插件路径
            settings.CefCommandLineArgs.Add("ppapi-flash-version", "99.0.0.999"); // 设置Flash插件版本
            settings.CefCommandLineArgs.Add("disable-gpu", "1"); // 禁用GPU
            settings.CefCommandLineArgs.Add("no-proxy-server", "1"); // 禁用代理服务器
            settings.IgnoreCertificateErrors = true; // 忽略证书错误

            // 新增以下设置以确保开发者工具的语言始终为中文
            settings.CefCommandLineArgs.Add("lang", "zh-CN"); // 设置浏览器语言为中文
            settings.CefCommandLineArgs.Add("intl.accept_languages", "zh-CN"); // 设置接受的语言列表

            // 设置环境变量强制开发者工具使用中文
            Environment.SetEnvironmentVariable("LANGUAGE", "zh_CN");

            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null); // 初始化CefSharp
        }
    }
}
