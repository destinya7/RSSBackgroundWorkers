using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;
using RSSBackgroundWorkerBusiness.Models;
using RSSFetcherService.Services;

namespace RSSFetcherService.Utils
{
    public class RSSParser : IRSSParser
    {
        public Channel ParseRSS(string xmlString)
        {
            Channel channel;

            try
            {
                var stringReader = new StringReader(xmlString);
                var xmlReader = XmlReader.Create(stringReader);
                var feed = SyndicationFeed.Load(xmlReader);

                channel = new Channel
                {
                    Title = feed.Title?.Text,
                    Link = feed.Links.Count > 0
                        ? feed.Links[0].Uri.AbsoluteUri
                        : string.Empty,
                    Description = feed.Description?.Text,
                    ChannelImage = new Channel.Image
                    {
                        URL = feed.ImageUrl?.AbsoluteUri
                    },
                    Articles = new List<Article>()
                };

                foreach (var item in feed.Items)
                {
                    var article = new Article
                    {
                        Title = item.Title?.Text,
                        Link = item.Links.Count > 0
                            ? item.Links[0].Uri.AbsoluteUri
                            : string.Empty,
                        PubDate = item.PublishDate.UtcDateTime
                    };
                    channel.Articles.Add(article);
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
