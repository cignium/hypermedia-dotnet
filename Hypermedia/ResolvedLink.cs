using System;

namespace Hypermedia
{
    public class ResolvedLink {
        public ResolvedLink(string rel, string title, Uri url) {
            Rel = rel;
            Title = title;
            Url = url;
        }

        public string Rel { get; }
        public string Title { get; }
        public Uri Url { get; }
    }
}