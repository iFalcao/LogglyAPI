using LogglyAPI.Services;
using Xunit;

namespace LogglyAPI.Tests
{
    public class SearchTest : BaseTest
    {
        [Fact]
        public async void SearchMessagesReturnsPopulatedObject()
        {
            var logglyClient = new LogglyClient(_logglyConfig);
            var response = await logglyClient.Search("*");
            Assert.NotNull(response);
            Assert.InRange(response.Id, 1, long.MaxValue);
            Assert.InRange(response.DateFrom, 1, long.MaxValue);
            Assert.InRange(response.DateTo, 1, long.MaxValue);
            Assert.InRange(response.ElapsedTime, 0.00000000000000001M, decimal.MaxValue);
            Assert.True(response.Status.Length > 0);
        }
    }
}
