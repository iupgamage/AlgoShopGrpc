using AlgoShop.Order.Protos;
using AlgoShop.Order.Settings;
using Grpc.Core;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using AlgoOrder = AlgoShop.Order.Models.Order;
using AlgoProduct = AlgoShop.Order.Models.Product;
using ProtoOrder = AlgoShop.Order.Protos.Order;
using ProtoProduct = AlgoShop.Order.Protos.Product;

namespace AlgoShop.Order.Services
{
    public class OrderService : OrderProcessing.OrderProcessingBase
    {
        private readonly IMongoCollection<AlgoOrder> _orderCollection;

        public OrderService(IOptions<AlgoShopDatabaseSettings> algoShopDatabaseSettings)
        {
            var mongoClient = new MongoClient(algoShopDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(algoShopDatabaseSettings.Value.DatabaseName);
            _orderCollection = mongoDatabase.GetCollection<AlgoOrder>(algoShopDatabaseSettings.Value.StockCollectionName);
        }

        public override async Task<GetResponse> GetAsync(GetRequest request, ServerCallContext context)
        {
            var result = await _orderCollection.Find(_ => true).ToListAsync();

            var protoOrderResponse = new GetResponse();

            foreach (var order in result ?? Enumerable.Empty<AlgoOrder>())
            {
                var protoOrder = new ProtoOrder
                {
                    Id = order.ID
                };

                var protoProducts = order.Products?.Select(p => new ProtoProduct
                {
                    Id = p.ID,
                    Quantity = p.Quantity,
                });

                protoOrder.Products.Add(protoProducts);

                protoOrderResponse.Orders.Add(protoOrder);
            }

            return protoOrderResponse;
        }

        public override async Task<CreateResponse> CreateAsync(CreateRequest request, ServerCallContext context)
        {
            var products = new List<AlgoProduct>();

            foreach (var protoProduct in request.Products ?? Enumerable.Empty<ProtoProduct>())
            {
                products.Add(new AlgoProduct { ID = protoProduct.Id, Quantity = protoProduct.Quantity });
            }

            var order = new AlgoOrder
            {
                ID = new Random().Next(),
                Products = products
            };

            await _orderCollection.InsertOneAsync(order);

            return new CreateResponse { Id = order.ID };
        }
    }
}
