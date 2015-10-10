using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Hypermedia {
    public class HypermediaTypeSerializer {
        private static readonly Type HypermediaValueType = typeof(HypermediaValue<>);
        private readonly UnResolvedLinkResolver _resolver;
        // Todo make this configureable
        private static readonly JsonSerializer Serializer = new JsonSerializer() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        // Todo support these with attributes later on instead
        private static readonly string[] PropertyIgnoreList = new[] { "Links", "Errors", "Profile", "MediaType" };

        public HypermediaTypeSerializer(UnResolvedLinkResolver resolver) {
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

            if (property.PropertyType.IsValueType || property.PropertyType == typeof(string)) {
                // Todo create switch case for performance
                var genericHypermediaType = HypermediaValueType.MakeGenericType(property.PropertyType);
                var hypermediaType = (IHypermediaType)Activator.CreateInstance(genericHypermediaType, value);

                AddAttributeSettings(hypermediaType, property);
                return JObject.FromObject(hypermediaType, Serializer);
            }

            Type genericArgumentType;
            if (TryGetHypermediaTypeGenericArgument(value?.GetType() ?? property.PropertyType, out genericArgumentType)) {
                var hypermediaType = (IHypermediaType)(value ?? Activator.CreateInstance(property.PropertyType, genericArgumentType == typeof(string) ? null : Activator.CreateInstance(genericArgumentType)));
                AddAttributeSettings(hypermediaType, property);
                return JObject.FromObject(hypermediaType, Serializer);
            }

            return CreateObject(property.PropertyType, property.GetCustomAttributes(true).Cast<Attribute>().ToArray(), property.GetValue(instance));
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