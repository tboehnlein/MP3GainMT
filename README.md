# MP3GainMT

This is a multi-threaded front end for MP3Gain aka one of the greatest programs ever written.  The way it is multi-threaded is each folder of files gets a seprate thread to do gain analysis.  So if you have an 8 core CPU, you can process 8 folders at once. I'm assuming your folders are individual albums.

## History
I've been working on this for years. You need Visual Studio 2022 to compile it. You can use Community Edition if you don't own Visual Studio Pro. I'm putting this out here in an incomplete state because I know somebody somewhere would love to use it. Also, hoping it motivate me to track down bugs and finish it.

## Warning
It still has a lot of bugs regarding doing any value besides 89.0 target dB. Still trying to figure that out. Undoing an analysis is also wonky.

This is nowhere near feature complete compared to MP3Gain. Tons of stuff has yet to be implemented. If you are doing more than the basics, go back to the original program.

If your file names are all messed up after a crash, open the folder in MP3GainMT again and it should fix everything. I rename the files before sending them to MP3Gain as workaround for the lack of unicode support with MP3Gain. There is a file in the folder with all of the original names, so running the software again will rename the files back to their original names.

## Operation
You have to locate your own copy of mp3gain.exe. See the browse button in the upper left corner.

Standard Wortkflow: 
+ Browse for your directory and click add files or drag and drop a folder into the table.
  + That will find all the MP3 files in the folder and its subfolders.
+ Click Read Tags.
+ Click Analyze.
+ Click Apply Gain.
+ Do not skip pressing any of the buttons.

![image](https://github.com/user-attachments/assets/04b2109d-d8ad-4c55-83d7-bf6b968f27e2)

## Performance
The whole point of this is to allow you to run MP3gain on multiple folders in parallel.  Huge performance gains can be had here.

I spent an enormous amount of time optimizing this software for extremely large MP3 collections.  It loads tens of thousands of files with ease. No flickering. No slow down.

If you have a large list of files, there are a lot of filtering and searching options so you can easily see what's going on with your folders without having to scroll through the whole list.

Filter Features Include:
+ Clipping
+ Track Clipping
+ Album Clipping
+ dB > XX.X
+ text-based matching

## User Interface

I'm trying to add user interface features that I personally love to have in software. I wish Tag&Rename had a dark mode so I added one here. Be the change you want to see.

User Interface Customization:
+ Light Mode/Dark Mode
+ Table Font Size
+ Table Double Click Row Behavior
  + Open MP3 file
  + Open MP3 file's folder
+ Right-click context menu to do either one


## Libraries I Use
+ BetterFolderBrowser (https://github.com/Willy-Kimura/BetterFolderBrowser)
+ Equin ApplicationFramework BindingListView (https://sourceforge.net/projects/blw/)
+ Newtonsoft JSON (https://www.newtonsoft.com/json)
+ NLog (https://nlog-project.org/)
+ TagLibSharp (https://github.com/mono/taglib-sharp) 

