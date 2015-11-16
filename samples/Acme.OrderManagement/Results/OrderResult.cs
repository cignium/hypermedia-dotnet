using System;
using Hypermedia;

namespace Acme.OrderManagement.Results {
    public class OrderResult : HypermediaResource<OrderResult> {
        public decimal? Number { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public AddressResult HeadOfficeAddress { get; set; }
        public Address DeliveryAddress { get; set; }
    }
}