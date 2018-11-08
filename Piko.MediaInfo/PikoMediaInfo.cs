using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaInfo;


namespace Piko.Media.Info.Tool
{
    public enum MediaInfoScanType
    {
        Lite,
        Full
    }


    public static class PikoMediaInfo
    {
        public static string DllPath;

        public static string Scan(string MediaPath, MediaInfoScanType ScanType = MediaInfoScanType.Full)
        {
            try
            {
                //MediaInfoWrapper mediaInfoWrapper = new MediaInfoWrapper(MediaPath);
                using (var mediaInfo = new MediaInfo.MediaInfo(DllPath)) //MediaInfo Dll
                {
                    mediaInfo.Open(MediaPath);
                    if (ScanType == MediaInfoScanType.Full)
                        mediaInfo.Option("Complete");
                    return mediaInfo.Inform();
                }
            }
            catch(Exception ex)
            {
                return "";
            }            
        }
    }
}
