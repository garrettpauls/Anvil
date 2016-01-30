using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Autofac;
using Autofac.Core;

using Splat;

namespace Anvil.Framework
{
    public class AutofacDependencyResolver : IMutableDependencyResolver
    {
        private readonly IContainer mContainer;

        public AutofacDependencyResolver(IContainer container)
        {
            mContainer = container;
        }

        public void Dispose()
        {
        }

        public object GetService(Type serviceType, string contract = null)
        {
            try
            {
                return string.IsNullOrEmpty(contract)
                           ? mContainer.Resolve(serviceType)
                           : mContainer.ResolveNamed(contract, serviceType);
            }
            catch(DependencyResolutionException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType, string contract = null)
        {
            try
            {
                var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);
                object instance = string.IsNullOrEmpty(contract)
                                      ? mContainer.Resolve(enumerableType)
                                      : mContainer.ResolveNamed(contract, enumerableType);
                return ((IEnumerable) instance).Cast<object>();
            }
            catch(DependencyResolutionException)
            {
                return null;
            }
        }

        public void Register(Func<object> factory, Type serviceType, string contract = null)
        {
            var builder = new ContainerBuilder();
            if(string.IsNullOrEmpty(contract))
            {
                builder.Register(x => factory()).As(serviceType).AsImplementedInterfaces();
            }
            else
            {
                builder.Register(x => factory()).Named(contract, serviceType).AsImplementedInterfaces();
            }
            builder.Update(mContainer);
        }

        public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback)
        {
            // this method is not used by RxUI
            throw new NotImplementedException("This method is not expected to be used by RxUI");
        }
    }
}