using FluentAssertions;
using Xunit;

namespace LogglyAPI.Tests
{
    public class SearchTest : BaseTest
    {
        [Fact]
        public async void SearchMessagesReturnsPopulatedObject()
        {
            var response = await _logglyClient.Search("*");

            response.Should().NotBeNull();
            response.Id.Should().BePositive();
            response.ElapsedTime.Should().BePositive();
            response.Status.Length.Should().BePositive();
            response.DateFrom.Should().BeBefore(response.DateTo);
        }
    }
}
