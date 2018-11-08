using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsolePikoDataService
{
    class Program
    {

        private static PikoDataService.PikoDataService _service;
        private static EventWaitHandle _waitHandle;


        static void Main(string[] args)
        {
            bool runConsole = false;

            foreach (string arg in args)
            {
                if (arg.ToLowerInvariant().Equals("-console"))
                {
                    runConsole = true;
                }
            }

            

            _service = new PikoDataService.PikoDataService();
            if (runConsole)
            {
                _waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                Console.WriteLine("Starting Workflow Service in Console Mode");
                Console.WriteLine("Press Ctrl+C to exit Console Mode");
                Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCancelKeyPress);
                _service.InternalStart();
                WaitHandle.WaitAll(new WaitHandle[] { _waitHandle });
            }

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new PikoDataService.PikoDataService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        protected static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            _service.InternalStop();
            _waitHandle.Set();
        }



    }
}
