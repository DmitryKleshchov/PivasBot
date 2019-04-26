using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SimpleFeedReader;


namespace PivasBot.RssConsumer
{
    public class RssConsumer
    {
        public static IEnumerable<FeedItem> GetFeeds(params string[] uris)
        {
            var reader = new FeedReader();
            return reader.RetrieveFeeds(uris);
        }
    }
}
