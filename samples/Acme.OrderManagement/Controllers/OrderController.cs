using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Acme.OrderManagement.Results;

namespace Acme.OrderManagement {
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
                CreatedDate = order.CreatedDate,
                DeliveryAddress = order.DeliveryAddress,
                HeadOfficeAddress = new AddressValue() {
                    City = "New York"
                }.WithUpdate(() => Patch(id, null))
            }
            .WithParent(() => All())
            .WithAction(() => Create(), "Create New")
            .CreateActionResult();
        }

        [HttpPost]
        [Route("orders/{id}")]
        public ActionResult Patch(Guid id, string data) {
            return Get(id);
        }
    }
}