using RSSBackgroundWorkerBusiness.Models;

namespace RSSFetcherService.Utils
{
    public class ArticleMessageConverter : MessageConverter<Article>,
        IArticleMessageConverter
    {
    }
}
