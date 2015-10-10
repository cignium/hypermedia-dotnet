using System.Collections.Generic;

namespace Hypermedia {
    public interface IHypermediaType {
        IList<UnresolvedLink> Links { get; }
        IList<string> Errors { get; } 
    }
}