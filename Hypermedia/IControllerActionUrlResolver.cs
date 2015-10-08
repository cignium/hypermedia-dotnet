using System;
using System.Linq.Expressions;

namespace Hypermedia {
    public interface IControllerActionUrlResolver {
        Uri Generate<T>(string controller, Expression<T> controllerAction, object routeValues);
        Uri Generate<T>(Expression<T> controllerAction, object routeValues);
    }
}