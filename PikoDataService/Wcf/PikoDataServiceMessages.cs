using PikoTrafficManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PikoDataService.Wcf
{

    [MessageContract]
    public class VolumeDataMessage
    {
        [MessageBodyMember]
        public VolumeData Volume { get; set; }
    }




}
