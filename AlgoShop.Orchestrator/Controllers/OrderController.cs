using AlgoShop.Orchestrator.Models;
using AlgoShop.Orchestrator.Protos;
using AlgoShop.Orchestrator.Services;
using Microsoft.AspNetCore.Mvc;
using ProtoProduct = AlgoShop.Orchestrator.Protos.Product;

namespace AlgoShop.Orchestrator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IGrpcService _grpcService;

        public OrderController(IGrpcService grpcServic)
        {
            _grpcService = grpcServic;
        }

        [HttpGet]
        public IActionResult GetAsync()
        {
            var data = new GetRequest { };
            var orders = _grpcService.OrderProcessingClient.GetAsync(data);

            return Ok(orders);
        }

        [HttpPost]
        public IActionResult CreateAsync([FromBody] OrderRequest orderRequest)
        {
            if (orderRequest.Products.Count == 0)
            {
                return BadRequest();
            }

            var protoProducts = new List<ProtoProduct>();

            foreach (var product in orderRequest.Products)
            {
                var protoProduct = new ProtoProduct
                {
                    Id = product.ID,
                    Quantity = product.Quantity
                };

                protoProducts.Add(protoProduct);
            }

            var data = new CreateRequest { };
            data.Products.AddRange(protoProducts);

            var orderId = _grpcService.OrderProcessingClient.CreateAsync(data);

            return Ok(orderId);
        }
    }
}
