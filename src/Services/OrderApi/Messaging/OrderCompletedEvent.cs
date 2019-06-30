using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicesOnDocker.Services.common.Messaging
{
    //this the event class to be published/fired in the transit bus  
    public class OrderCompletedEvent
    {
        //define prop to be sent to the CartApi through the bus
        public readonly string BuyerId;

        public OrderCompletedEvent(string buyerId)
        {
            BuyerId = buyerId;
        }

    }
}
