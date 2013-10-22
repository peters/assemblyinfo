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

Examples
-----

Determine which framework a .NET assembly is targeting

```cs
// 2.0
var targetFramework = AssemblyInfo.GetTargetFramework("myassembly_net20.dll");

// 4.0
var targetFramework = AssemblyInfo.GetTargetFramework("myassembly_net40.dll");

// 4.5
var targetFramework = AssemblyInfo.GetTargetFramework("myassembly_net45.dll");

// 4.5.1
var targetFramework = AssemblyInfo.GetTargetFramework("myassembly_net451.dll");

```

License
=======
MIT
