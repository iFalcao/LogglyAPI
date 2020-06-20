using FluentAssertions;
using System.Linq;
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

        [Fact]
        public async void GetRawEventsReturnPopulatedObjectList()
        {
            var searchResult = await _logglyClient.Search("*");
            var eventsResult = await _logglyClient.GetRawEvents<object>(searchResult.Id);

            eventsResult.Should().NotBeNull();
            eventsResult.Count().Should().Be(50);
        }
    }
}
