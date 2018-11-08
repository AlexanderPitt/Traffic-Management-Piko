using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Piko.XML.Data
{
    [Serializable()]
    [DataContract(Namespace = "http://developer.piko.com/PikoDataService/Data/")]
    public class PlaylistElementSecondaryEventData
    {
        [DataMember]
        public ulong Duration
        {
            get;set;
        }
        [DataMember]
        public string Param
        {
            get; set;
        }
        [DataMember]
        public string ExtendedParam
        {
            get;set;
        }
        [DataMember]
        public int Id
        {
            get; set;
        }
        [DataMember]
        public uint TcOffsetType
        {
            get; set;
        }
        [DataMember]
        public uint TcStartOffset
        {
            get; set;
        }
        [DataMember]
        public string SecondaryEventType
        {
            get; set;
        }
        [DataMember]
        public TemplateFieldValueData[] TemplateFields { get; set; }
    }
}
