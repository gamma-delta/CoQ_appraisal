#!/usr/bin/env janet
(use /src/skills)
(use /src/object-blueprints)
(use /src/subtypes)

(use cbt)

(build-metadata
  :qud-dlls "/home/petrak/.local/share/Steam/steamapps/common/Caves of Qud/CoQ_Data/Managed/"
  :qud-mods-folder "/home/petrak/.config/unity3d/Freehold Games/CavesOfQud/Mods/")

(declare-mod
  "appraisal"
  "Appraisal"
  "petrak@"
  "0.3.0"
  :description "Adds an Appraisal skill tree that lets you judge things about an object by looking at them."
  :thumbnail "thumbnail.png"
  :steam-id 3024356897)

(generate-xml "ObjectBlueprints.xml" object-blueprints)
(generate-xml "Skills.xml" skills)
(generate-xml "Subtypes.xml" subtypes)

# (set-debug-output true)

