# WinTenBot.NET
Official repository WinTenBot, written in .NET

# Run Local Development
- Clone this repo and open using your favorite IDE or Text Editor (e.g Visual Studio 2019 (Community is free edition), Jetbrains Rider (by Jetbrains) or Visual Studio Code.
- Install MySQL/MariaDB and create database e.g. zizibot.
- Install Net 3.1 SDK for ASP Net Core.
- Update setting any .json settings (appsettings.json, etc).
- Press Start in your IDE to start debugging or using dotnet run in .csproj directory.
- Your bot has ran local as Developvent using Poll mode.

# Run as Production
- Server with domain name include HTTPS support (e.g https://yoursite.co.id)
- Install Net 3.1 ASP NET Runtime
- Launch bot with `dotnet WinTenBot.dll` and get localhost:port
- Install webserver for reverse proxy.
- Add config for reverse proxy to domain name, [here example](https://www.google.com/search?client=firefox-b-d&q=nginx+reverse+proxy+example)
- WebHook will automatically ensured

Currently running on [Zizi Bot](t.me/MissZiziBot) and under beta in [Zizi Beta Bot](t.me/MissZiziBetaBot)
