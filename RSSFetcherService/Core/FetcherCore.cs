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
        IRSSParser _rssParser;

        public FetcherCore(
            IChannelRepository channelRepository,
            IArticleRepository articleRepository,
            IRSSParser rssParser
        )
        {
            _channelRepository = channelRepository;
            _articleRepository = articleRepository;
            _rssParser = rssParser;
        }

        public async Task<Channel> fetchChannel(string url)
        {
            Channel channel = _channelRepository.GetChannelByURL(url);

            if(channel == null)
            {
                channel = _rssParser.ParseRSS(url);
                _channelRepository.Insert(channel);
                await _channelRepository.Save();
            }
            else if(channel.DateModified.Subtract(DateTime.Now) >
                TimeSpan.FromHours(1.0))
            {
                var fetchedChannel =_rssParser.ParseRSS(url);

                channel.Title = fetchedChannel.Title;
                channel.Description = fetchedChannel.Description;
                channel.Link = fetchedChannel.Link;
                channel.ChannelImage = fetchedChannel.ChannelImage;
                channel.Articles = fetchedChannel.Articles;

                _channelRepository.Update(channel);
                await _channelRepository.Save();
            }

            return channel;
        }
    }
}
