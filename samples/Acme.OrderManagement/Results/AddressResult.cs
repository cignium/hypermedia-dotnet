using Hypermedia;

namespace Acme.OrderManagement.Results {
    public class AddressResult : HypermediaType<AddressResult> {
        public string Street { get; set; }
        public HypermediaValue<string> City { get; set; }
    }
}