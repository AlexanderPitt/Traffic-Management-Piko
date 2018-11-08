using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PikoDataService
{
    public static class PikoDataServiceApp
    {
        private static String _databasePath = "";
        public static String DatabasePath
        {
            get
            {
                if(String.IsNullOrEmpty(_databasePath))
                    _databasePath = ConfigurationManager.AppSettings["DatabasePath"];
                return _databasePath;
            }
        }

        private static String _pikoIniPath = "";
        public static String PikoIniPath
        {
            get
            {
                if (String.IsNullOrEmpty(_pikoIniPath))
                    _pikoIniPath = ConfigurationManager.AppSettings["PikoIniPath"];
                return _pikoIniPath;
            }
        }

        private static String _pikoPlaylistPath = "";
        public static String PikoPlaylistPath
        {
            get
            {
                if (String.IsNullOrEmpty(_pikoPlaylistPath))
                {
                    _pikoPlaylistPath = ConfigurationManager.AppSettings["PikoPlaylistPath"];
                    if (!System.IO.Directory.Exists(_pikoPlaylistPath))
                        System.IO.Directory.CreateDirectory(_pikoPlaylistPath);
                }
                return _pikoPlaylistPath;
            }
        }

        private static String _pikoBlockPath = "";
        public static String PikoBlockPath
        {
            get
            {
                if (String.IsNullOrEmpty(_pikoBlockPath))
                {
                    _pikoBlockPath = ConfigurationManager.AppSettings["PikoBlockPath"];
                    if (!System.IO.Directory.Exists(_pikoBlockPath))
                        System.IO.Directory.CreateDirectory(_pikoBlockPath);
                }
                return _pikoBlockPath;
            }
        }

    }
}
