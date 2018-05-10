using NSubstitute;
using OrderServiceDemo.Models.Exceptions;
using OrderServiceDemo.Services.Components;
using OrderServiceDemo.Services.Infrastructure;
using OrderServiceDemo.Models;
using OrderServiceDemo.Core;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

using Xunit;

namespace OrderServiceDemo.Unit.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLineItemRepository _orderLineItemRepository;

        public OrderServiceTests()
        {
            _orderRepository = Substitute.For<IOrderRepository>();
            _orderLineItemRepository = Substitute.For<IOrderLineItemRepository>();
        }

        [Fact]
        public async Task OrderService_WhenCreatingOrder_IfNoLineItems_ThrowsInvalidRequestException()
        {
            //Arrange
            var order = new Models.Order();
            var service = BuildService();

            //Act && Assert
            var result = await Assert.ThrowsAsync<InvalidRequestException>(() => service.CreateOrder(order));
        }

		[Fact]
        public async Task OrderService_WhenCancellingOrder_IfNoOrderExists_ThrowsInvalidRequestException()
        {
            //Arrange
            var order = new Models.Order();
            var service = BuildService();

            //Act && Assert
			var result = await Assert.ThrowsAsync<InvalidRequestException>(() => service.CancelOrder(order.OrderId));
        }

		[Fact]
        public async Task OrderService_WhenCancellingOrder_IfOrderAlreadyCancelled_ThrowsInvalidRequestException()
        {
            //Arrange
            var order = new Models.Order();
			order.OrderStatus = OrderStatus.Cancelled;
            var service = BuildService();

            //Act && Assert
            var result = await Assert.ThrowsAsync<InvalidRequestException>(() => service.CancelOrder(order.OrderId));
        }
        
		[Fact]
		public async Task OrderService_CancelOrder()
        {
            //Arrange
			var order = BuildOrderToModify();
            var service = BuildService();
			_orderRepository.GetOrder(1234).Returns(order);

			var updatedOrder = await service.CancelOrder(order.OrderId);
			//Act && Assert
			Assert.Equal(1234, updatedOrder.OrderId);
			Assert.Equal(OrderStatus.Cancelled, updatedOrder.OrderStatus);
        }

		[Fact]
        public async Task OrderService_WhenDeletingOrder_IfNoOrderExists_ThrowsInvalidRequestException()
        {
            //Arrange
            var order = new Models.Order();
			var service = BuildService();
            
            //Act && Assert
			var result = await Assert.ThrowsAsync<InvalidRequestException>(() => service.DeleteOrder(order.OrderId));
        }

        private OrderService BuildService() => new OrderService(
            _orderRepository,
            _orderLineItemRepository);

		private Order BuildOrderToModify() {
			Order orderToModify = new Order();
			orderToModify.OrderId = 1234;
			orderToModify.OrderStatus = OrderStatus.Open;
			List<OrderLineItem> lineItems = new List<OrderLineItem>();
            OrderLineItem lineItem = new OrderLineItem();
            lineItem.OrderId = 1234;
            lineItem.ProductId = 4321;
            lineItem.Quantity = 1;
            lineItem.ItemPrice = 4;
            lineItems.Add(lineItem);

            orderToModify.OrderLineItems = lineItems;

			string output = JsonConvert.SerializeObject(orderToModify);
            
			return orderToModify;
		}
		
    }
}
