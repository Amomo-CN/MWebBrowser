using Cys_Common;
using Cys_Common.Enum;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cys_DataRepository
{
    public class DownloadDataRepository
    {
        public void SaveDownloadSetting()
        {
            try
            {
                var setting = GlobalInfo.DownloadSetting;
                if (setting == null) return;
                var fileName = FileDataPath.GetFilePath(DataFileType.Download);
                CommonOperator.SaveDataJson(setting, fileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"发生异常：{ex}");
            }
        }

        public DownloadSetting GetDownloadSetting()
        {
            var fileName = FileDataPath.GetFilePath(DataFileType.Download);
            var setting = CommonOperator.GetDataJson<DownloadSetting>(fileName);
            setting ??= new DownloadSetting();
            setting.DownloadItemInfos ??= new List<DownloadItemInfo>();
            return setting;
        }
    }
}
