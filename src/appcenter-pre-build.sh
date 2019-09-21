#!/usr/bin/env bash
#
# Set config values in the Config.json file
#

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

if [ -z "$CONTENTFUL_SPACEKEY" ]
then
    echo "You need define the CONTENTFUL_SPACEKEY variable in App Center"
    exit
fi

if [ -z "$CONTENTFUL_DELIVERYAPIKEY" ]
then
    echo "You need define the CONTENTFUL_DELIVERYAPIKEY variable in App Center"
    exit
fi

cd "${APPCENTER_SOURCE_DIRECTORY}/CWITC.Clients.Portable"
echo "{\"FacebookAppId\": \"${FACEBOOK_APPID}\",\"GithubClientId\": \"${GITHUB_CLIENTID}\",\"GithubClientSecret\": \"${GITHUB_CLIENTSECRET}\",\"TwitterClientId\": \"${TWITTER_CLIENTID}\",\"TwitterClientSecret\": \"${TWITTER_CLIENTSECRET}\",\"ContentfulSpaceKey\": \"${CONTENTFUL_SPACEKEY}\",\"ContentfulDeliveryApiKey\": \"${CONTENTFUL_DELIVERYAPIKEY}\"}" > Config.json