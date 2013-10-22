[assemblyinfo](https://www.nuget.org/packages/assemblyinfo/0.1.0)
============

This small library uses [cecil](https://github.com/jbevain/cecil) to inspect MSIL code found in .NET assemblies
instead of `Assembly.Load` which ensures that you do not have to load it's dependencies
beforehand. This is particularly useful if you just want to know what .NET target framework an
assembly is targeting without running the risk of catching an exception from `Assembly.Load`
because of a missing dependency.

Powershell
==========

A powershell version is available [here](https://gist.github.com/peters/6991125). Please note that
there's a limitation where you cannot differentiate between **NET45** and **NET451**. This is because the
[Microsoft PE and COFF Specification](http://msdn.microsoft.com/en-us/windows/hardware/gg463119.aspx) does not
reveal this information.

Install via nuget
---
```
Install-Package assemblyinfo
```

Supported frameworks
----

NET 4.5.1
NET 4.5
NET 4.0
NET 3.5
NET 2.0

Examples
-----

Additional examples are available [here](https://github.com/peters/assemblyinfo/tree/develop/src/assemblyinfo.tests)

```cs
// Read from disk 
AssemblyInfo.GetTargetFramework("myassembly.dll").IsEqualTo(TargetFramework.Net_2_0);

// Read from byte array
AssemblyInfo.GetTargetFramework(
	File.ReadAllBytes("myassembly.dll")
).IsEqualTo(TargetFramework.Net_2_0);

// Read from stream
AssemblyInfo.GetTargetFramework(
	new MemoryStream(File.ReadAllBytes("myassembly.dll"))
).IsEqualTo(TargetFramework.Net_2_0);

// Read from current assembly
AssemblyInfo.GetTargetFramework(
	Assembly.GetExecutingAssembly()
).IsEqualTo(TargetFramework.Net_2_0);

// Determine minimum supported target framework
AssemblyInfo.GetTargetFramework(new List<string> { 
	"assembly1.dll",
	"assembly2.dll"
}).IsGreaterThan(TargetFramework.Net_2_0);

// Determine minimum supported target framework
AssemblyInfo.GetTargetFramework(new List<string> { 
	"assembly1.dll",
	"assembly2.dll"
}).IsLessThanOrEqualTo(TargetFramework.Net_4_5_1);

// Determine minimum supported target framework
AssemblyInfo.GetTargetFramework(new List<byte[]> { 
	File.ReadAllBytes("assembly1.dll"),
	File.ReadAllBytes("assembly2.dll")
}).IsGreaterThanOrEqualTo(TargetFramework.Net_4_5_1);
```

License
=======
MIT
