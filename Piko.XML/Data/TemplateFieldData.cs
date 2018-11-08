using PikoTrafficManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Piko.XML.Data
{
    [Serializable()]
    [DataContract(Namespace = "http://developer.piko.com/PikoDataService/Data/")]
    public class TemplateFieldData
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int IdFieldCategory { get; set; }
        [DataMember]
        public string FieldName { get; set; }
        [DataMember]
        public TemplateFieldType FieldType { get; set; }
    }
    [Serializable()]
    [DataContract(Namespace = "http://developer.piko.com/PikoDataService/Data/")]
    public class TemplateFieldValueData
    {
        [DataMember]
        public TemplateFieldData FieldDefinition { get; set; }

        [DataMember]
        public int IdValue { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}


