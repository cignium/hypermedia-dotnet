namespace Hypermedia {
    public abstract class HypermediaResource<T> : HypermediaType<T>, IHypermediaResource where T : class {
        public abstract string Profile { get; }
        public string MediaType => $"application/vnd.cignium.resource+json;profile={Profile}";

        // Should be extension method tied to Asp.Net 4
        public HypermediaActionResult CreateActionResult() {
            return new HypermediaActionResult(this);
        }
    }
}
