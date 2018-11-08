using PikoTrafficManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Piko.XML.Element
{
    [DataContract(Namespace = "http://developer.piko.com/PikoDataService/Data/")]
    public class Volume
    {

        public VolumeData Data { get; set; }
        public Volume(VolumeData VolumeData = null)
        {
            if (VolumeData != null)
                this.Data = VolumeData;
            else
            {
                this.Data = new VolumeData();
            }
        }

        public List<Support> Supports = new List<Support>();

        public string Path
        {
            get
            {
                return this.Data.Path;
            }
            set
            {
                this.Data.Path = value;
            }
        } 
    }


    public static class VolumeManager
    {
        private static void getFilesInVolumes(Volume volume)
        {
            string[] filesInVolume = System.IO.Directory.GetFiles(volume.Path);
            foreach (string fileName in filesInVolume)
            {
                Support support = new Support();
                support.Data.FileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                support.Data.UIdSupport = support.Data.FileName;
                support.Data.Extension = System.IO.Path.GetExtension(fileName);
                support.Data.FullPath = fileName;
                support.Data.Duration = 0;
                support.Data.Eom = 0;
                support.Data.IdCategory = 0;
                support.Data.Title = support.Data.FileName;
                support.Data.Width = 0;
                support.Data.Height = 0;
                support.Data.FrameRate = FrameRate.PAL;
                long length = new System.IO.FileInfo(fileName).Length;
                support.Data.FileSize = length;
                support.Data.TemplateFields = new Data.TemplateFieldValueData[0];
                volume.Supports.Add(support);
            }
        }

        public static Volume LoadVolume(string VolumePath)
        {
            Volume LoadedVolume = null;

            if (System.IO.Directory.Exists(VolumePath))
            {
                LoadedVolume = new Volume();
                LoadedVolume.Path = VolumePath;
                getFilesInVolumes(LoadedVolume);
            }


            return LoadedVolume;
        }
    }
}
