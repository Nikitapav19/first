using Xunit;
using task11;

namespace task11tests
{
    public class CalculatorTests
    {
        [Fact]
        public void TestDynamicCalculatorMethods()
        {
            ICalculator calc = CalculatorGenerator.CreateCalculator();

            Assert.Equal(5, calc.Add(2, 3));
            Assert.Equal(-1, calc.Minus(2, 3));
            Assert.Equal(20, calc.Mul(4, 5));
            Assert.Equal(2, calc.Div(10, 5));
        }
    }
}