using Hypermedia;

namespace Acme.OrderManagement.Results {
    public class AddressValue : HypermediaType<AddressValue> {
        public string Street { get; set; }
        public HypermediaValue<string> City { get; set; }
        protected override AddressValue Self => this;
    }
}