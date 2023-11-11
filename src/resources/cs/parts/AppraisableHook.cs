using System.Collections.Generic;
using System;
using XRL.Language;
using XRL.World.Anatomy;

namespace XRL.World.Parts {
  // Instead of mixing into Physics or something
  // just slappp this on everything lmao
  public class PKAPP_AppraisableHook : IPart {
    public static string NUMBER_FORMAT = "N0";

    public override bool WantEvent(int id, int cascade) =>
      base.WantEvent(id, cascade)
      || id == GetShortDescriptionEvent.ID;

    public override bool HandleEvent(GetShortDescriptionEvent e) {
      // i am assuming only the player can look at things.
      var player = The.Player;
      if (player != null && ParentObject != null && ParentObject != player) {
        // Quick-check for skills here ... even though we check again later
        // idk if this has any performance implications but it feels
        // more correct
        bool anySkills = new List<string>(){
          "PKAPP_Mass", "PKAPP_Thermometer", "PKAPP_Price",
          "PKAPP_MutantSpotter", "PKAPP_CyberneticSpotter",
        }.Exists(s => player.HasSkill(s));
        if (anySkills) {
          var appraisals = new List<string>();
          bool eyeball = player.HasSkill("PKAPP_TransparentEyeBall");

          if (player.HasSkill("PKAPP_Mass") 
            && !ParentObject.pPhysics.Takeable) {
            // Takeable things already show their mass
            appraisals.Add(FooIs(eyeball, ParentObject, 
              GameText.VariableReplace("=pronouns.subjective= =verb:weigh:afterpronoun= ", ParentObject),
              "#",
              ParentObject.Weight));
            if (ParentObject.GetPart<Inventory>() is Inventory inv) {
              appraisals.Add(FooIs(eyeball, ParentObject, 
                GameText.VariableReplace("=pronouns.subjective= =verb:are:afterpronoun= carrying ", ParentObject),
                "#",
                inv.GetWeight()));
            }
          }
          if (player.HasSkill("PKAPP_Thermometer") 
            && ParentObject.pPhysics != null) {
            // \xF8 is the cp437 degree symbol
            appraisals.Add(ItsFooIs(eyeball, ParentObject, "temperature", "\xF8",
              ParentObject.pPhysics.Temperature));
          }
          if (player.HasSkill("PKAPP_Price")
            && ParentObject.pPhysics.Takeable
            && ParentObject.GetPart<Commerce>() != null) {
            // Commerce.Value gives the price per unit; we want the price per stack
            double price = ParentObject.Value;
            appraisals.Add(ItsFooIs(eyeball, ParentObject, "price", 
              price == 1 ? " dram" : " drams", (int)price));

            if (player.HasSkill("PKAPP_Mass")) {
              if (ParentObject.Weight <= 0) {
                appraisals.Add(ParentObject.its + " price per lb is infinite");
              } else {
                int pricePerPound = (int) Math.Round(price / ParentObject.Weight);
                appraisals.Add(ItsFooIs(eyeball, ParentObject, "price per lb", 
                  "$/#", pricePerPound));
              }
            }
          }

          if (player.HasSkill("PKAPP_MutantSpotter")) {
            var mutStrs = new List<string>();
            foreach (var list in new[]{ParentObject.GetPhysicalMutations(), ParentObject.GetMentalMutations()}) {
              foreach (var mutation in list) {
                string post = "";
                if (eyeball && mutation.CanLevel()) {
                  post = " at level " + mutation.Level;
                }
                mutStrs.Add(mutation.DisplayName + post);
              }
            }
            if (mutStrs.Count > 0) {
              mutStrs.Sort();
              appraisals.Add(
                GameText.VariableReplace("=pronouns.subjective= =verb:have:afterpronoun= the ", ParentObject)
                + (mutStrs.Count == 1 ? "mutation" : "mutations")
                + " "
                + Grammar.MakeAndList(mutStrs));
            }
          }

          if (player.HasSkill("PKAPP_CyberneticSpotter")) {
            List<GameObject> cybers = ParentObject.GetInstalledCybernetics();
            if (cybers != null) {
              var cyberStrings = new List<string>();
              foreach (GameObject cyber in ParentObject.GetInstalledCybernetics()) {
                BodyPart bp = ParentObject.FindCybernetics(cyber);
                if (bp != null) {
                  cyberStrings.Add(eyeball
                    ? cyber.GetDisplayName(
                        BaseOnly: true, WithIndefiniteArticle: true, IncludeAdjunctNoun: false
                      ) + " implanted in " + ParentObject.its + " " + bp.GetOrdinalName()
                    : bp.GetOrdinalName()
                  );
                }
              }

              if (cyberStrings.Count > 0) {
                string cyberDescription;
                if (eyeball) {
                  cyberDescription = Grammar.MakeAndList(cyberStrings);
                } else {
                  cyberDescription = (cyberStrings.Count == 1 ? "a cybernetic" : "cybernetics")
                    + " implanted in " + ParentObject.its + " " 
                    + Grammar.MakeAndList(cyberStrings);
                }
                // society if GameObject.ithas actually returned "it has"
                appraisals.Add(
                  GameText.VariableReplace(
                    "=pronouns.subjective= =verb:have:afterpronoun= ", ParentObject
                  ) + cyberDescription);
              }
            }
          }

          if (appraisals.Count > 0) {
            e.Postfix.Append("\nYou appraise that ");
            e.Postfix.Append(Grammar.MakeAndList(appraisals) + ".\n");
          }
        }
      }

      return base.HandleEvent(e);
    }

    public static string ItsFooIs(bool eyeball, GameObject query,
      string name, string post, int stat) {
      // the unit is always singular.
      // this way we write "[wardens esther's] weight *is* 123#",
      // not "their weight *are* 123#".
      return FooIs(eyeball, query, 
        query.its + " " + name + " is ", post, stat);
    }

    public static string FooIs(bool eyeball, GameObject query,
      string name, string post, int stat) {
      var bob = Event.NewStringBuilder();
      bob.Append(name)
        .Append(NumberRange(eyeball, stat, query))
        .Append(post);
      return bob.ToString();
    }

    public static string NumberRange(bool eyeball, int realValue, GameObject obj) {
      if (eyeball) return realValue.ToString(NUMBER_FORMAT);

      // for fascinating and unknowable reasons the objects' ids are stored
      // as strings.
      var random = new System.Random((int)obj.ID.GetStableHashCode32());

      int minRange = Math.Abs(realValue) < 5 
        ? 5 
        : realValue * 10 / 100;
      int maxRange = Math.Abs(realValue) < 10 
        ? 10
        : realValue * 30 / 100;

      int lower = random.Next(-maxRange, -minRange) + realValue;
      if (realValue >= 0) {
        // Prevent very light objects from maybe having negative mass.
        lower = Math.Max(0, lower);
      }
      int upper = random.Next(minRange, maxRange) + realValue;
      return "between " + lower.ToString(NUMBER_FORMAT) + " and " + upper.ToString(NUMBER_FORMAT);
    }
  }  
}
