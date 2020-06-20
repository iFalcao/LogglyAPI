using FluentAssertions;
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

            response.Should().NotBeNull();
            response.Id.Should().BePositive();
            response.ElapsedTime.Should().BePositive();
            response.Status.Length.Should().BeGreaterThan(0);
            response.DateFrom.Should().BeBefore(response.DateTo);
        }
    }
}
