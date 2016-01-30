using System;
using System.Collections.Generic;
using System.Linq;

namespace Anvil.Framework.ComponentModel
{
    public sealed class DisposableTracker : IDisposable
    {
        private readonly List<IDisposable> mDisposables = new List<IDisposable>();
        private readonly object mDisposedLock = new object();
        private bool mIsDisposed;

        public T Add<T>(T disposable)
            where T : IDisposable
        {
            var wasAdded = false;

            lock(mDisposedLock)
            {
                if(!mIsDisposed)
                {
                    mDisposables.Add(disposable);
                    wasAdded = true;
                }
            }

            if(!wasAdded)
            {
                disposable.Dispose();
            }

            return disposable;
        }

        public void Dispose()
        {
            lock(mDisposedLock)
            {
                if(mIsDisposed)
                {
                    return;
                }

                mIsDisposed = true;
            }

            var exceptions = new List<Exception>();
            foreach(var disposable in mDisposables)
            {
                try
                {
                    disposable.Dispose();
                }
                catch(Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if(exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }
    }

    public static class DisposableTrackerExtensions
    {
        public static T TrackWith<T>(this T disposable, DisposableTracker tracker)
            where T : IDisposable
        {
            return tracker.Add(disposable);
        }
    }
}