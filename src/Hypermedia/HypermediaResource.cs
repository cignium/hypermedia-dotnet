namespace Hypermedia {
    public abstract class HypermediaResource<T> : HypermediaObject<T>, IHypermediaResource where T : HypermediaType<T> {
        private const string MediaTypeString = "application/vnd.cignium.resource+json;";
        protected virtual string Profile { get; } = string.Empty;

        public string MediaType => string.IsNullOrEmpty(Profile) ? MediaTypeString : MediaTypeString + $"profile={Profile};";

        // Should be extension method tied to Asp.Net 4
        public HypermediaActionResult CreateActionResult() {
            return new HypermediaActionResult(this);
        }
    }

    public sealed class HypermediaResource : HypermediaResource<HypermediaResource> {}
}