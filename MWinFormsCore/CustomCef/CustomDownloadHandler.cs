﻿using CefSharp;

using Cys_Common.Code.Configure;

namespace MWinFormsCore.CustomCef
{
    public class CustomDownloadHandler : IDownloadHandler
    {
        private readonly Action<bool, DownloadItem> _downloadCallBackEvent;//第一个参数为true为update

        public CustomDownloadHandler(Action<bool, DownloadItem> downloadCallBackEvent)
        {
            _downloadCallBackEvent = downloadCallBackEvent;
        }

        public bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
        {
            return true;
        }

        public bool OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem,
       IBeforeDownloadCallback callback)
        {
            if (callback.IsDisposed) return false;
            _downloadCallBackEvent?.Invoke(false, downloadItem);
            downloadItem.IsInProgress = true;
            var path = GetDownloadFullPath(downloadItem.SuggestedFileName);
            callback.Continue(path, false);

            // 根据你的业务逻辑决定是否允许下载，这里示例为允许
            return true;
        }


        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem,
            IDownloadItemCallback callback)
        {
            _downloadCallBackEvent?.Invoke(true, downloadItem);
        }


        private string GetDownloadFullPath(string suggestedFileName)
        {
            var configPath = ConfigHelper.Config.DownLoadPath.TrimEnd('\\') + "\\";
            return configPath + suggestedFileName;
        }
    }
}
