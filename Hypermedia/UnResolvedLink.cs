using System;

namespace Hypermedia {
    public abstract class UnResolvedLink {
        protected UnResolvedLink(string rel, string title) {
            Rel = rel;
            Title = title;
        }

        public string Rel { get; }
        public string Title { get; }
        public abstract Uri Resolve(IControllerActionUrlResolver resolver);
    }
}