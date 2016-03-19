using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Anvil.Framework.Threading
{
    public sealed class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<Task<T>> taskFactory) : base(taskFactory, LazyThreadSafetyMode.ExecutionAndPublication)
        {
        }

        public AsyncLazy(Task<T> task) : this(() => task)
        {
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return Value.GetAwaiter();
        }
    }
}
