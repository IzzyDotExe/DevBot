# DevBot
A discord bot made for developers around the world!


# THIS BOT IS WIP 
## Planned features
- Github and wakatime integration
- Code time leaderboards
- Fetching info on github repos
- StackExchange integration
- code formatter / deobfuscator
- Github repo management


## Installing .NET for linux

This bot targets `.net 5.0` so you'll need to install the SDK if you want to build it. [Learn about how to install it here for your distro.](https://docs.microsoft.com/en-us/dotnet/core/install/linux?WT.mc_id=dotnet-35129-website) You will also need to install the dependencies through nuget, an extra source is needed for the nigtly version of Dsharpplus. 

### .Net on Ubuntu 21.04
First install the repository
```bash
wget https://packages.microsoft.com/config/ubuntu/21.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
```
Then install the SDK through apt
```bash
sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-5.0
```
### .Net on Arch/Manjaro
The .net 5.0 SDK can be installed through the AUR. The easiest way is to use a tool like [yay.](https://github.com/Jguer/yay)

First install yay if you have not
```
 | ~/Documents/DevBot  sudo pacman -S yay
```

Then, use yay to install dotnet 5 SDK
```
 | ~/Documents/DevBot  yay dotnet 5
```
Please note that you may also need to install the targeting pack.

## Installing Dependencies (Linux)

Now you will need the dependencies, the following nuget packages are required. The first thing you need to do is add a new nuget repository so you can access the nightly builds of dshapplus. You will need to find your `NuGet.config` file, it is usually located in `~/.nuget/NuGet`

Once you have found the file it should look something like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
```
Add this new package source: `<add key="DSharpPlus SlimGet" value="https://nuget.emzi0767.com/api/v3/index.json" />`

Once that is finished, save the file and exit your editor. Now you can install the dependencies.

```
 | ~/Documents/DevBot  dotnet restore
```
