
# CUR Inventory Edit

A CLI tool for editing Nancy Drew: The Curse of Blackmoor Manor save files to add or remove items from the player's inventory.

This tool was created to mitigate a bug that causes the columns in the great hall not to return the key items after the first use. Rather than attempting to reverse engineer the game code and patch the bug directly, this tool lets you add the items back to your inventory if the bug occurs.

## Warning
Back up your save files before running this tool! This tool has worked consistently on the version of the game I have, but there's always the risk of file corruption.

## Usage
Run the tool and provide the full path to your CUR save file with the .SAV file extension. Where your save games are located may change depending on where you installed the game. In my case, it was under "C:\Nancy Drew\The Curse of Blackmoor Manor\Saves" but yours may be in Program Files or in your Documents directory.

Run the 'help' command for a full list of commands. Remember to run the 'save' command to write the changes to the save file.

## Technical Details
As best as I have been able to disceren through testing:

CUR save files store the player's inventory starting at file address 0x110. The inventory consists of an array of 16 bit integers corresponding to in-game item ids.

### Save File Inventory Description

| File Offset | Size | Description |
| ----------- | ---- | ----------- |
| 0x110 | 4 bytes | Inventory Item Count |
| 0x114 | 2 bytes | Unknown (always seems to be the sequence 0x02 0x00) |
| 0x116 | n bytes | Array of 16 bit item IDs. Count is determined by 32 bit value at 0x110 |

### Discovered Item IDs

| Item ID | Description |
| ------- | ----------- |
| 1 | LouLou Cake |
| 2 | Coat of Arms (Gargoyle Hint) |
| 3 | Cricket Ball |
| 4 | Full Glow Stick |
| 5 | Empty Glow Stick |
| 6 | Lightning Bolt Column Key |
| 7 | East Hall Key |
| 8 | Helmet Column Key |
| 9 | Wand Column Key |
| 10 | Key Mould |
| 11 | Moon Column Key |
| 12 | Telescope Lens |
| 13 | Glowstone |
| 14 | Uncle Roger Note |
| 15 | Final Key |
| 16 | Clock Column Key |
| 17 | Jane's Door Puzzle Key |
| 19 | Arrowhead Column Key |
| 20 | Crank |
| 21 | Goggles |
| 22 | Easter Egg |
| 23 | Strange Key |
| 25 | Jonny Rutter |
| 26 | Food |
| 27 | Sealed Letter |
| 28 | Moving Rooms Map |
