using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using HypermediaClient.Results;

namespace HypermediaClient {
    public class OrderController : Controller {
        public OrderController() {
            Db = new InMemoryDatabase();
        }

        private InMemoryDatabase Db { get; }

        [Route("orders")]
        public ActionResult All() {
            return new ContentResult() { Content = "Hej" };
        }

        [Route("orders/create")]
     //   [HttpPost]
        [HttpGet]
        public async Task<ActionResult> Create() {
            var order = new Order() { Number = Db.Orders.Any() ? Db.Orders.Max(x => x.Number) + 1 : 1 };
            await Db.AddAsync(order);
            return Get(order.Id);
        }

        [Route("orders/{id}")]
        [HttpGet]
        public ActionResult Get(Guid id) {
            var order = Db.Orders.SingleOrDefault(x => x.Id == id);

            if (order == null) {
                return HttpNotFound();
            }

            return new OrderResult() {
                Number = order.Number,
                CreatedDate = order.CreatedDate
            }
            .WithParent(() => All())
            .WithAction(() => Create(), "Create New");
        }
    }
}