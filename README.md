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

Settigns:

- RandomSeed: Basically a Key to make the randomness repeatable.
- VarietyCountPerItem: The Number of Variations to be created for every item.
- Rarities:
  - Label: Added to each generated item's name
  - Num Enchantments: The number of enchantments used to define a rarity...i.e. 1 enchantment is fairly balanced...4 probably not so much
  - RarityWeight: This defines the chance for each Rarity to be chosen when generating item Variants.
    - Calculation Process:
      1. Generate sum of all Rarity Weights
      2. Generate a random number between 0 and the sum of Rarity Weights
      3. Check starting with the largest Rarity Weight is in the range between the Sum (or the previous rarity weight, in the following checks) and the current rarity weight.
    - To get the percentage chance for a rarity to be generated (not found those are different!) you divide the Rarity Weight through the Sum of all Rarity Weights.
