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
            response.RSID.Id.Should().BePositive();
            response.RSID.ElapsedTime.Should().BePositive();
            response.RSID.Status.Length.Should().BePositive();
            response.RSID.DateFrom.Should().BeBefore(response.RSID.DateTo);
        }
    }
}
