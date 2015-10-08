using System;
using Hypermedia;

namespace HypermediaClient.Results {
    public class OrderResult : DocumentResult {
        public HypermediaValue<int> Number { get; set; }
        public HypermediaValue<DateTimeOffset> CreatedDate { get; set; }
        public AddressValue Address { get; set; }
    }

    public class AddressValue : HypermediaValue<AddressValue> {
        public HypermediaValue<string> Street { get; set; }
        public HypermediaValue<string> City { get; set; }
    }
}