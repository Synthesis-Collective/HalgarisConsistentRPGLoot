# HalgarisConsistentUniqueRPGLoot

## What does this Patcher do ?

It adds a lot of enchanted weapons with varying numbers of rarities and effectiveness.
It is fully customizable. 

## Versioning

Use the `Tag` Versioning in Synthesis to not accidentally break your saves (rolling back *should* work for fixing it).

`X.Y.Z`:
- X - Big rework or bigger changes (new settings etc) **NEEDS NEW SAVE**
- Y - Internal changes that 100% will alter the consistency of the outputs between versions **NEEDS NEW SAVE**.
- Z - Typos, Minor Bug Fixes or Changes to default settings for NEW users.

## Settings

You can use these to customize how strong the presence of enchanted items is in your game.

**Settings:**

- **General Settings:**
  - RandomSeed:
    - Default: `42` 
    - Basically a Key to make the randomness repeatable.
  - Only process constructible equipment
    - Default: `On`
    - Only items that are referenced in crafting and tampering recipes get processed and enchanted.
  - Untouchable Equipment Keywords
    - Keywords that are on unique and/or incompatible items
    - Defaults:
      - `Skyrim.Keyword.MagicDisallowEnchanting`
        - (Usually used on unique legendary enchantments on unique items that would be worthless if their enchantment could just be found on bandits, and if the Artefact would not have it anymore.)
      - `Skyrim.Keyword.DaedricArtifact`
        - Same as MagicDisallowEnchanting but even more obvious.
      - `Skyrim.Keyword.WeapTypeStaff`
        - Staff magic effects would turn your swords into weird looking magic staffs and magic staffs would be weird with sword and bow enchantments.
- **Enchantment Settings:**
  - Enchantment List Mode:
    - Decides if the following list gets used as a blacklist or whitelist for allowed magic ObjectEffects (Enchantments)
    - `Blacklist` (Default)
    - `Whitelist`
  - Enchantment List
    - Default:
      - `Skyrim.ObjectEffect.BoundBattleaxeEnchantment`
      - `Skyrim.ObjectEffect.BoundBowEnchantment`
      - `Skyrim.ObjectEffect.BoundSwordEnchantment`
    - The defaults here filter the visual effect enchantments as they have no gameplay impact.
  - Plugin List Mode:
    - Decides if the following list gets used as a blacklist or whitelist for ESPs/ESMs of which the magic effects get distributed.
    - `Blacklist` (Default)
    - `Whitelist`
  - Plugin List
    - Default: `Empty` Because you Ideally manage to get universal filters done in the Keyword and Enchantment List Settings.
- **Rarity and Variation Settings:**
  - Generation Mode:
    - `GenerateRarities` (Default)
      - Generates and distributes the various enchantments as by the settings in the next section.
    - `JustDistributeEnchantments`
      - As the name implies only distributes enchantments, so that each weapon has each enchantment once.
      - With this selected the following settings can be ignored.
      - Not yet implemented.
  - Armor/WeaponSettings (Separate since some people have more armors or weapon in their setups)
    - VarietyCountPerItem:
      - The Number of Variations to be created for every item.
    - Rarities:
      - Label: Added to each generated item's name
      - Num Enchantments: The number of enchantments used to define a rarity...i.e. 1 enchantment is fairly balanced...4 probably not so much
      - RarityWeight: This defines the chance for each Rarity to be chosen when generating item Variants.
        - Calculation Process:
          1. Generate sum of all Rarity Weights
          2. Generate a random number between 0 and the sum of Rarity Weights
          3. Check starting with the largest Rarity Weight is in the range between the Sum (or the previous rarity weight, in the following checks) and the current rarity weight.
        - To get the percentage chance for a rarity to be generated (not found those are different!) you divide the Rarity Weight through the Sum of all Rarity Weights.
