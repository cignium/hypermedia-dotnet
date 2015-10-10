using System;

namespace Hypermedia {
    public class SimpleUrlLink : UnresolvedLink {
        public Uri Url { get; set; }

        public SimpleUrlLink(string rel, string title, Uri url) : base(rel, title) {
            Url = url;
        }

        public override Uri Resolve(IControllerActionUrlResolver resolver) {
            return Url;
        }
    }
}