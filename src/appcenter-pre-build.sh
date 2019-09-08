#!/usr/bin/env bash
#
# For Xamarin, change some constants located in some class of the app.
# In this sample, suppose we have an AppConstant.cs class in shared folder with follow content:
#
# namespace Core
# {
#     public class AppConstant
#     {
#         public const string ApiUrl = "https://CMS_MyApp-Eur01.com/api";
#     }
# }
# 
# Suppose in our project exists two branches: master and develop. 
# We can release app for production API in master branch and app for test API in develop branch. 
# We just need configure this behaviour with environment variable in each branch :)
# 
# The same thing can be perform with any class of the app.
#
# AN IMPORTANT THING: FOR THIS SAMPLE YOU NEED DECLARE API_URL ENVIRONMENT VARIABLE IN APP CENTER BUILD CONFIGURATION.

if [ -z "$FACEBOOK_APPID" ]
then
    echo "You need define the FACEBOOK_APPID variable in App Center"
    exit
fi

if [ -z "$GITHUB_CLIENTID" ]
then
    echo "You need define the GITHUB_CLIENTID variable in App Center"
    exit
fi

if [ -z "$GITHUB_CLIENTSECRET" ]
then
    echo "You need define the GITHUB_CLIENTSECRET variable in App Center"
    exit
fi

if [ -z "$TWITTER_CLIENTID" ]
then
    echo "You need define the TWITTER_CLIENTID variable in App Center"
    exit
fi

if [ -z "$TWITTER_CLIENTSECRET" ]
then
    echo "You need define the TWITTER_CLIENTSECRET variable in App Center"
    exit
fi

APP_CONSTANT_FILE=$APPCENTER_SOURCE_DIRECTORY/CWITC.Clients.Portable/Constants.cs

if [ -e "$APP_CONSTANT_FILE" ]
then
    echo "Updating FacebookAppId to $FACEBOOK_APPID in AppConstant.cs"
    sed -i '' 's#FacebookAppId = "[-A-Za-z0-9:_./]*"#FacebookAppId = "'$FACEBOOK_APPID'"#' $APP_CONSTANT_FILE

    echo "Updating GithubClientId to $GITHUB_CLIENTID in AppConstant.cs"
    sed -i '' 's#GithubClientId = "[-A-Za-z0-9:_./]*"#GithubClientId = "'$GITHUB_CLIENTID'"#' $APP_CONSTANT_FILE

    echo "Updating ApGithubClientSecretiUrl to $GITHUB_CLIENTSECRET in AppConstant.cs"
    sed -i '' 's#GithubClientSecret = "[-A-Za-z0-9:_./]*"#GithubClientSecret = "'$GITHUB_CLIENTSECRET'"#' $APP_CONSTANT_FILE

    echo "Updating TwitterClientId to $TWITTER_CLIENTID in AppConstant.cs"
    sed -i '' 's#TwitterClientId = "[-A-Za-z0-9:_./]*"#TwitterClientId = "'$TWITTER_CLIENTID'"#' $APP_CONSTANT_FILE

    echo "Updating TwitterClientSecret to $TWITTER_CLIENTSECRET in AppConstant.cs"
    sed -i '' 's#TwitterClientSecret = "[-A-Za-z0-9:_./]*"#TwitterClientSecret = "'$TWITTER_CLIENTSECRET'"#' $APP_CONSTANT_FILE

    echo "File content:"
    cat $APP_CONSTANT_FILE
fi
