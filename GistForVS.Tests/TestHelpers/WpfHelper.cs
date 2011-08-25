using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace GistForVS.Tests.TestHelpers
{
    public class WpfHelper
    {
        public static void RunBlockAsSTA(Action block)
        {
            Exception ex = null;
            var t = new Thread(() => {

                var deferred = RxApp.DeferredScheduler; 
                var taskpool = RxApp.TaskpoolScheduler;
                try {
                    RxApp.DeferredScheduler = new DispatcherScheduler((new Grid()).Dispatcher);
                    RxApp.TaskpoolScheduler = Scheduler.ThreadPool;
                    block();
                } catch(Exception e) {
                    ex = e;
                    throw;
                } finally {
                    RxApp.DeferredScheduler = deferred;
                    RxApp.TaskpoolScheduler = taskpool;
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            if (ex != null)
            {
                // NB: If we don't do this, the test silently passes
                throw ex;
            }
        }
    }
}
