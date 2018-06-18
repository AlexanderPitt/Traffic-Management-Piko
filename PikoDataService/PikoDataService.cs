using PikoDataService.Wcf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PikoDataService
{

    
    public partial class PikoDataService : ServiceBase
    {
        private ServiceHost _serviceHost = null;

        public PikoDataService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (this._serviceHost != null)
            {
                this._serviceHost.Close();
            }

            // Mise en place et démarrage du service mover WCF.
            List<Uri> serviceAddresses = new List<Uri>();
            //serviceAddresses.Add(new Uri(ConfigurationManager.AppSettings["LocalAddress"]));
            serviceAddresses.Add(new Uri(ConfigurationManager.AppSettings["ExternalAddress"]));
            
            this._serviceHost = new ServiceHost(typeof(ServiceDataPiko), serviceAddresses.ToArray());
            this._serviceHost.AddServiceEndpoint(typeof(IPikoDataService), new NetTcpBinding(SecurityMode.None), "ServiceDataPiko");
            //this._serviceHost.AddServiceEndpoint(typeof(IPikoDataService), new NetHttpBinding(BasicHttpSecurityMode.None), "ServiceDataPiko");
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();            
            //smb.HttpsGetEnabled = true;
            this._serviceHost.Description.Behaviors.Add(smb);
            this._serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");
            //this._serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            this._serviceHost.Open();
        }

        protected override void OnStop()
        {
            try
            {
                if (this._serviceHost != null)
                {
                    
                    try
                    {
                        this._serviceHost.Close();
                    }
                    catch
                    {
                        this._serviceHost.Abort();
                    }
                    finally
                    {
                        this._serviceHost = null;
                    }
                }
                
            }
            catch (Exception ex)
            {
                
            }
        }

        public void InternalStart()
        {
            OnStart(null);
        }

        public void InternalStop()
        {
            OnStop();
        }

    }
}
