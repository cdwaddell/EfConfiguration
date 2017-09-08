using System;
using Xunit;

namespace EfConfiguration.Tests
{
    public class UnitTest1
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void Test1()
        {
            Assert.NotNull(new object());
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Test2()
        {
            Assert.Null(new object());
        }
    }
}
