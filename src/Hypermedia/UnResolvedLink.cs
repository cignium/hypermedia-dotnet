using System;
using System.Collections.Generic;

namespace Hypermedia {
    public abstract class UnresolvedLink {
        protected UnresolvedLink(string rel, string title) {
            Rel = rel;
            Title = title;
        }

        public string Rel { get; }
        public string Title { get; }
        public abstract Uri Resolve(IControllerActionUrlResolver resolver);

        public virtual bool IsValid(IEnumerable<ILinkFilter> filters) {
            return true;
        }
    }
}