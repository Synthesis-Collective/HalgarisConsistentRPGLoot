# Synthesis RPG Loot (Enchantment Distributor)

## What does this Patcher do ?

This patcher allows you to generate more enchantment variations defined as rarities,
that mix existing enchantments into new ones.  
And it then distributes those enchantments to the items in your leveled lists
based on various configurable settings.

## Versioning

Use the `Tag` Versioning in Synthesis to not accidentally break your saves
(rolling back *should* work for fixing it).

`X.Y.Z`:

- X - Big rework or bigger changes (new settings etc) **NEEDS NEW SAVE**
- Y - Internal changes that 100% will alter the consistency of the outputs between versions **NEEDS NEW SAVE**.
- Z - Typos, Minor Bug Fixes or Changes to default settings for NEW users.

## Settings

You can use these to customize and adjust the patcher to your setup.
Any mention of weight is equivalent to the `count` property of leveled lists.

### Settings:

- **General Settings:**
  - **Only process constructible equipment:**
    - **Default:** `On`
    - Only items that are referenced in crafting and tampering recipes get processed and enchanted.
  - **Enchantment Separator:**
    - **Default:** `, `
    - Separator used for listing all enchantments on labels
  - **Last Enchantment Separator:**
    - **Default:** ` and `
    - Separator used for the last two enchantments listed on labels
  - **LeveledList Flags List:**
    - Information about those flags can be found on [en.uesp.net](https://en.uesp.net/wiki/Skyrim:Leveled_Lists)
      (they are named slightly different) and [ck.uesp.net](https://ck.uesp.net/wiki/LeveledItem)
    - `CalculateFromAllLevelsLessThanOrEqualPlayer`
      - **Default:** `On`
      - Default because it is present in Vanilla enchanted Leveled Lists
    - `CalculateForEachItemInCount`
      - **Default:** `On`
      - Default because it it present in Vanilla enchanted Leveled Lists
    - `UseAll`
      - **Default:** `Off`
    - `SpecialLoot`
      - **Default:** `Off`
  - **Untouchable Equipment Keywords:**
    - Keywords that are on unique and/or incompatible items
    - Defaults:
      - `Skyrim.Keyword.MagicDisallowEnchanting`
        - (Usually used on unique legendary enchantments on unique items
          that would be worthless if their enchantment could just be found on bandits,
          and if the Artefact would not have it anymore.)
      - `Skyrim.Keyword.DaedricArtifact`
        - Same as MagicDisallowEnchanting but even more obvious.
      - `Skyrim.Keyword.WeapTypeStaff`
        - Staff magic effects would turn your swords into weird looking magic staffs
          and magic staffs would be weird with sword and bow enchantments.
- **Enchantment Settings:**
  - **Enchantment List Mode:**
    - Decides if the following list gets used as a blacklist or whitelist
      for allowed magic ObjectEffects (Enchantments)
    - `Blacklist` **(Default)**
    - `Whitelist`
  - **Enchantment List:**
    - **Default:**
      - `Skyrim.ObjectEffect.BoundBattleaxeEnchantment`
      - `Skyrim.ObjectEffect.BoundBowEnchantment`
      - `Skyrim.ObjectEffect.BoundSwordEnchantment`
    - The defaults here filter the visual effect enchantments as they have no gameplay impact.
  - **Plugin List Mode:**
    - Decides if the following list gets used as a blacklist or whitelist
      for ESPs/ESMs of which the magic effects get distributed.
    - `Blacklist` **(Default)**
    - `Whitelist`
  - **Plugin List:**
    - **Default:** `Empty` Because you Ideally manage to get universal filters done in the Keyword
      and Enchantment List Settings.
- **Rarity And Variation Distribution Settings:**
  - **RandomSeed:**
    - **Default:** `42`
    - Basically a Key to make the randomness repeatable, as long as your leveled lists,
      enchantments and weapons in the list don't change.
  - **Leveled List Base**
    - Changes where the RPGLoot leveled lists are inserted.
    - `AllValidEnchantedItem` The chance of encountering enchanted gear is similar to vanilla,
      but the gear you find will use the new rarities.
      - This means everywhere a vanilla leveled list had an enchanted variant of an item this patcher inserts
        different rarities for it.
      - When using this option keep in mind that the enchanted item becomes the `Base Item`,
        meaning it is recommended to remove the un-named Rarity option.
      - Since the game creates a lot unique lists for enchanted items it is recommended 
        to also keep the Weights(especially the Base Item Weight very low), since the game has 
        a lot more enchanted item entries in leveled lists due to the various enchantments that
        will all get expanded with the new loot.
    - `AllValidUnenchantedItems` **(Default)** This essentially reworks the whole system, 
      allowing you a greater control if you want more chances for enchanted loot.
      - This can potentially even cause your crafted gear to craft as enchanted versions.
      - This will also keep vamilla enchanted items untouched
        (if I remember my implementation of this code from as of writing nearly 2 years ago correctly)
  - **Armor/WeaponSettings:** _(Separate since some people have more armors or weapon in their setups)_
    - **Variety Count Per Rarity:**
      - **Default:** `16`
      - The Number of Variations to be created for every item, per rarity level.
        - This means each item gets `Variety_Count_Per_Rarity*Number_of_Rarities` items generated in total.
        - **You are multiplying the total amount of the respective item type in your setup by this value!
          So be careful as too high values can break, xEdit and then your game!**
    - **Base Item Chance Weight**
      - **Default:** `20`
      - `count` property of the weapon entry next to the rarity leveled lists.
      - This it the item type selected as the `Leveled List Base`.
        - So in the case of using `AllValidEnchantedItem` it would be the vanilla enchanted item.
        - When using `AllValidUnenchantedItems` it will be the unenchanted base item.
    - **Rarities:**
      - Label: Added prefix to each generated item's name.
      - Num Enchantments: The number of enchantments used to define the rarity.
      - RarityWeight: Amount of times the rarity is put into leveled lists.
      - AllowDisenchanting: If not enabled new Items get the Keyword: `Skyrim.Keyword.MagicDisallowEnchanting`
      - **Default Rarities:**  
        *The Base Item Rarity is included for visualizing the percentages.* 

        | Rarity Label | Number of Enchantments            | Rarity Weight | Allow Disenchanting | *Percentage* |
        |--------------|-----------------------------------|---------------|---------------------|--------------|
        | *Base*       | *`AllValidUnenchantedItems` => 0* | 20            | *Item Default*      | ~40,8%       |
        | -            | 1                                 | 17            | true                | ~34,7%       |
        | Rare         | 2                                 | 8             | false               | ~16,3%       |
        | Epic         | 3                                 | 3             | false               | ~6,1%        |
        | Legendary    | 4                                 | 1             | false               | ~2,1%        |
          
      - The formula for translating this into percentages is:
        `Weight_of_Rarity/Sum_of_Base_And_Rarity_Weights` (The base item weight is considered a rarity in this context)
        - *Disclaimer: The percentages won't be 100% reflected like this in gameplay since they only account for the
          chances introduced by this patcher and need to be considered on top of the vanilla/base chances of your setup
          to get either normal or enchanted gear depending on the `Leveled List Base` that was chosen in the previous
          settings. Additionally the various filters for what are valid items will effect those percentages as well.* 
