using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;
using RSSBackgroundWorkerBusiness.Models;

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
                    Link = feed.BaseUri?.AbsoluteUri,
                    Description = feed.Description?.Text,
                    ChannelImage = new Channel.Image
                    {
                        URL = feed.ImageUrl?.AbsoluteUri
                    },
                    Articles = new List<Article>()
                };

                foreach (var item in feed.Items)
                {
                    channel.Articles.Add(new Article
                    {
                        Title = item.Title?.Text,
                        Link = item.BaseUri?.AbsoluteUri,
                        PubDate = item.PublishDate.UtcDateTime,
                    });
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
