using FluentAssertions;
using Xunit;

namespace LogglyAPI.Tests
{
    public class EventsTest : BaseTest
    {
        [Fact]
        public async void GetEventsReturnsPopulatedObject()
        {
            var searchResult = await _logglyClient.Search("*");
            var eventsResult = await _logglyClient.GetEvents(searchResult.Id);

            eventsResult.Should().NotBeNull();
            eventsResult.TotalEvents.Should().BePositive();
            eventsResult.Page.Should().Be(0);
            eventsResult.Events.Should().NotBeEmpty();
        }
    }
}
