using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Piko.XML.Data
{
    [DataContract(Namespace = "http://developer.piko.com/PikoDataService/Data/")]
    public class Category
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public TemplateFieldData[] TemplateFields { get; set; }
    }
}
