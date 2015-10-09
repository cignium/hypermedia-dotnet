using System.Linq.Expressions;

namespace Hypermedia {
    public interface ILinkFilter {
        bool Filter<T>(string controllerName, Expression<T> expression);
    }
}