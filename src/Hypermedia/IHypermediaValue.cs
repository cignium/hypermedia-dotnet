using System.Deployment.Internal;

namespace Hypermedia {
    public interface IHypermediaValue {
        bool IsReadonly { get; set; }
        string Format { get; set; }
    }
}