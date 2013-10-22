using System.Collections.Generic;
#if NET_3_5 || NET_4_0
using System.Linq;
#else 
#endif

namespace assemblyinfo.Extensions
{

    public static class TargetFrameworkExtensions
    {
        public static bool IsGreaterThan(this TargetFramework a, TargetFramework b)
        {
            return (int)a > (int)b;
        }

        public static bool IsEqualTo(this TargetFramework a, TargetFramework b)
        {
            return (int) a == (int) b;
        }

        public static bool IsGreaterThanOrEqualTo(this TargetFramework a, TargetFramework b)
        {
            return a.IsGreaterThan(b) || a.IsEqualTo(b);
        }

        public static bool IsLessThan(this TargetFramework a, TargetFramework b)
        {
            return !a.IsGreaterThan(b) && !a.IsEqualTo(b);
        }

        public static bool IsLessThanOrEqualTo(this TargetFramework a, TargetFramework b)
        {
            return a.IsLessThan(b) || a.IsEqualTo(b);
        }

        public static bool IsGreaterThan(this IEnumerable<TargetFramework> frameworks, TargetFramework framework)
        {
            return frameworks.Any(currentFramework => currentFramework.IsGreaterThan(framework));
        }

        public static bool IsGreaterThanOrEqualTo(this IEnumerable<TargetFramework> frameworks, TargetFramework framework)
        {
            return frameworks.Any(currentFramework => currentFramework.IsGreaterThanOrEqualTo(framework));
        }

        public static bool IsLessThan(this IEnumerable<TargetFramework> frameworks, TargetFramework framework)
        {
            return frameworks.Max(x => (int) x) < (int) framework;
        }

        public static bool IsLessThanOrEqualTo(this IEnumerable<TargetFramework> frameworks, TargetFramework framework)
        {
            return frameworks.All(currentFramework => currentFramework.IsLessThanOrEqualTo(framework));
        }

    }
}
