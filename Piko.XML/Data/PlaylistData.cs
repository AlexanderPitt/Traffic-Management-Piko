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
    public class PlaylistData
    {
        [DataMember]
        public string PlaylistFileName { get; set; }
        [DataMember]
        public PlayListType PlaylistType { get; set; }
        [DataMember]
        public string PlaylistPath { get; set; }
        [DataMember]
        public string PlaylistTitle { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public PlaylistElementData[] Elements { get; set; }
    }
}
