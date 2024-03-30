# Synthesis RPG Loot (Enchantment Distributor)

## What does this Patcher do ?

This patcher will distribute the enchantments found in your setup to your leveled lists and previously unenchanted items.

For this the patcher has a Distribution Only Mode and a more fun and enhanced RPGLoot rarity level Distribution.

## Versioning

Use the `Tag` Versioning in Synthesis to not accidentally break your saves (rolling back *should* work for fixing it).

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
  - **LeveledList Flags List:**
    - Information about those flags can be found on [en.uesp.net](https://en.uesp.net/wiki/Skyrim:Leveled_Lists) (they are lightly different named) and [ck.uesp.net](https://ck.uesp.net/wiki/LeveledItem)
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
        - (Usually used on unique legendary enchantments on unique items that would be worthless if their enchantment could just be found on bandits, and if the Artefact would not have it anymore.)
      - `Skyrim.Keyword.DaedricArtifact`
        - Same as MagicDisallowEnchanting but even more obvious.
      - `Skyrim.Keyword.WeapTypeStaff`
        - Staff magic effects would turn your swords into weird looking magic staffs and magic staffs would be weird with sword and bow enchantments.
- **Enchantment Settings:**
  - **Enchantment List Mode:**
    - Decides if the following list gets used as a blacklist or whitelist for allowed magic ObjectEffects (Enchantments)
    - `Blacklist` **(Default)**
    - `Whitelist`
  - **Enchantment List:**
    - **Default:**
      - `Skyrim.ObjectEffect.BoundBattleaxeEnchantment`
      - `Skyrim.ObjectEffect.BoundBowEnchantment`
      - `Skyrim.ObjectEffect.BoundSwordEnchantment`
    - The defaults here filter the visual effect enchantments as they have no gameplay impact.
  - **Plugin List Mode:**
    - Decides if the following list gets used as a blacklist or whitelist for ESPs/ESMs of which the magic effects get distributed.
    - `Blacklist` **(Default)**
    - `Whitelist`
  - **Plugin List:**
    - **Default:** `Empty` Because you Ideally manage to get universal filters done in the Keyword and Enchantment List Settings.
- **Rarity And Variation Distribution Settings:**
  - **RandomSeed:**
    - **Default:** `42`
    - Basically a Key to make the randomness repeatable, as long ass your leveled lists, enchantments and weapons in the list don't change.
  - **Leveled List Base**
    - Changes where the RPGLoot leveled lists are inserted.
    - `AllValidEnchantedItem` The chance of encountering enchanted gear is similar to vanilla, but the gear you find will use the new rarities.
    - `AllValidUnenchantedItems` **(Default)** This essentially reworks the whole system, allowing you a greater control if you want more chances for enchanted loot.
  - **Armor/WeaponSettings:** _(Separate since some people have more armors or weapon in their setups)_
    - **Variety Count Per Rarity:**
      - **Default:** `16`
      - The Number of Variations to be created for every item.
    - **Base Item Chance Weight**
      - **Default:** `40`
      - `count` property of the weapon entry next to the rarity leveled lists.
    - **Rarities:**
      - Label: Added prefix to each generated item's name.
      - Num Enchantments: The number of enchantments used to define the rarity.
      - RarityWeight: Amount of times the rarity is put into leveled lists.
      - AllowDisenchanting: If not enabled new Items get the Keyword: `Skyrim.Keyword.MagicDisallowEnchanting`
      - **Default Rarities:**

        | Rarity Label | Number of Enchantments | Rarity Weight | Allow Disenchanting |
        | ------------ | ---------------------- | ------------- | ------------------- |
        | -            | 1                      | 40            | true                |
        | Rare         | 2                      | 13            | false               |
        | Epic         | 3                      | 5             | false               |
        | Legendary    | 4                      | 2             | false               |
