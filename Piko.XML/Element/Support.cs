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
    public class Support : AElement
    {
        public SupportData Data
        {
            get;
            set;
        }

        public Support(SupportData Data = null) : base(ElementType.Support) {
            if(Data != null)
            {
                this.Data = Data;
            }
            else
            {
                this.Data = new SupportData();
            }
        }



    }
}
