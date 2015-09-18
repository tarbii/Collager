using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using LinqToTwitter;

namespace Collager.Models
{
    public class Twitter
    {
        public static async Task<ApplicationOnlyAuthorizer> Authorize()
        {
            throw new NotImplementedException("Enter API key & secret below");
            var auth = new ApplicationOnlyAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore()
                {
                    ConsumerKey = "",
                    ConsumerSecret = ""
                }
            };
            await auth.AuthorizeAsync();
            return auth;
        }


        public static async Task<List<User>> ShowFriends(IAuthorizer auth, string login)
        {
            var users = new List<User>();
            var twitterCtx = new TwitterContext(auth);

            Friendship friendship;
            long cursor = -1;
            do
            {
                friendship = await twitterCtx.Friendship.Where(x =>
                   x.Type == FriendshipType.FriendsList
                   && x.ScreenName == login
                   && x.Cursor == cursor
                   && x.Count == 200
                   && x.IncludeUserEntities == false)
                    .SingleOrDefaultAsync();

                if (friendship != null
                    && friendship.Users != null
                    && friendship.CursorMovement != null)
                {
                    cursor = friendship.CursorMovement.Next;
                    users.AddRange(friendship.Users);
                }
            } while (cursor != 0);
            return users;
        }

    }
}