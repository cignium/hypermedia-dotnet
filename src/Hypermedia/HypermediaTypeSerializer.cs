using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Hypermedia {
    public class HypermediaTypeSerializer {
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
            var hypermediaObject = instance as HypermediaType ?? new HypermediaObject();

            var addedProperties = new Dictionary<string, string>();

            foreach (var propertyInfo in type.GetProperties()) {
                if (PropertyIgnoreList.Contains(propertyInfo.Name)) {
                    continue;
                }

                var property = GetProperty(propertyInfo, instance, hypermediaObject);

                addedProperties.Add(propertyInfo.Name, propertyInfo.Name);
                property.Add("name", ToCamelCase(propertyInfo.Name));
                properties.Add(property);
            }

            foreach (var hypermediaProperty in hypermediaObject.Properties) {
                if (addedProperties.ContainsKey(hypermediaProperty.Key)) {
                    continue;
                }

                var property = SerializeValueType(hypermediaProperty.Value);
                property.Add("name", ToCamelCase(hypermediaProperty.Key));
                properties.Add(property);
            }

            return jObject;
        }

        private JObject GetProperty(PropertyInfo property, object instance, HypermediaType hypermediaInstance) {
            var value = instance != null ? property.GetValue(instance) : null;

            if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string)) {
                return CreateObject(property.PropertyType, property.GetCustomAttributes(true).Cast<Attribute>().ToArray(), value);
            }

            HypermediaType type;
            if (!hypermediaInstance.Properties.TryGetValue(property.Name, out type)) {
                type = hypermediaInstance.ConfigureProperty(property, () => value);
            }

            return SerializeValueType(type);
        }

        private JObject SerializeValueType(HypermediaType type) {
            var jObject = JObject.FromObject(type, Serializer);

            jObject.Add("links", JArray.FromObject(_resolver.Resolve(type.Links), Serializer));
            jObject.Add("errors", JArray.FromObject(type.Errors, Serializer));

            return jObject;
        }

        private static string ToCamelCase(string name) {
            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }

        private JObject CreateEmptyObject(object instance) {
            var jObject = new JObject();
            var hypermediaType = instance as HypermediaType;

            jObject.Add("links", hypermediaType == null ? new JArray() : JArray.FromObject(_resolver.Resolve(hypermediaType.Links), Serializer));
            jObject.Add("errors", hypermediaType == null ? new JArray() : JArray.FromObject(hypermediaType.Errors, Serializer));

            return jObject;
        }
    }
}