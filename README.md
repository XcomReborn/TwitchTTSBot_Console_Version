# CSharpTwitchTTSBot
 
 A simple bot that uses the twitchlib library and System.Speech.Synthesis windows SAPI5 to speak text written in twitch IRC Chat

 Dependancies:

 Requires the following libraries:

 TwitchLib 3.3.0 - https://github.com/TwitchLib/TwitchLib
 System.Speech - https://docs.microsoft.com/en-us/dotnet/api/system.speech.synthesis.speechsynthesizer?view=netframework-4.8

These can be installed in your IDE of choice, if you are using VSCode NUGet, I would recommend installing NuGet Gallery(v0.0.24) via the marketplace.

Library Install Instructions (VSCode):

Open market place (Ctrl+Shift+x)
search for and install NuGet.

In Command Pallet (Ctrl+Shift+P)
open NuGet Gallery
search for and install TwitchLib 3.3.0
search for and install System.Speech 6.0.0
search for and install NewtonSoft.Json 13.0.1

The bot commands are restricted to twitch channel moderators and are as follows:

!voices - lists all available voices
!voice # [optional userName]- sets the users voice to an available voice listed in !voices or sets the voice of an another user to that voice

!alias [alias] [optional userName] - sets the users alias or sets the alias of another user

!ignore [] - ignores the following user name
!unignore [] - unignores the following user name
!blacklist - lists all the usernames in the blacklist (ignorelist)


