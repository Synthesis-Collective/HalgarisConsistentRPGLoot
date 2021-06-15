# HalgarisConsistentRPGLoot 

## Versioning

Use the `Tag` Versioning in Synthesis to not accidentally break your saves (rolling back *should* work).

X.Y.Z:
- X - Big rework or bigger changes (new settings etc) or a milestone (like me feeling it is finished enough to be called a 1.0 :sweat_smile: ) might **NEED NEW SAVE**.
- Y - Internal changes that 100% will alter the consistency of the outputs between versions **NEEDS NEW SAVE**.
- Z - Typos, Minor Bug Fixes or Changes to default settings for NEW users.

## Settings (slightly outdated)

You can use these to customize what it will do when you run Synthesis.

Settigns:

- VarietyCountPerItem: Number of varieties per base item, this will be constant and split among the varieties described.

- Rarities:
  - Label: Added to each generated item's name
  - Num Enchantments: The number of enchantments used to define a rarity...i.e. 1 enchantment is fairly balanced...4 probably not so much
  - LLEntries: This works as the odds that a specific rarity will be created.
    - When UseRNGRarities = true, this will be randomized, and work as a chance value.
    - When UseRNGRarities = false, this will define how many varieties of this rarity will be created
      - Formula: (LLEntries / AllEntriesForAllItems)* VarietyCountPerItem

- UseRNGRarities: Should the number of varieties per item be randomly selected, or the same for all items? (it is possible to get none of some rarities or all of others, even an even spread)

```
