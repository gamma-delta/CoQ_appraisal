using System.Collections.Generic;
using System;
using XRL.Language;

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
          "PKAPP_MutantSpotter","PKAPP_CyberneticSpotter",
        }.Exists(s => player.HasSkill(s));
        if (anySkills) {
          var appraisals = new List<string>();
          bool eyeball = player.HasSkill("PKAPP_TransparentEyeBall");

          if (player.HasSkill("PKAPP_Mass") 
            && !ParentObject.pPhysics.Takeable) {
            // Takeable things already show their mass
            appraisals.Add(FooIs(eyeball, ParentObject, 
              ParentObject.it + " " + ParentObject.GetVerb("weigh") + " ", 
              " lbs",
              ParentObject.Weight));
            if (ParentObject.GetPart<Inventory>() is Inventory inv) {
              appraisals.Add(FooIs(eyeball, ParentObject, 
                ParentObject.it + " " + ParentObject.Is + " carrying ", 
                " lbs",
                inv.GetWeight()));
            }
          }
          if (player.HasSkill("PKAPP_Thermometer") 
            && ParentObject.pPhysics != null) {
            appraisals.Add(ItsFooIs(eyeball, ParentObject, "temperature", "\xF8",
              ParentObject.pPhysics.Temperature));
          }
          if (player.HasSkill("PKAPP_Price")
            && ParentObject.pPhysics.Takeable
            && ParentObject.GetPart<Commerce>() is Commerce commerce) {
            var price = (int) ParentObject.Value;
            appraisals.Add(ItsFooIs(eyeball, ParentObject, "price", 
              price == 1 ? " dram" : " drams", price));
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
              appraisals.Add(
                ParentObject.it + " " + ParentObject.Has
                + " the " 
                + (mutStrs.Count == 1 ? "mutation" : "mutations")
                + " "
                + Grammar.MakeAndList(mutStrs));
            }
          }

          if (player.HasSkill("PKAPP_CyberneticSpotter")) {
            var cybers = ParentObject.GetInstalledCybernetics();
            if (cybers != null) {
              var cyberStrings = new List<string>();
              foreach (var cyber in ParentObject.GetInstalledCybernetics()) {
                var bp = ParentObject.FindCybernetics(cyber);
                if (bp != null) {
                  cyberStrings.Add(eyeball
                    ? cyber.a + " " + cyber.BaseDisplayName + " implanted in " + ParentObject.its + " " + bp.GetOrdinalName()
                    : bp.GetOrdinalName());
                }
              }

              if (cyberStrings.Count > 0) {
                if (eyeball) {
                  appraisals.Add(ParentObject.it + " has " + Grammar.MakeAndList(cyberStrings));
                } else {
                  appraisals.Add(ParentObject.it + " " + ParentObject.Has
                    + " "
                    + (cyberStrings.Count == 1 ? "a cybernetic" : "cybernetics")
                    + " implanted in " + ParentObject.its + " " 
                    + Grammar.MakeAndList(cyberStrings));
                }
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
      return FooIs(eyeball, query, 
        query.its + " " + name + " " + query.Is + " ", post, stat);
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
      var random = new System.Random((int)obj.id.GetStableHashCode32());

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
