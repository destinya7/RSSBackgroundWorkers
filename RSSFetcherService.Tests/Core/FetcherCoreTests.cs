using System.Collections.Generic;
using Moq;
using Xunit;
using RSSBackgroundWorkerBusiness.Repositories;
using RSSFetcherService.Utils;
using RSSBackgroundWorkerBusiness.Models;
using RSSFetcherService.Core;
using System.Threading.Tasks;
using System;
using RSSFetcherService.Services;

namespace RSSFetcherService.Tests.Core
{
    public class FetcherCoreTests
    {
        readonly string xmlString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <rss xmlns:atom=\"http://www.w3.org/2005/Atom\" xmlns:content=\"http://purl.org/rss/1.0/modules/content/\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:media=\"http://search.yahoo.com/mrss/\" version=\"2.0\"> <channel> <title><![CDATA[CNN.com - RSS Channel - App International Edition]]></title> <description><![CDATA[CNN.com delivers up-to-the-minute news and information on the latest top stories, weather, entertainment, politics and more.]]></description> <link>https://www.cnn.com/app-international-edition/index.html</link> <image> <url>http://i2.cdn.turner.com/cnn/2015/images/09/24/cnn.digital.png</url> <title>CNN.com - RSS Channel - App International Edition</title> <link>https://www.cnn.com/app-international-edition/index.html</link> </image> <pubDate>Tue, 24 Dec 2019 02:34:11 GMT</pubDate> <ttl>10</ttl> <atom10:link xmlns:atom10=\"http://www.w3.org/2005/Atom\" rel=\"self\" type=\"application/rss+xml\" href=\"http://rss.cnn.com/rss/edition\" /> <feedburner:info xmlns:feedburner=\"http://rssnamespace.org/feedburner/ext/1.0\" uri=\"rss/edition\" /> <atom10:link xmlns:atom10=\"http://www.w3.org/2005/Atom\" rel=\"hub\" href=\"http://pubsubhubbub.appspot.com/\" /> <item> <title><![CDATA[In Asia Pacific the climate crisis is happening now, not in the future]]></title> <description><![CDATA[The world's most disaster-prone region felt the harsh reality of the climate crisis in 2019.]]></description> <link>https://www.cnn.com/2019/12/23/asia/asia-pacific-climate-crisis-intl-hnk/index.html</link> <guid isPermaLink=\"true\">https://www.cnn.com/2019/12/23/asia/asia-pacific-climate-crisis-intl-hnk/index.html</guid> <pubDate>Tue, 24 Dec 2019 02:50:36 GMT</pubDate> </item> <item> <title><![CDATA[Woman shames would-be porch pirate at neighbor's house]]></title> <description><![CDATA[A would-be porch pirate in Glendale, California, was in for a surprise when he tried to steal a package from someone's porch and their neighbor intervened.]]></description> <link>https://www.cnn.com/videos/us/2019/12/21/neighbor-shames-would-be-porch-pirate-holiday-package-thief-ndwknd-vpx.cnn</link> <guid isPermaLink=\"true\">https://www.cnn.com/videos/us/2019/12/21/neighbor-shames-would-be-porch-pirate-holiday-package-thief-ndwknd-vpx.cnn</guid> <pubDate>Sat, 21 Dec 2019 14:00:33 GMT</pubDate> </item> <item> <title><![CDATA[Police spread Christmas cheer by giving out candy instead of tickets]]></title> <description><![CDATA[Some drivers in Boise, Idaho, have gotten a jolly surprise after seeing flashing blue lights in their rear-view mirrors: police are giving out candy instead of tickets for some minor offenses.]]></description> <link>https://www.cnn.com/2019/12/23/us/boise-police-candy-instead-of-traffic-tickets-trnd/index.html</link> <guid isPermaLink=\"true\">https://www.cnn.com/2019/12/23/us/boise-police-candy-instead-of-traffic-tickets-trnd/index.html</guid> <pubDate>Mon, 23 Dec 2019 21:39:48 GMT</pubDate> </item> </channel> </rss>";
        List<Channel> channels;

        public FetcherCoreTests()
        {
            channels = new List<Channel>
            {
                new Channel
                {
                    Id = 1,
                    Title = "Channel 1",
                    Link = "http://www.channel1.com"
                },
                new Channel
                {
                    Id = 2,
                    Title = "Channel 2",
                    Link = "http://www.channel2.com"
                },
                new Channel
                {
                    Id = 3,
                    Title = "Channel 3",
                    Link = "http://www.channel3.com"
                }
            };
        }

