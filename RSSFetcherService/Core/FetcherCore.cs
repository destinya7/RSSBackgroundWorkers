using RSSBackgroundWorkerBusiness.Models;
using RSSBackgroundWorkerBusiness.Repositories;
using RSSFetcherService.Utils;
using System;
using System.Threading.Tasks;

namespace RSSFetcherService.Core
{
    public class FetcherCore
    {
        IChannelRepository _channelRepository;
        IArticleRepository _articleRepository;
        IHttpRSSClient _httpRssClient;
        IRSSParser _rssParser;

        public FetcherCore(
            IChannelRepository channelRepository,
            IArticleRepository articleRepository,
            IHttpRSSClient httpRssClient,
            IRSSParser rssParser
        )
        {
            _channelRepository = channelRepository;
            _articleRepository = articleRepository;
            _httpRssClient = httpRssClient;
            _rssParser = rssParser;
        }

        public async Task<Channel> fetchChannel(string url)
        {
            Channel channel = null;

            try
            {
                channel = _channelRepository.GetChannelByURL(url);

                if (channel == null)
                {
                    string xmlString = await _httpRssClient.GetRSSXmlString(url);
                    channel = _rssParser.ParseRSS(xmlString);
                    _channelRepository.Insert(channel);
                    await _channelRepository.Save();
                }
                else if (channel.DateModified.Subtract(DateTime.Now) >
                    TimeSpan.FromHours(1.0))
                {
                    string xmlString = await _httpRssClient.GetRSSXmlString(url);
                    Channel fetchedChannel = _rssParser.ParseRSS(xmlString);

                    channel.Title = fetchedChannel.Title;
                    channel.Description = fetchedChannel.Description;
                    channel.Link = fetchedChannel.Link;
                    channel.ChannelImage = fetchedChannel.ChannelImage;
                    channel.Articles = fetchedChannel.Articles;

                    _channelRepository.Update(channel);
                    await _channelRepository.Save();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return channel;
        }
    }
}
