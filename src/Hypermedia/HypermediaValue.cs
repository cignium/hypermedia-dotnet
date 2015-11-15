using System;
using System.Linq.Expressions;

namespace Hypermedia {
    public abstract class HypermediaValue<T> : HypermediaType<HypermediaValue<T>>, IHypermediaValue {
        private readonly Expression<Func<T>> _valueResolver;
        private readonly T _value;

        protected HypermediaValue() { }

        protected HypermediaValue(T value) {
            _value = value;
        }

        protected HypermediaValue(Expression<Func<T>> valueResolver) {
            if (valueResolver == null) {
                throw new ArgumentNullException(nameof(valueResolver));
            }
            _valueResolver = valueResolver;
        }

        public bool IsReadonly { get; set; }
        public string Format { get; set; }

        public T Value {
            get {
                if (_valueResolver != null) {
                    return _valueResolver.Compile()();
                }

                return _value;
            }
            set {
                if (_valueResolver != null) {
                    throw new InvalidOperationException("Cannot set value when value resolver is set");
                }
            }
        }

        internal abstract string Type { get; }
    }

    public class NumberValue : HypermediaValue<decimal?> {
        public NumberValue(decimal? value) : base(value) { }

        public NumberValue() { }

        public NumberValue(Expression<Func<decimal?>> valueResolver) : base(valueResolver) { }

        internal override string Type => "number";
    }

    public class StringValue : HypermediaValue<string> {
        public StringValue(string value) : base(value) { }

        public StringValue() { }

        public StringValue(Expression<Func<string>> valueResolver) : base(valueResolver) { }

        internal override string Type => "string";
    }

    public class DateTimeValue : HypermediaValue<DateTimeOffset?> {
        public DateTimeValue(DateTimeOffset? value) : base(value) { }

        public DateTimeValue() { }

        public DateTimeValue(Expression<Func<DateTimeOffset?>> valueResolver) : base(valueResolver) { }

        internal override string Type => "date";
    }
}