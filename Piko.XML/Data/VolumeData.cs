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
    public class VolumeData
    {
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public SupportData[] SupportsData { get; set; }
    }



}
