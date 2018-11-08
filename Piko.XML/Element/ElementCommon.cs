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
    public abstract class AElement
    {
        public AElement(ElementType Type)
        {
            this.ElementType = Type;
        }

        public ElementType ElementType { get; set; }

    }
}
