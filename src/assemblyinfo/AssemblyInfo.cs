using System.IO;
using Mono.Cecil;

namespace assemblyinfo
{

    public enum TargetFramework
    {
        Net_1_0 = TargetRuntime.Net_1_0,
        Net_1_1 = TargetRuntime.Net_1_1,
        Net_2_0 = TargetRuntime.Net_2_0,
        Net_4_0 = TargetRuntime.Net_4_0,
        Net_4_5 = TargetRuntime.Net_4_0 << 1,
        Net_4_5_1 = TargetRuntime.Net_4_0 << 2
    }

    public static class AssemblyInfo
    {
        public static TargetFramework GetTargetFramework(string filename)
        {
            using (var ms = new MemoryStream(File.ReadAllBytes(filename)))
            {
                return GetTargetFramework(ms);
            }
        }

        public static TargetFramework GetTargetFramework(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            {
                return GetTargetFramework(ms);
            }
        }

        public static TargetFramework GetTargetFramework(Stream stream)
        {
            var platformAsm = AssemblyDefinition.ReadAssembly(stream);
            foreach (var attr in platformAsm.CustomAttributes)
            {
                if (attr.AttributeType.FullName != "System.Runtime.Versioning.TargetFrameworkAttribute") continue;
                var targetFrameworkVersion = attr.Properties[0].Argument.Value.ToString();
                if (targetFrameworkVersion.StartsWith(".NET Framework 4.5.1"))
                {
                    return TargetFramework.Net_4_5_1;
                }
                if (targetFrameworkVersion.StartsWith(".NET Framework 4.5"))
                {
                    return TargetFramework.Net_4_5;
                }
            }
            return (TargetFramework)platformAsm.MainModule.Runtime;
        }

    }
}