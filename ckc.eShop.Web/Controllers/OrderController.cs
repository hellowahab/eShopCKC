﻿using Ckc.EShop.ApplicationCore.Interface;
using Ckc.EShop.ApplicationCore.Specifications;
using Ckc.EShop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ckc.EShop.Web.Controllers
{
    [Route("[Controller]/[action]")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUriComposer _uriComposer;

        public OrderController(IOrderRepository orderRepository,
            IUriComposer uriComposer)
        {
            _orderRepository = orderRepository;
            _uriComposer = uriComposer;

        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.ListAsync(new CustomerOrderWithItemsSpecification(User.Identity.Name));

            var viewModel = orders.Select(o => new OrderViewModel()
            {
                OrderDate = o.OrderDate,
                OrderItems = o.OrderItems?.Select
                (oi => new OrderItemViewModel()
                {
                    Discount = 0,
                    PictureUrl = _uriComposer.ComposePicUri(oi.ItemOrdered.PictureUri),
                    ProductId = oi.ItemOrdered.CatalogItemId,
                    ProductName = oi.ItemOrdered.ProductName,
                    UnitPrice = oi.UnitPrice,
                    Units = oi.Units
                }).ToList(),
                OrderNumber = o.Id,
                ShippingAddress = o.ShipToAddress,
                Status = "Pending",
                Total = o.Total()
            });
            return View(viewModel);
        }

        [HttpGet("{orderID}")]
        public async Task<IActionResult> Detail(int orderId)
        { 
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
            var viewModel = new OrderViewModel()
            {
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select
                (oi => new OrderItemViewModel()
                {
                    Discount = 0,
                    PictureUrl = _uriComposer.ComposePicUri(oi.ItemOrdered.PictureUri),
                    ProductId = oi.ItemOrdered.CatalogItemId,
                    ProductName = oi.ItemOrdered.ProductName,
                    UnitPrice = oi.UnitPrice,
                    Units = oi.Units
                }).ToList(),
                OrderNumber = order.Id,
                ShippingAddress = order.ShipToAddress,
                Status = "Pending",
                Total = order.Total()
            };
            return View(viewModel);
        }
    }
}
