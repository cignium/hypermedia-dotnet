namespace Hypermedia {
    internal interface IHypermediaResource : IHypermediaType {
        string MediaType { get; }
    }
}