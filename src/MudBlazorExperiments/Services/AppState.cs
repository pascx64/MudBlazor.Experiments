using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Experiments.Services
{
    /// <summary>
    /// Globally store the state of the app
    /// Use this to inform components that they should lock changes and actions while API calls are in progress
    /// </summary>
    public class AppState
    {
        public bool IsBusy => BusyCount != 0;
        private long BusyCount = 0;

        public delegate void OnBusyStateChangedHandler(bool busy);
        public event OnBusyStateChangedHandler? OnBusyStateChanged;

        public async Task Busy(Task task)
        {
            if (Interlocked.Increment(ref BusyCount) == 1) // we went from 0 to 1, from not busy to very busy 
                OnBusyStateChanged?.Invoke(true);
            
            await task;

            if (Interlocked.Decrement(ref BusyCount) == 0) // we went from 0 to 1, from very busy to not busy
                OnBusyStateChanged?.Invoke(false);
        }
        
        public async Task<T> Busy<T>(Task<T> task)
        {
            await Busy((Task)task);

            return task.Result;
        }

        public Func<Task> Busy(Func<Task> func)
        {
            return () => Busy(func());
        }
        public Func<T, Task> Busy<T>(Func<T, Task> func)
        {
            return x => Busy(func(x));
        }
    }
}
