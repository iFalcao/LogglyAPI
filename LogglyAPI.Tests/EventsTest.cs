using FluentAssertions;
using LogglyAPI.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LogglyAPI.Tests
{
    public class EventsTest : BaseTest
    {
        [Fact]
        public async void GetEventsReturnsPopulatedObject()
        {
            var searchResult = await this._logglyClient.Search("*");
            var eventsResult = await this._logglyClient.GetEventsByRsid(searchResult.RSID.Id);

            eventsResult.Should().NotBeNull();
            eventsResult.TotalEvents.Should().BePositive();
            eventsResult.Page.Should().Be(0);
            eventsResult.Events.Should().NotBeEmpty();
        }

        [Fact]
        public async void GetRawEventsReturnPopulatedObjectList()
        {
            var searchResult = await this._logglyClient.Search("*");
            var eventsResult = await this._logglyClient.GetRawEventsByRsid<object>(searchResult.RSID.Id);

            eventsResult.Should().NotBeNull();
            eventsResult.Count().Should().Be(50);
        }

        [Fact]
        public void EventsIteratorPageSizeInvalidValueThrowsException()
        {
            Func<Task> getEventsIteratorAction = async () =>
            {
                await this._logglyClient.GetEventsIterator(
                    "*",
                    from: DateParameter.FromTimeInterval(TimeInterval.Minutes, 5),
                    until: DateParameter.DEFAULT_UNTIL,
                    pageSize: 1001);
            };

            getEventsIteratorAction.Should().Throw<ArgumentException>();
        }

        [Fact]
        public async void EventsIteratorReturnsPopulatedObjectWithoutNextPageUrl()
        {
            var eventsIterator = await this._logglyClient.GetEventsIterator(
                "*",
                from: DateParameter.FromTimeInterval(TimeInterval.Minutes, 2),
                until: DateParameter.DEFAULT_UNTIL,
                pageSize: 1000);

            eventsIterator.Should().NotBeNull();
            eventsIterator.NextPageUrl.Should().BeNull();
            eventsIterator.Events.Count().Should().BePositive();
        }

        [Fact]
        public async void EventsIteratorReturnsPopulatedObject()
        {
            var eventsIterator = await this._logglyClient.GetEventsIterator(
                "*",
                from: DateParameter.FromTimeInterval(TimeInterval.Days, 2),
                until: DateParameter.DEFAULT_UNTIL,
                pageSize: 1);

            eventsIterator.Should().NotBeNull();
            eventsIterator.NextPageUrl.Should().NotBeNull();
            eventsIterator.Events.Count().Should().BePositive();
        }
    }
}
