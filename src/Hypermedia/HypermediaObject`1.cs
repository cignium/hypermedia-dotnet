using System;
using System.Linq.Expressions;

namespace Hypermedia {
    public class HypermediaObject<T> : HypermediaType<T> where T : HypermediaType<T> {

        public T ConfigureProperty(Expression<Func<T, decimal?>> memberExpression, Action<NumberValue> configureAction) {
            return GenericConfigureProperty(memberExpression, configureAction);
        }

        public T ConfigureProperty(Expression<Func<T, decimal>> memberExpression, Action<NumberValue> configureAction) {
            return GenericConfigureProperty(memberExpression, configureAction);
        }

        public T ConfigureProperty(Expression<Func<T, int>> memberExpression, Action<NumberValue> configureAction) {
            return GenericConfigureProperty(memberExpression, configureAction);
        }

        public T ConfigureProperty(Expression<Func<T, int?>> memberExpression, Action<NumberValue> configureAction) {
            return GenericConfigureProperty(memberExpression, configureAction);
        }

        public T ConfigureProperty(Expression<Func<T, string>> memberExpression, Action<StringValue> configureAction) {
            return GenericConfigureProperty(memberExpression, configureAction);
        }

        public T ConfigureProperty(Expression<Func<T, DateTimeOffset?>> memberExpression, Action<DateTimeValue> configureAction) {
            return GenericConfigureProperty(memberExpression, configureAction);
        }

        public T ConfigureProperty(Expression<Func<T, DateTimeOffset>> memberExpression, Action<DateTimeValue> configureAction) {
            return GenericConfigureProperty(memberExpression, configureAction);
        }

        public T ConfigureProperty(Expression<Func<T, bool?>> memberExpression, Action<BooleanValue> configureAction) {
            return GenericConfigureProperty(memberExpression, configureAction);
        }

        public T ConfigureProperty(Expression<Func<T, bool>> memberExpression, Action<BooleanValue> configureAction) {
            return GenericConfigureProperty(memberExpression, configureAction);
        }

        public T ConfigureProperty<THypermediaType>(string name, object value, Action<THypermediaType> configureAction) where THypermediaType : HypermediaType {
            var property = CreateAndAddProperty<THypermediaType>(name, () => value);
            configureAction(property);
            return Self;
        }

        private T GenericConfigureProperty<THypermediaType, TType>(Expression<Func<T, TType>> memberExpression, Action<THypermediaType> configureAction) where THypermediaType : HypermediaType {
            var expression = (MemberExpression)memberExpression.Body;
            var propertyInfo = GetType().GetProperty(expression.Member.Name);

            Func<object> valueResolver = () => propertyInfo.GetValue(this);
            ConfigureProperty(propertyInfo, valueResolver, configureAction);
            return Self;
        }
    }
}