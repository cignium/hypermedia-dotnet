using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Acme.OrderManagement.Results;
using Hypermedia;

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

            var result = new OrderResult() {
                Number = order.Number,
                CreatedDate = order.CreatedDate,
                DeliveryAddress = order.DeliveryAddress,
                HeadOfficeAddress = new AddressResult() {
                    City = "New York"
                }.WithUpdate(() => Patch(id, null))
            }
                .WithParent(() => All())
                .WithAction(() => Create(), "Create New");

            result.ConfigureProperty(x => x.Number, x => x.WithAction(() => Patch(id, ""), "")
        );

            return result.CreateActionResult();
        }

        [HttpPost]
        [Route("orders/{id}")]
        public ActionResult Patch(Guid id, string data) {
            return Get(id);
        }

        [HttpGet]
        [Route("orders/dynamic")]
        public ActionResult DynamicResult() {
            return new HypermediaResource()
                .ConfigureProperty("Number", 10, (NumberValue x) => x.Format = "n2")
                .ConfigureProperty("Name", 10, (StringValue x) => x.Format = "textarea")
                .CreateActionResult();
        }
    }
}