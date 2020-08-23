
# BNetClassicChat

## Important Notice:
Blizzard has abandoned this project with the release of Warcraft 3 Reforged. The API endpoint no longer exists. This repo will no longer be maintained.

C# API wrapper for Blizzard's classic chat API. Requires an API key from Blizzard to connect. Currently works for V3 Alpha of the API. Current spec document available here https://s3-us-west-1.amazonaws.com/static-assets.classic.blizzard.com/public/Chat+Bot+API+Alpha+v3.pdf.

Original API referenced by this post: https://us.battle.net/forums/en/bnet/topic/20754336617?page=1

BNetClassicChat_ClientAPI folder conntains the API wrapper itself, while BNetClassicChat_CmdLine contains a sample program demonstrating its use

## Installation
Available on Nuget as [BNetClassicChat_ClientAPI][nuget] or 
Clone the repo and import BNetClassicChat_APIONLY.sln into a C# project

[nuget]:https://www.nuget.org/packages/BNetClassicChat_ClientAPI/

## Build requirements
* Visual Studio 2017 or .NET Core 2.0+ CLI
* Newtonsoft.json and WebSocketSharp-netstandard (available from Nuget)



