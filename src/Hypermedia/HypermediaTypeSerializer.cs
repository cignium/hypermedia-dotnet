using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Hypermedia {
    public class HypermediaTypeSerializer {
        private static readonly Type HypermediaValueType = typeof(HypermediaValue<>);
        private readonly UnresolvedLinkResolver _resolver;
        // Todo make this configureable
        private static readonly JsonSerializer Serializer = new JsonSerializer() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        // Todo support these with attributes later on instead
        private static readonly string[] PropertyIgnoreList = { "Links", "Errors", "Profile", "MediaType", "Properties", "Self" };

        public HypermediaTypeSerializer(UnresolvedLinkResolver resolver) {
            if (resolver == null) {
                throw new ArgumentNullException(nameof(resolver));
            }
            _resolver = resolver;
        }

        public JObject Serialize(object instance) {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }
            return CreateObject(instance.GetType(), new Attribute[] { }, instance);
        }

        private JObject CreateObject(Type type, Attribute[] attributes, object instance) {
            var jObject = CreateEmptyObject(instance);

            var properties = new JArray();
            jObject.Add("properties", properties);

            foreach (var propertyInfo in type.GetProperties()) {
                if (PropertyIgnoreList.Contains(propertyInfo.Name)) {
                    continue;
                }

                var property = GetProperty(propertyInfo, instance);
                property.Add("name", GetPropertyName(propertyInfo));
                properties.Add(property);
            }

            return jObject;
        }

        private JObject GetProperty(PropertyInfo property, object instance) {
            var value = instance == null ? null : property.GetValue(instance);

            if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string)) {
                return CreateObject(property.PropertyType, property.GetCustomAttributes(true).Cast<Attribute>().ToArray(), property.GetValue(instance));
            }

            var hypermediaType = GetOrCreateHypermediaType(property, instance, value);

            AddAttributeSettings(hypermediaType, property);
            return JObject.FromObject(hypermediaType, Serializer);
        }

        private void AddAttributeSettings(IHypermediaType hypermediaType, PropertyInfo property) {
            // Todo read Readonly attributes etc
        }

        private string GetPropertyName(PropertyInfo property) {
            // Todo read display attribute
            return char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
        }

        private JObject CreateEmptyObject(object instance) {
            var jObject = new JObject();
            var hypermediaType = instance as IHypermediaType;

            jObject.Add("links", hypermediaType == null ? new JArray() : JArray.FromObject(_resolver.Resolve(hypermediaType.Links), Serializer));
            jObject.Add("errors", hypermediaType == null ? new JArray() : JArray.FromObject(hypermediaType.Errors, Serializer));

            return jObject;
        }

        private static IHypermediaType GetOrCreateHypermediaType(PropertyInfo property, object instance, object value) {
            var parentHypermediaType = instance as IHypermediaType;

            IHypermediaType hypermediaType;
            if (parentHypermediaType != null && parentHypermediaType.Properties.TryGetValue(property.Name, out hypermediaType)) {
                return hypermediaType;
            }

            if (property.PropertyType == typeof(string)) {
                return new StringValue((string)value);
            }

            if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?)) {
                return new NumberValue((decimal?)value);
            }

            if (property.PropertyType == typeof(DateTimeOffset) || property.PropertyType == typeof(DateTimeOffset?)) {
                return new DateTimeValue((DateTimeOffset?)value);
            }

            throw new NotSupportedException($"Does not support {property.PropertyType} yet.");
        }

        private static bool TryGetHypermediaTypeGenericArgument(Type instanceType, out Type genericType) {
            genericType = null;
            var type = instanceType;
            while (type != null) {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == HypermediaValueType) {
                    genericType = type.GetGenericArguments()[0];
                    return true;
                }

                type = type.BaseType;
            }
            return false;
        }
    }
}