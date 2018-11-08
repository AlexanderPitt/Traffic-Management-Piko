using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PikoTrafficManager.Data
{
    [DataContract(Name = "ElementType")]
    public enum ElementType : uint
    {
        [EnumMember]
        Support = 0,
        [EnumMember]
        Break = 1
    }


}
