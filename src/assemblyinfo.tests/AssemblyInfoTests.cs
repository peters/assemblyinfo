using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using assemblyinfo.Extensions;
using NUnit.Framework;

namespace assemblyinfo.tests
{
    [TestFixture]
    public class AssemblyInfoTests
    {
        internal static readonly string MygetOutputFolder = Path.Combine(typeof(AssemblyInfoTests).Assembly.Location,
                @"..\..\..\..\..\bin\assemblyinfo\0.1.0\AnyCpu");

        internal static readonly string AssemblyName = Path.GetFileName(typeof(AssemblyInfo).Assembly.Location);

        internal static readonly List<string> Assemblies = new List<string>
        {
            Path.Combine(MygetOutputFolder, @"net_2_0_Release\v2.0\" + AssemblyName),
            Path.Combine(MygetOutputFolder, @"net_4_0_Release\v4.0\" + AssemblyName),
            Path.Combine(MygetOutputFolder, @"net_4_0_Release\v4.5\" + AssemblyName),
            Path.Combine(MygetOutputFolder, @"net_4_0_Release\v4.5.1\" + AssemblyName)
        };

        private static List<byte[]> AssembliesByteArray;
        private static List<Stream> AssembliesStream;

        internal static readonly List<TargetFramework> AssembliesTargetFramework = new List<TargetFramework>
        {
            TargetFramework.Net_2_0,
            TargetFramework.Net_4_0,
            TargetFramework.Net_4_5,
            TargetFramework.Net_4_5_1
        };

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            // NB! If running tests with resharper you need to disable shadow-copying
            // http://www.jetbrains.com/resharper/webhelp/Reference__Options__Tools__Unit_Testing.html
            if (!Directory.Exists(MygetOutputFolder))
            {
                // please build project before you run tests
                // powershell > .\myget.ps1 -packageVersion 0.1.0
                Debugger.Break();
            }
        }

        [SetUp]
        public void SetUp()
        {
            if (AssembliesByteArray == null)
            {
                AssembliesByteArray = new List<byte[]>
                {
                    File.ReadAllBytes(Assemblies[0]),
                    File.ReadAllBytes(Assemblies[1]),
                    File.ReadAllBytes(Assemblies[2]),
                    File.ReadAllBytes(Assemblies[3])
                };
            }
            AssembliesStream = new List<Stream>
            {
                new MemoryStream(AssembliesByteArray[0]),
                new MemoryStream(AssembliesByteArray[1]),
                new MemoryStream(AssembliesByteArray[2]),
                new MemoryStream(AssembliesByteArray[3])
            };
        }

        [Test]
        public void ShouldReadNet20Framework()
        {
            var tf = AssemblyInfo.GetTargetFramework(Assemblies[0]);
            var tfByteArray = AssemblyInfo.GetTargetFramework(AssembliesByteArray[0]);
            var tfStream = AssemblyInfo.GetTargetFramework(AssembliesStream[0]);
        
            Assert.That(tf, Is.EqualTo(TargetFramework.Net_2_0));
            Assert.That(tfByteArray, Is.EqualTo(TargetFramework.Net_2_0));
            Assert.That(tfStream, Is.EqualTo(TargetFramework.Net_2_0));
        }

        [Test]
        public void ShouldReadNet40Framework()
        {
            var tf = AssemblyInfo.GetTargetFramework(Assemblies[1]);
            var tfByteArray = AssemblyInfo.GetTargetFramework(AssembliesByteArray[1]);
            var tfStream = AssemblyInfo.GetTargetFramework(AssembliesStream[1]);

            Assert.That(tf, Is.EqualTo(TargetFramework.Net_4_0));
            Assert.That(tfByteArray, Is.EqualTo(TargetFramework.Net_4_0));
            Assert.That(tfStream, Is.EqualTo(TargetFramework.Net_4_0));
        }

        [Test]
        public void ShouldReadNet45Framework()
        {
            var tf = AssemblyInfo.GetTargetFramework(Assemblies[2]);
            var tfByteArray = AssemblyInfo.GetTargetFramework(AssembliesByteArray[2]);
            var tfStream = AssemblyInfo.GetTargetFramework(AssembliesStream[2]);

            Assert.That(tf, Is.EqualTo(TargetFramework.Net_4_5));
            Assert.That(tfByteArray, Is.EqualTo(TargetFramework.Net_4_5));
            Assert.That(tfStream, Is.EqualTo(TargetFramework.Net_4_5));
        }

        [Test]
        public void ShouldReadNet451Framework()
        {
            var tf = AssemblyInfo.GetTargetFramework(Assemblies[3]);
            var tfByteArray = AssemblyInfo.GetTargetFramework(AssembliesByteArray[3]);
            var tfStream = AssemblyInfo.GetTargetFramework(AssembliesStream[3]);

            Assert.That(tf, Is.EqualTo(TargetFramework.Net_4_5_1));
            Assert.That(tfByteArray, Is.EqualTo(TargetFramework.Net_4_5_1));
            Assert.That(tfStream, Is.EqualTo(TargetFramework.Net_4_5_1));
        }

        [Test]
        public void ShouldReadNetFrameworkFromMultipleAssemblies()
        {
            CollectionAssert.AreEqual(AssemblyInfo.GetTargetFramework(Assemblies), AssembliesTargetFramework);
            CollectionAssert.AreEqual(AssemblyInfo.GetTargetFramework(AssembliesByteArray), AssembliesTargetFramework);
            CollectionAssert.AreEqual(AssemblyInfo.GetTargetFramework(AssembliesStream), AssembliesTargetFramework);
        }

        [Test]
        public void ShouldDetermineMinimumSupportedTargetFramework()
        {
            Assert.That(AssemblyInfo.GetTargetFramework(Assemblies).IsGreaterThan(TargetFramework.Net_2_0), Is.EqualTo(true));
            Assert.That(AssemblyInfo.GetTargetFramework(AssembliesByteArray).IsGreaterThan(TargetFramework.Net_2_0), Is.EqualTo(true));
            Assert.That(AssemblyInfo.GetTargetFramework(AssembliesStream).IsGreaterThan(TargetFramework.Net_2_0), Is.EqualTo(true)); SetUp();

            Assert.That(AssemblyInfo.GetTargetFramework(Assemblies).IsGreaterThanOrEqualTo(TargetFramework.Net_4_5_1), Is.EqualTo(true));
            Assert.That(AssemblyInfo.GetTargetFramework(AssembliesByteArray).IsGreaterThanOrEqualTo(TargetFramework.Net_4_5_1), Is.EqualTo(true));
            Assert.That(AssemblyInfo.GetTargetFramework(AssembliesStream).IsGreaterThanOrEqualTo(TargetFramework.Net_4_5_1), Is.EqualTo(true)); SetUp();
        }

        [Test]
        public void TestReadTargetFrameworkFromAssembly()
        {
#if NET20
            Assert.That(AssemblyInfo.GetTargetFramework(Assembly.GetExecutingAssembly()), Is.GreaterThanOrEqualTo(TargetFramework.Net_2_0));
#else
            Assert.That(AssemblyInfo.GetTargetFramework(Assembly.GetExecutingAssembly()), Is.GreaterThanOrEqualTo(TargetFramework.Net_4_0));
#endif
        }

    }
}
