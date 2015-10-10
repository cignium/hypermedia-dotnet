using System;
using System.Collections.Generic;
using System.Linq;

namespace Hypermedia {
    public class UnresolvedLinkResolver {
        private readonly IControllerActionUrlResolver _resolver;
        private readonly ICollection<ILinkFilter> _filters;

        public UnresolvedLinkResolver(IControllerActionUrlResolver resolver, ICollection<ILinkFilter> filters) {
            if (resolver == null) {
                throw new ArgumentNullException(nameof(resolver));
            }
            if (filters == null) {
                throw new ArgumentNullException(nameof(filters));
            }
            _resolver = resolver;
            _filters = filters;
        }

        public IEnumerable<ResolvedLink> Resolve(IEnumerable<UnresolvedLink> links) {
            return links
                .Where(x => x.IsValid(_filters))
                .Select(x => new ResolvedLink(x.Rel, x.Title, x.Resolve(_resolver)))
                .ToList();
        }
    }
}