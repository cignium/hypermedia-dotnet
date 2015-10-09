using System;

namespace Hypermedia {
    public class HypermediaValue<T> : HypermediaType<HypermediaValue<T>>, IHypermediaValue {
        public HypermediaValue(T value) {
            Value = value;
        }

        protected HypermediaValue() { }

        public bool IsReadonly { get; set; }
        public string Format { get; set; }
        public T Value { get; set; }

        internal string Type {
            get {
                // Todo return correct type
                return "string";
            }
        }

        protected override HypermediaValue<T> Self => this;

        public static implicit operator HypermediaValue<T>(T value) {
            return new HypermediaValue<T>(value);
        }
    }
}