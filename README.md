# HalgarisConsistentUniqueRPGLoot

## What does this Patcher do ?

It adds a lot of enchanted weapons with varying numbers of rarities and effectiveness.
It is fully customizable. 

## Versioning

Use the `Tag` Versioning in Synthesis to not accidentally break your saves (rolling back *should* work for fixing it).

`X.Y.Z`:
- X - Big rework or bigger changes (new settings etc) or a milestone (like me feeling it is finished enough to be called a 1.0 :sweat_smile: ) might **NEED NEW SAVE** (the changes on the releases tab will mention it).
- Y - Internal changes that 100% will alter the consistency of the outputs between versions **NEEDS NEW SAVE**.
- Z - Typos, Minor Bug Fixes or Changes to default settings for NEW users.

## Settings

You can use these to customize how strong the presence of enchanted items is in your game.

Settigns:

- VarietyCountPerItem: Number of varieties per base item, this will be constant and split among the varieties described.

- Rarities:
  - Label: Added to each generated item's name
  - Num Enchantments: The number of enchantments used to define a rarity...i.e. 1 enchantment is fairly balanced...4 probably not so much
  - LLEntries: This works as the odds that a specific rarity will be created.
    - Formula: (LLEntries / AllEntriesForAllItems)* VarietyCountPerItem
