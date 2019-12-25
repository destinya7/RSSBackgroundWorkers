using RSSBackgroundWorkerBusiness.Repositories;
using RSSCollectorService.Services;

namespace RSSCollectorService.Core
{
    public class CollectorCore : ICollectorCore
    {
        private IChannelRepository _channelRepository;
        private IWorkerQueuePublisher _workerQueuePublisher;

        public CollectorCore(
            IChannelRepository channelRepository,
            IWorkerQueuePublisher workerQueuePublisher
        )
        {
            _channelRepository = channelRepository;
            _workerQueuePublisher = workerQueuePublisher;
        }

        public async void CollectUrls()
        {
            var channels =
                await _channelRepository.GetChannelsLastUpdatedWithin(60);

            foreach(var channel in channels)
            {
                _workerQueuePublisher.PublishUrl(channel.RSS_URL);
            }
        }
    }
}
