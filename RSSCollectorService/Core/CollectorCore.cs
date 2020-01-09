using RSSBackgroundWorkerBusiness.Repositories;
using RSSCollectorService.Services;
using System;

namespace RSSCollectorService.Core
{
    public class CollectorCore : ICollectorCore
    {
        private IChannelRepository _channelRepository;
        private IWorkerQueuePublisher _workerQueuePublisher;
        private ILoggerService _logger;

        public CollectorCore(
            IChannelRepository channelRepository,
            IWorkerQueuePublisher workerQueuePublisher,
            ILoggerService logger
        )
        {
            _channelRepository = channelRepository;
            _workerQueuePublisher = workerQueuePublisher;
            _logger = logger;
        }

        public void CollectUrls()
        {
            try
            {
                _logger.Info("Fetching Channels in repo");

                var channels = _channelRepository.GetChannelsLastUpdatedWithin(60);

                _logger.Debug("Done fetching Channels in repo");

                foreach (var channel in channels)
                {
                    _logger.Info($"Publishing {channel.RSS_URL}");

                    _workerQueuePublisher.PublishMessage(channel.RSS_URL);

                    _logger.Info($"Published {channel.RSS_URL}");
                }
            } catch (Exception e)
            {
                _logger.Info($"Error Collecting Urls {e.ToString()}");
            }
        }
    }
}
