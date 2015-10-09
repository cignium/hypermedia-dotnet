using System;
using Hypermedia;

namespace HypermediaClient.Results {
    public class OrderResult : DocumentResult {
        public HypermediaValue<int> Number { get; set; }
        public HypermediaValue<DateTimeOffset> CreatedDate { get; set; }
        public AddressValue HeadOfficeAddress { get; set; }
        public Address DeliveryAddress { get; set; }
        protected override DocumentResult Self => this;
    }
}