using System.Collections.Generic;
using assemblyinfo.Extensions;
using NUnit.Framework;

namespace assemblyinfo.tests
{
    [TestFixture]
    public class TargetFrameworkExtensionsTest
    {

        private static List<TargetFramework> Frameworks = new List<TargetFramework>
        {
            TargetFramework.Net_2_0,
            TargetFramework.Net_4_0,
            TargetFramework.Net_4_5,
            TargetFramework.Net_4_5_1
        };

        [Test]
        public void TestIsGreaterThan()
        {
            Assert.True(TargetFramework.Net_4_5.IsGreaterThan(TargetFramework.Net_2_0));
            Assert.False(TargetFramework.Net_4_5.IsGreaterThan(TargetFramework.Net_4_5));

            Assert.True(Frameworks.IsGreaterThan(TargetFramework.Net_2_0));
            Assert.True(Frameworks.IsGreaterThan(TargetFramework.Net_4_0));
            Assert.True(Frameworks.IsGreaterThan(TargetFramework.Net_4_5));
            Assert.False(Frameworks.IsGreaterThan(TargetFramework.Net_4_5_1));
        }

        [Test]
        public void TestIsEqualTo()
        {
            Assert.True(TargetFramework.Net_4_5.IsEqualTo(TargetFramework.Net_4_5));
            Assert.False(TargetFramework.Net_4_5.IsEqualTo(TargetFramework.Net_2_0));
        }

        [Test]
        public void TestIsGreaterThanOrEqualTo()
        {
            Assert.True(TargetFramework.Net_4_5.IsGreaterThanOrEqualTo(TargetFramework.Net_2_0));
            Assert.True(TargetFramework.Net_4_5.IsGreaterThanOrEqualTo(TargetFramework.Net_4_5));
            Assert.False(TargetFramework.Net_4_5.IsGreaterThanOrEqualTo(TargetFramework.Net_4_5_1));

            Assert.True(Frameworks.IsGreaterThanOrEqualTo(TargetFramework.Net_2_0));
            Assert.True(Frameworks.IsGreaterThanOrEqualTo(TargetFramework.Net_4_0));
            Assert.True(Frameworks.IsGreaterThanOrEqualTo(TargetFramework.Net_4_5));
            Assert.True(Frameworks.IsGreaterThanOrEqualTo(TargetFramework.Net_4_5_1));
        }

        [Test]
        public void TestIsLessThan()
        {
            Assert.False(TargetFramework.Net_4_5.IsLessThan(TargetFramework.Net_2_0));
            Assert.True(TargetFramework.Net_4_5.IsLessThan(TargetFramework.Net_4_5_1));
            Assert.False(TargetFramework.Net_4_5.IsLessThan(TargetFramework.Net_4_5));

            Assert.False(Frameworks.IsLessThan(TargetFramework.Net_2_0));
            Assert.False(Frameworks.IsLessThan(TargetFramework.Net_4_0));
            Assert.False(Frameworks.IsLessThan(TargetFramework.Net_4_5));
            Assert.False(Frameworks.IsLessThan(TargetFramework.Net_4_5_1));

            Assert.True(new List<TargetFramework>
            {
                TargetFramework.Net_2_0, 
                TargetFramework.Net_4_0
            }.IsLessThan(TargetFramework.Net_4_5));
        }

        [Test]
        public void TestIsLessThanOrEqualTo()
        {
            Assert.False(TargetFramework.Net_4_5.IsLessThanOrEqualTo(TargetFramework.Net_2_0));
            Assert.True(TargetFramework.Net_4_5.IsLessThanOrEqualTo(TargetFramework.Net_4_5_1));
            Assert.True(TargetFramework.Net_4_5.IsLessThanOrEqualTo(TargetFramework.Net_4_5));

            Assert.False(Frameworks.IsLessThanOrEqualTo(TargetFramework.Net_2_0));
            Assert.False(Frameworks.IsLessThanOrEqualTo(TargetFramework.Net_4_0));
            Assert.False(Frameworks.IsLessThanOrEqualTo(TargetFramework.Net_4_5));
            Assert.True(Frameworks.IsLessThanOrEqualTo(TargetFramework.Net_4_5_1));

            Assert.True(new List<TargetFramework>
            {
                TargetFramework.Net_2_0, 
                TargetFramework.Net_4_0,
                TargetFramework.Net_4_5
            }.IsLessThanOrEqualTo(TargetFramework.Net_4_5));
        }

    }
}
