using System.Collections.Generic;
using System.Reflection;

namespace assemblyinfo.Extensions
{
    public static class AssemblyExtensions
    {
        public static bool IsGreaterThan(this Assembly assembly, TargetFramework framework)
        {
            return AssemblyInfo.GetTargetFramework(assembly).IsGreaterThan(framework);
        }

        public static bool IsEqualTo(this Assembly assembly, TargetFramework framework)
        {
            return AssemblyInfo.GetTargetFramework(assembly).IsEqualTo(framework);
        }

        public static bool IsGreaterThanOrEqualTo(this Assembly assembly, TargetFramework framework)
        {
            return AssemblyInfo.GetTargetFramework(assembly).IsGreaterThanOrEqualTo(framework);
        }

        public static bool IsLessThan(this Assembly assembly, TargetFramework framework)
        {
            return AssemblyInfo.GetTargetFramework(assembly).IsLessThan(framework);
        }

        public static bool IsLessThanOrEqualTo(this Assembly assembly, TargetFramework framework)
        {
            return AssemblyInfo.GetTargetFramework(assembly).IsLessThanOrEqualTo(framework);
        }

        public static bool IsGreaterThan(this IEnumerable<Assembly> assemblies, TargetFramework framework)
        {
            return AssemblyInfo.GetTargetFramework(assemblies).IsGreaterThan(framework);
        }

        public static bool IsGreaterThanOrEqualTo(this IEnumerable<Assembly> assemblies, TargetFramework framework)
        {
            return AssemblyInfo.GetTargetFramework(assemblies).IsGreaterThanOrEqualTo(framework);
        }

        public static bool IsLessThan(this IEnumerable<Assembly> assemblies, TargetFramework framework)
        {
            return AssemblyInfo.GetTargetFramework(assemblies).IsLessThan(framework);
        }

        public static bool IsLessThanOrEqualTo(this IEnumerable<Assembly> assemblies, TargetFramework framework)
        {
            return AssemblyInfo.GetTargetFramework(assemblies).IsLessThan(framework);
        }

    }
}
