using Gordon360;

namespace UnitTestsGordon360
{
    public class UnitTest1
    { 
        [Fact]
        public void Test1_ThisIsNotAGoodName()
        {
            Assert.Equal(1, 1);
        }

        [Theory]
        [InlineData(1)]
        public void IsOdd_OddInteger_ReturnsTrue(int value)
        {
            Assert.True(IsOdd(value));
        }

        [Theory,InlineData(2)]
        public void IsOdd_EvenInteger_ReturnsFalse(int value)
        {
            Assert.False(IsOdd(value));
        }

        bool IsOdd(int value)
        {
            return value % 2 == 1;
        }
    }
}