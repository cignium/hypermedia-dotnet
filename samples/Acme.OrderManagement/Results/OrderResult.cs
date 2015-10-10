using System;
using Hypermedia;

namespace Acme.OrderManagement.Results {
    public class OrderResult : HypermediaResource<OrderResult> {
        public HypermediaValue<int> Number { get; set; }
        public HypermediaValue<DateTimeOffset> CreatedDate { get; set; }
        public AddressResult HeadOfficeAddress { get; set; }
        public Address DeliveryAddress { get; set; }
    }
}