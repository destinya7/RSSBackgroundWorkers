using RSSBackgroundWorkerBusiness.Models;
using RSSBackgroundWorkerBusiness.Repositories;
using RSSFetcherService.Services;
using RSSFetcherService.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RSSFetcherService.Core
{
    public class FetcherCore : IFetcherCore
    {
        IChannelRepository _channelRepository;
        IArticleRepository _articleRepository;
        IHttpRSSClient _httpRssClient;
        IRSSParser _rssParser;
        ILoggerService _logger;

        public FetcherCore(
            IChannelRepository channelRepository,
            IArticleRepository articleRepository,
            IHttpRSSClient httpRssClient,
            IRSSParser rssParser,
            ILoggerService logger
        )
        {
            _channelRepository = channelRepository;
            _articleRepository = articleRepository;
            _httpRssClient = httpRssClient;
            _rssParser = rssParser;
            _logger = logger;
        }

        public async Task<Channel> FetchChannel(string url)
        {
            Channel channel = null;

            try
            {
                _logger.Info($"Fetching channel for url {url}");

                channel = await _channelRepository.GetByUrl(url);

                _logger.Info($"Done fetching channel for url {url}");

                if (channel == null)
                {
                    _logger.Info($"Channel for url {url} not found. Fetching rss xml");

                    string xmlString = await _httpRssClient.GetRSSXmlString(url);

                    _logger.Info($"Done fetching rss xml for url {url}. Parsing xml");

                    channel = _rssParser.ParseRSS(xmlString);

                    _logger.Info($"Done parsing xml. Inserting new channel for url {url}");

                    channel.RSS_URL = url;
                    
                    var result = await _channelRepository.Add(channel);

                    _logger.Info($"Done Inserting saving new channel for url {url} "
                        + $" Status: {result.Status.ToString()}");

                    if (result.Exception != null)
                        _logger.Error(result.Exception.ToString());
                }
                else if (DateTime.Now.Subtract(channel.DateModified) >
                    TimeSpan.FromHours(1.0))
                {
                    _logger.Info($"Channel with url {url} is older than an hour. Fetching rss xml");

                    string xmlString = await _httpRssClient.GetRSSXmlString(url);

                    _logger.Info($"Done fetching rss xml for url {url}. Parsing xml");

                    Channel fetchedChannel = _rssParser.ParseRSS(xmlString);

                    _logger.Info($"Done parsing xml. Updating existing channel for url {url}");

                    channel = await UpdateChannel(channel, fetchedChannel);
                    
                    _logger.Info($"Done updating existing channel with url {url}");

                }
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                throw e;
            }

            return channel;
        }

        private async Task<Channel> UpdateChannel(Channel oldChannel, Channel updatedChannel)
        {
            List<Article> newArticles = new List<Article>();

            oldChannel.Title = updatedChannel.Title;
            oldChannel.Description = updatedChannel.Description;
            oldChannel.Link = updatedChannel.Link;
            oldChannel.ChannelImage = updatedChannel.ChannelImage;

            await _channelRepository.Update(oldChannel);

            if (updatedChannel.Articles != null)
            {
                foreach (var article in updatedChannel.Articles)
                {
                    var existingArticle =
                        await _articleRepository.GetByUrl(article.Link);

                    if (existingArticle == null)
                    {
                        article.ChannelId = oldChannel.Id;
                        article.Channel = oldChannel;

                        await _articleRepository.Add(article);
                        newArticles.Add(article);

                        _logger.Info($"New Article with url {article.Link} Created");
                    }
                    else
                    {
                        existingArticle.Title = article.Title;
                        existingArticle.Description = article.Description;
                        existingArticle.PubDate = article.PubDate;

                        await _articleRepository.Update(existingArticle);
                        newArticles.Add(existingArticle);

                        _logger.Info($"Article with url {article.Link} Updated");
                    }
                }
            }

            updatedChannel.Articles = newArticles;

            return updatedChannel;
        }
    }
}
