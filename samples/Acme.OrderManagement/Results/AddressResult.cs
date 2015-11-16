using Hypermedia;

namespace Acme.OrderManagement.Results {
    public class AddressResult : HypermediaObject<AddressResult> {
        public string Street { get; set; }
        public string City { get; set; }
    }
}