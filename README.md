[assemblyinfo]
============

This small library uses Mono cecil to inspect MSIL code found in .NET assemblies
without invoking `Assembly.Load` which enables you to avoid having to load
the assembly's dependencies.

Right now the only supported feature is extracting which .NET framework
an .NET assembly is targeting.

Usage
-----

Overloads for `byte[]` and `Stream` is also available.

```cs
// 2.0
var targetFramework = AssemblyInfo.GetTargetFramework("myassembly_net20.dll");

// 4.0
var targetFramework = AssemblyInfo.GetTargetFramework("myassembly_net40.dll");

// 4.5
var targetFramework = AssemblyInfo.GetTargetFramework("myassembly_net40.dll");

// 4.5.1
var targetFramework = AssemblyInfo.GetTargetFramework("myassembly_net451.dll");

```

License
=======
MIT