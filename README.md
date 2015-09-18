# Collager
######Makes photo collage of one's twitter friends' (followings) profile pictures.
Collager is an ASP.NET MVC Application made to participate in UA Web Challenge contest.

######How it works

You enter Twitter login ang get collage made from avatars of users that are followed by provided login. 
The more tweets the bigger picture on collage.

App uses Twitter [Public API](https://dev.twitter.com/rest/public) and 
[Application-only Authentication](https://dev.twitter.com/oauth/application-only) 
so you will need to create [Twitter App](https://apps.twitter.com/) and enter your `ConsumerKey` and `ConsumerSecret` 
in `Models.Twitter.Authorize()` method to make this code work.

Also you can see it in work [here](http://collager.azurewebsites.net/).

![collage example](http://oi62.tinypic.com/xm1ys0.jpg "collage example")
