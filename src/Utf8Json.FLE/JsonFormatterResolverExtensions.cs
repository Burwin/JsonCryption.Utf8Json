using Utf8Json.Resolvers;

namespace Utf8Json.FLE
{
    internal static class JsonFormatterResolverExtensions
    {
        public static bool AllowsPrivate(this IJsonFormatterResolver formatterResolver)
        {
            var thisType = formatterResolver.GetType();
            return thisType == StandardResolver.AllowPrivate.GetType()
                || thisType == StandardResolver.AllowPrivateCamelCase.GetType()
                || thisType == StandardResolver.AllowPrivateExcludeNull.GetType()
                || thisType == StandardResolver.AllowPrivateExcludeNullCamelCase.GetType()
                || thisType == StandardResolver.AllowPrivateExcludeNullSnakeCase.GetType()
                || thisType == StandardResolver.AllowPrivateSnakeCase.GetType();
        }
    }
}
