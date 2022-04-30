# Risk of Rain 2 - Starting Items GUI

### TL:DR
This mod will allow players to use a GUI to select which items will be given to them at the start of a run. There are 4 modes: *Free*, *Earnt Persistent*, *Earnt Consumable* and *Random*. Works in both singleplayer and multiplayer (if the host and the client have the mod installed, enabled and are using the same mode. NOTE: Each client has to select their own items).

### Build

```zsh
git clone https://github.com/szymonj99/StartingItemsGUI
cd StartingItemsGUI
dotnet restore && dotnet build StartingItemsGUI.csproj --arch x64 --nologo
```

This will result in a `StartingItemsGUI.dll` file being output to either `Build/Debug` or `Build/Release`, depending on which build you have compiled.
One way to get this mod into your game is to then copy the `.dll` file into the outdated `StartingItemsGUI` folder and overwrite when prompted.

### License
This repository is licensed under the included [GNU GPLv3](LICENSE.txt) license.

### Distribution
This mod will be uploaded to Thunderstore soon(tm).

### INSPIRATION
This mod was inspired by features in other roguelike games such as *Undermine*, *Dungreed* and *Dead Cells*. The goal was to give a returning player a sense of progression, to give them a boost to make them more formidable the more they play.

## DESCRIPTION
The main menu will have a new button labelled *"Starting Items"*. Clicking it will open up the shop interface. Every item and piece of equipment that has been unlocked (completed the requisite achievement) and discovered (been picked up at least once) will be listed here, alongside how many times it has been purchased. To purchase an item or piece of equipment, left click it. The same item can be purchased multiple times. To sell an item or piece of equipment, right click it. Four buttons, labelled *"Earnt Consumable"*, *"Earnt Persistent"*, *"Free"* and *"Random"* are displayed on the top left of the interface. Clicking these will allow the player to change the mods mode. The differences of these four modes are detailed below. Another three buttons, labelled "Profile: 1", "Profile: 2" and "Profile: 3", are displayed in the top right of the interface. Clicking these will allow the player to change the current item loadout. Buying an item in one profile does not affect the others, this is so a player could configure each profile differently to easily switch between them between games. Each of the 3 profiles is unique to each mod mode. This mod can be enabled and disabled from this menu, should the player wish to play normally again. When a game begins the items from the currently selected mod mode and profile will be spawned into the players inventory.

### MODES

**EARNED CONSUMABLE MODE**
Items in the shop interface will have a price associated with them. Buying an item will cost *credits*, selling an item will refund the full amount of *credits*. When a game is played while this mode is active the *credits* that were spent on items will be used up and consumed. The current profile will also have its items removed, they will have to be purchased again with the players remaining *credits* if they want to be spawned again in subsequent games.

**EARNED PERSISTENT MODE**
Items in the shop interface will have a price associated with them. Buying an item will cost *credits*, selling an item will refund the full amount of *credits*. When a game is played while this mode is active the *credits* that were spent on items will NOT be used up and consumed. The purchases will persist between games, as a kind of permanent upgrade to the players character.

**FREE MODE**
Items in the shop interface will have no price, they can be purchased as many times as the player wishes. The purchases will persist between games and will not be reset.

**RANDOM MODE**
Items in the shop interface will have a price associated with them and an arbitrary number of *credits* are allocated in this mode. When a game is played, all the *credits* will be spent on a random assortment of items. Every game will have different items.

### CREDIT EARNING METHODS
Credits will be earnt regardless of the current mode of the mod. 

**BOSS MODE**
Players will earn credits for defeating endgame bosses. The amount of credits awarded per boss kill will vary based on the game's difficulty. Supported bosses in this mode are the: *Lunar Scavanger* and *Mithrix*.

**STAGE MODE**
Players will earn credits for every stage they have cleared. The amount of credits awarded per stage cleared will vary based on the game's difficulty and how it ended (win, loss, obliteration, limbo).

**ENDING MODE**
Players will earn credits for finishing games. The amount of credits cleared will vary based on the game's difficulty and how it ended (win, loss, obliteration, limbo).

## CONFIGURATION
After a player has opened the item shop interface for the first time a number of configuration files will be created.
These can be found at *"<RISK OF RAIN 2 INSTALL LOCATION>/Risk of Rain 2/BepInEx/config/"* and *"<RISK OF RAIN 2 INSTALL LOCATION>/Risk of Rain 2/BepInEx/config/Starting Items GUI/"*.
If you desire to alter the configuration of the mod it must be done by editing these files.
The intention is that this would only be necessary once, with the rest of the player's interactions being done through the shop interface inside the game itself.

## CREDITS

**Fork Maintainer**
szymonj99

**Icon Design**
Ben C.

**Education**
Ebkr, Harb, iDeathHD

**Support, Suggestions, Beta Testing and Bug Reports**
blazingdrummer
Argent, Beaker, Borst, breadguy, Cookiefox, dfauci, DoctorPepper, ffrig, Galxy, Goldair1, Gridalien, hugglesthemerciless, Hypro, Kinggrinyov, McFow1er, Nathanel, NetherCrowCSOLYOO, Nik, themostinternet, Zarko
