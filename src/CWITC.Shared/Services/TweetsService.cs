using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using CWITC.Clients.Portable;
using CWITC.Clients.Portable.Services;
using Censored;
using Xamarin.Forms;
using CWITC.Shared.Services;

[assembly: Dependency(typeof(TweetsService))]
namespace CWITC.Shared.Services
{
    public class TweetsService : ITweetsService
    {
        readonly Censor censor;
        const string words = "";
        public TweetsService()
        {
            censor = new Censor(words.Split(new[] { ',' }));
        }

        public async Task<IEnumerable<Tweet>> GetTweets()
        {
			var tweets = new List<Tweet>();

			var auth = new ApplicationOnlyAuthorizer()
			{
				CredentialStore = new InMemoryCredentialStore
				{
                    ConsumerKey = Clients.Portable.Settings.Current.TwitterApiKey,
					ConsumerSecret = Clients.Portable.Settings.Current.TwitterApiSecret
				},
			};
			await auth.AuthorizeAsync();

			var twitterContext = new TwitterContext(auth);

			var queryResponse = await
                //twitterContext.Search.W
				(from tweet in twitterContext.Search
                 where tweet.Type == SearchType.Search &&
					 (tweet.Query == "%23CWITC") &&
					 tweet.Count == 100
				 select tweet).SingleOrDefaultAsync();

            if (queryResponse == null || queryResponse.Statuses == null)
            {
                return tweets;
            }

			tweets =
				(from tweet in queryResponse.Statuses
				 where tweet.RetweetedStatus.StatusID == 0 && !tweet.PossiblySensitive && !censor.HasCensoredWord(tweet.Text)
				 select new Tweet
				 {
					 TweetedImage = tweet.Entities?.MediaEntities.Count > 0 ? tweet.Entities?.MediaEntities?[0].MediaUrlHttps ?? string.Empty : string.Empty,
					 ScreenName = tweet.User?.ScreenNameResponse ?? string.Empty,
					 Text = tweet.Text,
					 Name = tweet.User.Name,
					 CreatedDate = tweet.CreatedAt,
					 Url = string.Format("https://twitter.com/{0}/status/{1}", tweet.User.ScreenNameResponse, tweet.StatusID),
					 Image = (tweet.RetweetedStatus != null && tweet.RetweetedStatus.User != null ?
							 tweet.RetweetedStatus.User.ProfileImageUrl.Replace("http://", "https://") : tweet.User.ProfileImageUrl.Replace("http://", "https://"))
				 }).Take(15).ToList();

            return tweets;
        }
    }
}
