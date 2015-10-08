using System.Collections.Generic;

namespace Hypermedia {
    public class HypermediaValue<T> {
        public HypermediaValue(T value) {
            Links = new List<UnResolvedLink>();
            Value = value;
        }

        public HypermediaValue() {
            Links = new List<UnResolvedLink>();
        }

        public T Value { get; set; }
        public IList<UnResolvedLink> Links { get; }

        public static implicit operator HypermediaValue<T>(T value) {
            return new HypermediaValue<T>(value);
        }
    }
}