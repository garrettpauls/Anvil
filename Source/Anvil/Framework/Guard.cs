using System;

namespace Anvil.Framework
{
    public sealed class Guard
    {
        public static void AgainstNullArgument(string paramName, object value)
        {
            if(value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}