using PikoTrafficManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PikoTrafficManager
{
    public static class Dataconverter
    {
        public static VolumeData ConvertWCFDataVolumeData(DataPikoClient.VolumeData volData)
        {
            VolumeData result = new VolumeData();
            result.Path = volData.Path;
            result.SupportsData = new SupportData[volData.SupportsData.Length];
            for(int i=0;i < volData.SupportsData.Length;i++)
            {
                DataPikoClient.SupportData supData = volData.SupportsData[i];
                result.SupportsData[i] = ConvertWCFDataSupportData(supData);
            }
            return result;
        }

        public static SupportData ConvertWCFDataSupportData(DataPikoClient.SupportData supData)
        {
            SupportData result = new SupportData();
            result.Duration = supData.Duration;
            result.Eom = supData.Eom;
            result.Extension = supData.Extension;
            result.FileName = supData.FileName;
            result.FileSize = supData.FileSize;
            result.FrameRate =(FrameRate)supData.FrameRate;
            result.FullPath = supData.FullPath;
            result.Height = supData.Height;
            result.IdCategory = supData.Height;
            result.IsExist = supData.IsExist;
            result.TcStart = supData.TcStart;
            result.Title = supData.Title;
            result.UIdSupport = supData.UIdSupport;
            result.Width = supData.Width;
            result.TemplateFields = new Piko.XML.Data.TemplateFieldValueData[supData.TemplateFields.Length];




            return result;
        }


    }
}
