using Piko.XML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PikoTrafficManager.Data
{
    [Serializable()]
    [DataContract(Namespace = "http://developer.piko.com/PikoDataService/Data/")]
    public class SupportData
    {
        [DataMember]
        public string UIdSupport { get; set; }
        [DataMember]
        public int IdCategory { get; set; }
        [DataMember]
        public string Extension { get; set; }
        [DataMember]
        public ulong TcStart { get; set; }
        [DataMember]
        public ulong Duration { get; set; }
        [DataMember]
        public int Eom { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public FrameRate FrameRate { get; set; }
        [DataMember]
        public long FileSize { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string FullPath { get; set; }
        [DataMember]
        public bool IsExist { get; set; }
        [DataMember]
        public TemplateFieldValueData[] TemplateFields { get; set; }
    }
}
