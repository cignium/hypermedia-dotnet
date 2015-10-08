using System.Collections.Generic;
using System.Threading.Tasks;

namespace HypermediaClient {
    public class InMemoryDatabase {
        private static readonly IList<Order> OrderList = new List<Order>();
        public IList<Order> Orders => OrderList;

        public async Task AddAsync(Order order) {
            await Task.Run(() => Orders.Add(order));
        }
    }
}