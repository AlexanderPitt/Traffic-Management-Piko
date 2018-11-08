using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PikoTrafficManager.Data
{
    [DataContract]
    public class BlockData
    {
        [DataMember]
        public string Name;
        [DataMember]
        public PlaylistElementData[] Elements = null;
    }
}
