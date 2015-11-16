using System;

namespace Hypermedia {
    public abstract class HypermediaValue<T> : HypermediaType<HypermediaValue<T>>, IHypermediaValue {
        private readonly Func<object> _valueResolver;
        private readonly object _value;

        protected HypermediaValue() { }

        protected HypermediaValue(object value) {
            _value = value;
        }

        protected HypermediaValue(Func<object> valueResolver) {
            if (valueResolver == null) {
                throw new ArgumentNullException(nameof(valueResolver));
            }
            _valueResolver = valueResolver;
        }

        public bool IsReadonly { get; set; }
        public string Format { get; set; }

        public object Value {
            get {
                if (_valueResolver != null) {
                    return _valueResolver();
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
        public NumberValue(object value) : base(value) { }

        public NumberValue(Func<object> valueResolver) : base(valueResolver) { }

        internal override string Type => "number";
    }

    public class StringValue : HypermediaValue<string> {
        public StringValue(object value) : base(value) { }


        public StringValue(Func<object> valueResolver) : base(valueResolver) { }

        internal override string Type => "string";
    }

    public class DateTimeValue : HypermediaValue<DateTimeOffset?> {
        public DateTimeValue(object value) : base(value) { }

        public DateTimeValue(Func<object> valueResolver) : base(valueResolver) { }

        internal override string Type => "date";
    }
}