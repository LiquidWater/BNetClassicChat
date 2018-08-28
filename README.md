# BNetClassicChat
C# API wrapper for Blizzard's classic chat API. Requires an API key from Blizzard to connect. Currently works for V3 Alpha of the API. Current spec document available here https://s3-us-west-1.amazonaws.com/static-assets.classic.blizzard.com/public/Chat+Bot+API+Alpha+v3.pdf.

Original API referenced by this post: https://us.battle.net/forums/en/bnet/topic/20754336617?page=1

## Build requirements
* Visual Studio 2017
* Newtonsoft.json (available from nuget)
* WebSocketsharp (available from nuget)

Clone the repo using git, update nuget packages, and build.

## BNetClassicChat_ClientAPI
The API wrapper itself

## BNetClassicChat_CmdLine
Sample program demonstrating the use of BNetClassicChat_ClientAPI
