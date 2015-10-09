using System.Collections.Generic;

namespace Hypermedia {
    public interface IHypermediaType {
        IList<UnResolvedLink> Links { get; }
        IList<string> Errors { get; } 
    }
}