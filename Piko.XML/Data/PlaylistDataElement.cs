using Piko.XML.Data;
using Piko.XML.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PikoTrafficManager.Data
{
    [Serializable()]
    [DataContract(Namespace = "http://developer.piko.com/PikoDataService/Data/")]
    public class PlaylistElementData
    {
        public PlaylistElementData()
        {
            //Default
            this.Uid = "";
            this.Title = "";
            this.TCIn = 0;
            this.FrameRate = Data.FrameRate.PAL;
            this.Duration = 0;
            this.StartMode = Data.StartMode.Auto;
        }
        [DataMember]
        public SupportData Support { get; set; }
        [DataMember]
        public String Uid { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public uint TCIn { get; set; }
        [DataMember]
        public bool Repeat { get; set; }
        [DataMember]
        public ulong Duration { get; set; }
        [DataMember]
        public FrameRate FrameRate { get; set; }
        [DataMember]
        public StartMode StartMode { get; set; }
        [DataMember]
        public ElementType ElementType { get; set; }
        [DataMember]
        public PlaylistElementSecondaryEventData[] SecondaryEvents { get; set; }
    }
}
