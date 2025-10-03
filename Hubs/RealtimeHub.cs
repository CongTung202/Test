using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace NemeShop.Hubs
{
    public class RealtimeHub : Hub
    {
        public async Task NotifyProductUpdated(int productId)
            => await Clients.All.SendAsync("ProductUpdated", productId);

        public async Task NotifyReviewAdded(int productId, int reviewId)
            => await Clients.All.SendAsync("ReviewAdded", productId, reviewId);

        public async Task NotifyOrderStatusChanged(int orderId, int status)
            => await Clients.All.SendAsync("OrderStatusChanged", orderId, status);
    }
}
