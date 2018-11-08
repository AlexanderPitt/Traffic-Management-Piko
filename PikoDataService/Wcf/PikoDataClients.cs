using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PikoDataService.Wcf
{
    public interface IPikoDataServiceCallBack
    {
        [OperationContract]
        void OnCallBack();
    }


    public class PikoDataClients
    {


        public IPikoDataServiceCallBack CallBack;
    }
}
