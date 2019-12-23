using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;
using RSSBackgroundWorkerBusiness.Models;

namespace RSSFetcherService.Utils
{
    public class RSSParser : IRSSParser
    {
        public Channel ParseRSS(string url)
        {
            XmlReader xmlReader = null;
            SyndicationFeed feed = null;
            Channel channel = null;

            try
            {
                xmlReader = XmlReader.Create(url);
                feed = SyndicationFeed.Load(xmlReader);
                channel = new Channel
                {
                    Title = feed.Title.Text,
                    Link = feed.BaseUri.AbsoluteUri,
                    Description = feed.Description.Text,
                    ChannelImage = new Channel.Image
                    {
                        URL = feed.ImageUrl.AbsoluteUri
                    },
                    Articles = new List<Article>()
                };

                foreach(var item in feed.Items)
                {
                    channel.Articles.Add(new Article
                    {
                        Title = item.Title.Text,
                        Link = item.BaseUri.AbsoluteUri,
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
