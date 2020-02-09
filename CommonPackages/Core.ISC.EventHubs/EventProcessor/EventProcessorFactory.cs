using Core.ISC.Messaging;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
[assembly: InternalsVisibleTo("Core.ISC.EventHubs.Test")]
namespace Core.ISC.EventHubs
{
    class EventProcessorFactory : IEventProcessorFactory
    {
        private IServiceProvider serviceProvider;

        public EventProcessorFactory(IServiceProvider obj)
        {
            this.serviceProvider = obj;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {

            return serviceProvider.GetRequiredService<IEventProcessor>();
        }
    }

}
