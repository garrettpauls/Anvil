using System;

namespace Anvil.Framework
{
    public static class AggregateExceptionExtensions
    {
        public static Exception SingleOrFlattened(this AggregateException exception)
        {
            var flattened = exception.Flatten();

            return flattened.InnerExceptions.Count == 1
                       ? flattened.InnerExceptions[0]
                       : flattened;
        }
    }
}