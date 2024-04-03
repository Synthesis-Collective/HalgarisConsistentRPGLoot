# Changelog

## Version 3.1.0 - 03.04.2024
* Removed only process constructable items cause it isn't creating the limit that's intended for.
* Fixes for SkyrimVR and the Plugin Header Version 1.71

## Version 3.0.0 - 01.04.2024
* Complete rewrite of the item distribution method
  * Added filters with sane default values to filter out "dummy" enchantments that are just used for VFX
  * Allows for more flexibility and control than any previous version
    * please check the [README.md](README.md) for more details
  * Should be less prone to breaking even on bigger setups
    * I still advise caution since the file size of the resulting ESP could turn out bigger than 
      the `Skyrim.esm` (The default settings of this patcher with a fresh steam install results 
      in an ESP that is over double the size than the largest official DLC) and I can't guarantee how
      or if the Engine and tools like xEdit can handle that when it gets bigger!!
* Item and Enchantment Names now list all enchantments
  * added a config option to configure the separators for this
* lowered the total output size of the patch
*increased patching performance
