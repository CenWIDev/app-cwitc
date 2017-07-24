const functions = require('firebase-functions');
var Twitter = require('twitter-node-client').Twitter;

// // Create and Deploy Your First Cloud Functions
// // https://firebase.google.com/docs/functions/write-firebase-functions
//
// exports.helloWorld = functions.https.onRequest((request, response) => {
//  response.send("Hello from Firebase!");
// });

exports.tweets = functions.https.onRequest((request, response) => {
//Callback functions
	var error = function (err, response, body) {
    	console.log('ERROR [%s]', err);
	};
	var success = function (data) {

        response.send(data);

    	console.log('Data [%s]', data);
	};

	var Twitter = require('twitter-node-client').Twitter;

	//Get this data from your twitter apps dashboard
	{
    	"consumerKey": "SwgBQECG9xpBXsd9TejKbn23Y",
    	"consumerSecret": "GZN9NMK4hIniSR0WzHYgDzE8dzElf480l1c8GjPpXkfl2fLP3NXXX",
	}

	// make a directory in the root folder of your project called data
	// copy the node_modules/twitter-node-client/twitter_config file over into data/twitter_config`
	// Open `data/twitter_config` and supply your applications `consumerKey`, 'consumerSecret', 'accessToken', 'accessTokenSecret', 'callBackUrl' to the appropriate fields in your data/twitter_config file
    
    var twitter = new Twitter();

	
	//
	// Get 10 tweets containing the hashtag haiku
	//
	twitter.getSearch({'q':'#cwitc','count': 20}, error, success);
});
