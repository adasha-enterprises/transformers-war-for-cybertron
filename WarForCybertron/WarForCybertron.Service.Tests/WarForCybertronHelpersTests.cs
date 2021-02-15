using WarForCybertron.Model;
using WarForCybertron.Service.Helpers;
using Xunit;

namespace WarForCybertron.Service.Tests
{
    public class WarForCybertronHelpersTests
    {
        [Theory]
        [InlineData(9, 5, 3,"autobot")]
        [InlineData(9, 5, 5, "draw")]
        [InlineData(8, 5, 5, "decepticon")]
        public void CanTestForCowardice(int autobotStrength, int decepticonStrength, int decepticonCourage, string expectedOutcome)
        {
            // arrange
            var autobot = new Transformer
            {
                Strength = autobotStrength
            };

            var decepticon = new Transformer
            {
                Strength = decepticonStrength,
                Courage = decepticonCourage
            };

            // act
            var victor = TransformerHelpers.TestForCowardice(autobot, decepticon);
            string result = "draw";

            if (victor != null)
            {
                result = victor == autobot ? "autobot" : "decepticon";
            }

            // Assert
            Assert.Equal(expectedOutcome, result);
        }
    }
}
