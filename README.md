# Bullfrog Sound Extractor

This is a simple commandline tool to extract the constituent .wav files of SOUND.DAT files used in mid '90s Bullfrog Production games. These games all used slight variations on the same core file format design, and this tool can extract nearly all of them.

The intention is both for exploring the sound files inside, but also for use with [sndbanker](https://github.com/swfans/sndbanker) [tools](https://github.com/dkfans/sndbanker) to rebuild the SOUND.DAT file with the desired files present.

The tool outputs a log to a text file called filelist.txt that records the original sample file placements in the .DAT file. This list is essential for replacing individual samples, or creating new sndbanker input files.

## Supported Games:

Use the 2/3 letter code listed below as an argument for the correct game to make the tool use the correct format, or you will have errors.

* Dungeon Keeper - dk
* Gene Wars - gw
* Magic Carpet - mc1
* Magic Carpet 2: The NetherWorlds - mc2
* Syndicate Wars - sw 
* Theme Hospital - th

Note while Hi-Octane includes a files called SOUND.DAT, it's actually in a totally different format, so will not be supported here.

## Usage:

Put the tool in a directory with the SOUND.DAT file you want to extract

BullFrogSoundExtractor.exe [Input file] [Game code]

e.g. BullFrogSoundExtractor.exe SOUND.DAT sw

To extract the .wav files from Syndicate Wars

or  BullFrogSoundExtractor.exe SOUND-0.DAT th

To extract the .wav files from Theme Hospital

.DAT files that have high and low quality sample sets (Syndicate Wars, Gene Wars) will have two export directories created, "Hi" and "Lo" to store them. Games that didn't use this feature (Magic Carpets, Dungeon Keeper, Theme Hospital) will just extract to a directory called "extracted".

The most common version of the file is the most simple one, used by Dungeon Keeper and Theme Hospital. If in doubt with a game not listed above, try "th" as the game code. This is needed for e.g. the SYNCREDS.DAT file in Syndicate Wars, as opposed to the main SW SOUND.DAT file that needs the sw argument. If no game code is used as an argument, the tool will run in Theme Hospital mode.

The Creation demo is not supported yet, it uses a more complex variant of the Syndicate Wars file format. This isn't hard to support, but it's not in place yet.
There are also several other early demos that use weird and wonderful variants of the .DAT format. These likely won't work with this tool, but try runinng in TH mode for the best chance of success.
