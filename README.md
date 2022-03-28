# CSharpTwitchTTSBot

## Downloadable Executable for Windows 10 in Releases:

## User Instructions for Windows:

1. Download Release 
2. Unzip to folder (You can do this with winrar or similar application).
3. Inside the folder you will find a file called "CSharpTwitchTTSBot.exe" and a folder called "data"
4. Inside the "data" folder you will find a file called "userSettings.json"
5. Modify the defaultJoinChannel and botAdminUserName values using a text editor like notepad (the current values for each are "xcomreborn").
6. Run the program by double clicking the CSharpTwitchTTSBot.exe icon.

-- A console window should appear in the background. 
-- To close the program, you can either press return with that window in focus OR type !closetts in the twitch chat channel you selected OR by closing the console window with the close icon.


## Bot Commands.

### Bot commands for Streamer/AdminUserName:

**!ignoreword** [word] - will mute a message containing a specific word eg: http or www.  
**!unignoreword** [word] - will remove the word from the ignore word list.  
**!closetts** - will close the program.  

### Bot commands restricted to twitch channel moderators:

**!voices** - lists all available voices.  
**!voice #** - sets the users voice to an available voice listed in !voices.  
**!uservoice** [userName] # - sets the userName's voice to an available voice listed in !voices.  

**!alias** [alias] [optional userName] - sets mod username alias.  
**!useralias** [userName] [alias] - sets another users alias.  

**!ignore [userName]** - ignores the following user name.  
**!unignore [userName]** - unignores the following user name.  
**!blacklist** - lists all the usernames in the blacklist (!ignorelist).  

**!substitute [word] [substitute]** - will substitute the words in the substitute for the word.  
**!removesubstitute [word]** - removes the word from the substitute dictionary.  


### ==Advanced Commands==

**!regex [pattern] [substitute]** - will substitute the words in the substitute for the regular expression match in the pattern. Care should be taken not to use a broad match search.  
**!removeregex [pattern]** - removes the pattern from the regex substitute dictionary.  


## Compilation Instructions.
 
 A simple bot that uses the twitchlib library and System.Speech.Synthesis windows SAPI5 to speak text written in twitch IRC Chat.  

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






