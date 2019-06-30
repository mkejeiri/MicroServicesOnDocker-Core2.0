using System.Threading.Tasks;
using MassTransit;
using MicroServicesOnDocker.Services.CartApi.Model;
using MicroServicesOnDocker.Services.common.Messaging;
using Microsoft.Extensions.Logging;

namespace MicroServicesOnDocker.Services.CartApi.Messaging.Consumer
{
    public class OrderCompletedEventConsumer : IConsumer<OrderCompletedEvent>
    {
        private ICartRepository _repository;
        private readonly ILogger<OrderCompletedEventConsumer> _logger;

        public OrderCompletedEventConsumer(ICartRepository repository, ILogger<OrderCompletedEventConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<OrderCompletedEvent> context)
        {
           _logger.LogInformation("we are consuming the event now...");
           return _repository.DeleteCartAsync(context.Message.BuyerId);
        }
    }
}
