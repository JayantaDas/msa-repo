using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ISC.EventHubs
{
    public class Subscription
    {
        public Type HandlerType { get; }

        private Subscription(Type handlerType)
        {
            HandlerType = handlerType;
        }

        public static Subscription Typed(Type handlerType)
        {
            return new Subscription(handlerType);
        }
    }
}
