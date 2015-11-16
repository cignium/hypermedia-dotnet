using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hypermedia {
    public abstract class HypermediaType {
        protected HypermediaType() {
            Links = new List<UnresolvedLink>();
            Errors = new List<string>();
            Properties = new Dictionary<string, HypermediaType>();
        }

        internal IList<UnresolvedLink> Links { get; }
        internal IList<string> Errors { get; }
        internal IDictionary<string, HypermediaType> Properties { get; }

        internal void ConfigureProperty<THypermediaType>(PropertyInfo propertyInfo, Func<object> valueResolver, Action<THypermediaType> configureAction) where THypermediaType : HypermediaType {
            var property = (THypermediaType)ConfigureProperty(propertyInfo, valueResolver);
            configureAction(property);
        }

        internal HypermediaType ConfigureProperty(PropertyInfo propertyInfo, Func<object> value) {
            var property = CreateHypermediaType(propertyInfo.PropertyType, value);
            Properties.Add(propertyInfo.Name, property);
            // Read and add attribute data

            return property;
        }

        protected THypermediaType CreateAndAddProperty<THypermediaType>(string name, Func<object> valueResolver) where THypermediaType : HypermediaType {
            var type = (THypermediaType)Activator.CreateInstance(typeof(THypermediaType), valueResolver);
            Properties.Add(name, type);
            return type;
        }

        private static HypermediaType CreateHypermediaType(Type type, Func<object> value) {
            if (type == typeof(string)) {
                return new StringValue(value);
            }

            if (type == typeof(decimal) || type == typeof(decimal?) || type == typeof(int?) || type == typeof(int)) {
                return new NumberValue(value);
            }

            if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?)) {
                return new DateTimeValue(value);
            }

            if (type == typeof(bool) || type == typeof(bool?)) {
                return new BooleanValue(value);
            }

            throw new NotSupportedException(type.ToString());
        }
    }
}