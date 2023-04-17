using AlgoShop.Orchestrator.Protos;
using Grpc.Net.Client;

namespace AlgoShop.Orchestrator.Services
{
    public interface IGrpcService
    {
        OrderProcessing.OrderProcessingClient OrderProcessingClient { get; }
    };

    public class GrpcService : IGrpcService
    {
        readonly GrpcChannel GrpcChannel = GrpcChannel.ForAddress("http://localhost:5004");

        OrderProcessing.OrderProcessingClient IGrpcService.OrderProcessingClient
        {
            get
            {
                return new OrderProcessing.OrderProcessingClient(GrpcChannel);
            }
        }
    }
}
