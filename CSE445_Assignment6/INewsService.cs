using System.ServiceModel;

namespace CSE445_Assignment6.NewsService
{
    [ServiceContract]
    public interface INewsService
    {
        // Returns news article links for a user-selected set of topics
        [OperationContract]
        string[] NewsFocus(string[] topics);
    }
}