        [Fact]
        public async void FetchChannel_Inserts_Channel()
        {
            Mock<IChannelRepository> channelRepoMoq = new Mock<IChannelRepository>();
            Mock<IArticleRepository> articleRepoMoq = new Mock<IArticleRepository>();
            Mock<IHttpRSSClient> httpRssClientMoq = new Mock<IHttpRSSClient>();
            Mock<IRSSParser> rssParserMoq = new Mock<IRSSParser>();
            Mock<ILoggerService> loggerServiceMoq = new Mock<ILoggerService>();
            bool isChannelSaved = false;

            httpRssClientMoq
                .Setup(client => client.GetRSSXmlString("testurl.com"))
                .ReturnsAsync(xmlString);
            rssParserMoq
                .Setup(parser => parser.ParseRSS(xmlString))
                .Returns(new Channel
                {
                    Id = 0,
                    Title = "Channel 4",
                    Link = "http://www.channel4.com"
                });
            channelRepoMoq
                .Setup(repo => repo.GetAll())
                .ReturnsAsync(channels);
            channelRepoMoq
                .Setup(repo => repo.GetById(channels[0].Id))
                .ReturnsAsync(channels[0]);
            channelRepoMoq
                .Setup(repo => repo.GetChannelByURL(channels[0].Link))
                .Returns(Task.FromResult<Channel>(null));
            channelRepoMoq
                .Setup(repo => repo.Insert(default(Channel)))
                .Callback(() => isChannelSaved = true);

            FetcherCore fetcherCore = new FetcherCore(
                channelRepoMoq.Object,
                articleRepoMoq.Object,
                httpRssClientMoq.Object,
                rssParserMoq.Object,
                loggerServiceMoq.Object);

            await fetcherCore.FetchChannel("http://www.channel4.com");

            Assert.True(isChannelSaved);
        }

        [Fact]
        public async void FetchChannel_Updates_Channel()
        {
            Mock<IChannelRepository> channelRepoMoq = new Mock<IChannelRepository>();
            Mock<IArticleRepository> articleRepoMoq = new Mock<IArticleRepository>();
            Mock<IHttpRSSClient> httpRssClientMoq = new Mock<IHttpRSSClient>();
            Mock<IRSSParser> rssParserMoq = new Mock<IRSSParser>();
            Mock<ILoggerService> loggerServiceMoq = new Mock<ILoggerService>();
            Channel channel = new Channel
            {
                Id = 1,
                Title = "Channel 1",
                Link = "http://www.channel1.com",
                DateModified = DateTime.Now.AddHours(-3.0)
            };
            bool isChannelUpdated = false;

            httpRssClientMoq
                .Setup(client => client.GetRSSXmlString("http://www.channel1.com"))
                .ReturnsAsync(xmlString);
            rssParserMoq
                .Setup(parser => parser.ParseRSS(xmlString))
                .Returns(new Channel
                {
                    Title = "Channel 1",
                    Link = "http://www.channel1.com"
                });
            channelRepoMoq
                .Setup(repo => repo.GetChannelByURL("http://www.channel1.com"))
                .Returns(Task.FromResult<Channel>(channel));
            channelRepoMoq
                .Setup(repo => repo.Update(channel))
                .Callback(() => isChannelUpdated = true);

            FetcherCore fetcherCore = new FetcherCore(
                channelRepoMoq.Object,
                articleRepoMoq.Object,
                httpRssClientMoq.Object,
                rssParserMoq.Object,
                loggerServiceMoq.Object);

            await fetcherCore.FetchChannel("http://www.channel1.com");

            Assert.True(isChannelUpdated);
        }

        [Fact]
        public async void FetchChannel_Recently_Modified()
        {
            Mock<IChannelRepository> channelRepoMoq = new Mock<IChannelRepository>();
            Mock<IArticleRepository> articleRepoMoq = new Mock<IArticleRepository>();
            Mock<IHttpRSSClient> httpRssClientMoq = new Mock<IHttpRSSClient>();
            Mock<IRSSParser> rssParserMoq = new Mock<IRSSParser>();
            Mock<ILoggerService> loggerServiceMoq = new Mock<ILoggerService>();
            Channel channel = new Channel
            {
                Id = 1,
                Title = "Channel 1",
                Link = "http://www.channel1.com",
                DateModified = DateTime.Now.AddHours(-3.0)
            };

            httpRssClientMoq
                .Setup(client => client.GetRSSXmlString("http://www.channel1.com"))
                .ReturnsAsync(xmlString);
            rssParserMoq
                .Setup(parser => parser.ParseRSS(xmlString))
                .Returns(new Channel
                {
                    Title = "Channel 1",
                    Link = "http://www.channel1.com"
                });
            channelRepoMoq
                .Setup(repo => repo.GetChannelByURL("http://www.channel1.com"))
                .Returns(Task.FromResult<Channel>(channel));

            FetcherCore fetcherCore = new FetcherCore(
                channelRepoMoq.Object,
                articleRepoMoq.Object,
                httpRssClientMoq.Object,
                rssParserMoq.Object,
                loggerServiceMoq.Object);

            var resultChannel = await fetcherCore.FetchChannel("http://www.channel1.com");

            Assert.Equal(channel.Id, resultChannel.Id);
        }
    }
}
