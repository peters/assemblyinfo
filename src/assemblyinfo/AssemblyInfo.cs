using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                return GetTargetFramework(fileStream);
            }
        }

        public static TargetFramework GetTargetFramework(byte[] file)
        {
            using (var ms = new MemoryStream(file))
            {
                return GetTargetFramework(ms);
            }
        }

        public static TargetFramework GetTargetFramework(Stream file)
        {
            var platformAsm = AssemblyDefinition.ReadAssembly(file);
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

        public static TargetFramework GetTargetFramework(Assembly assembly)
        {
            using (var fileStream = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                return GetTargetFramework(fileStream);                
            }
        }

        public static IEnumerable<TargetFramework> GetTargetFramework(IEnumerable<string> filenames)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var filename in filenames)
            {
                yield return GetTargetFramework(filename);
            }
        }

        public static IEnumerable<TargetFramework> GetTargetFramework(IEnumerable<byte[]> files)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var byteArray in files)
            {
                yield return GetTargetFramework(byteArray);
            }
        }

        public static IEnumerable<TargetFramework> GetTargetFramework(IEnumerable<Stream> files)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var stream in files)
            {
                yield return GetTargetFramework(stream);
            }
        }

        public static IEnumerable<TargetFramework> GetTargetFramework(IEnumerable<Assembly> files)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var stream in files)
            {
                yield return GetTargetFramework(stream);
            }
        }

    }
}