#!/usr/bin/env janet
(use /src/skills)
(use /src/object-blueprints)

(use cbt)

(build-metadata
  :qud-dlls "/home/petrak/.local/share/Steam/steamapps/common/Caves of Qud/CoQ_Data/Managed/"
  :qud-mods-folder "/home/petrak/.config/unity3d/Freehold Games/CavesOfQud/Mods/")

(declare-mod
  "appraisal"
  "Appraisal"
  "petrak@"
  "0.1.0")

(generate-xml "ObjectBlueprints.xml" object-blueprints)
(generate-xml "Skills.xml" skills)

# (set-debug-output true)

