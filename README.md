# MP3GainMT

This is a multi-threaded front end for MP3Gain aka one of the greatest programs ever written.

## History
I've been working on this for years. You need Visual Studio 2022 to compile it.

## Warning
It still has a lot of bugs regarding doing any value besides 89.0 target dB. Still trying to figure that out. Undoing an analysis is also wonky. If this program screws things up, load it up in the original MP3Gain and fix it.

## Operation
You have to locate your own copy of mp3gain.exe.
Standard Wortkflow: Browse for your directory. Click Add Files. Click Read Tags. Click Analysis. Click Apply Gain.

## Performance
The whole point of this is to allow you to run MP3gain on multiple folders in parallel.  I have a 16 core CPU and the amount of time you can save is insane if you have a lot of folders to process.

I spent an enormous amount of time optimizing this software for extremely large MP3 collections.  It loads tens of thousands of files with ease.

There are a lot of filtering and searching options so you can easily see what's going on with your folders without having to scroll through the whole list.
