using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace assemblyinfo.tests
{
    [TestFixture]
    public class AssemblyInfoTests
    {
        private static readonly string MygetOutputFolder = Path.Combine(typeof(AssemblyInfoTests).Assembly.Location, 
                @"..\..\..\..\..\bin\assemblyinfo\0.1.0\AnyCpu");

        private static readonly string AssemblyName = Path.GetFileName(typeof(AssemblyInfo).Assembly.Location);

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

        [Test]
        public void ShouldReadNet20Framework()
        {
            var tf = AssemblyInfo.GetTargetFramework(Path.Combine(MygetOutputFolder, @"net_2_0_Release\v2.0\" + AssemblyName));
            Assert.That(tf, Is.EqualTo(TargetFramework.Net_2_0));
        }

        [Test]
        public void ShouldReadNet40Framework()
        {
            var tf = AssemblyInfo.GetTargetFramework(Path.Combine(MygetOutputFolder, @"net_4_0_Release\v4.0\" + AssemblyName));
            Assert.That(tf, Is.EqualTo(TargetFramework.Net_4_0));
        }

        [Test]
        public void ShouldReadNet45Framework()
        {
            var tf = AssemblyInfo.GetTargetFramework(Path.Combine(MygetOutputFolder, @"net_4_0_Release\v4.5\" + AssemblyName));
            Assert.That(tf, Is.EqualTo(TargetFramework.Net_4_5));
        }

        [Test]
        public void ShouldReadNet451Framework()
        {
            var tf = AssemblyInfo.GetTargetFramework(Path.Combine(MygetOutputFolder, @"net_4_0_Release\v4.5.1\" + AssemblyName));
            Assert.That(tf, Is.EqualTo(TargetFramework.Net_4_5_1));
        }

    }
}
