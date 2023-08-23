using XRL.Wish;
using XRL.World.Skills;
using XRL;

namespace petrakat.appraisal {
  [HasWishCommand]
  public class MyWishHandler {
    [WishCommand(Command = "pkapp_skills")]
    public static bool Skills() {
      The.Player.AddSkill("PKAPP_AppraisalSkillTree");
      var root = SkillFactory.Factory.SkillByClass["PKAPP_AppraisalSkillTree"];
      foreach (var skill in root.PowerList) {
        The.Player.AddSkill(skill.Class);
      }

      return true;
    }
        
    [WishCommand(Command = "pkapp_bigbrain")]
    public static bool AddEyeball() {
      The.Player.AddSkill("PKAPP_TransparentEyeBall");
      return true;
    }

    [WishCommand(Command = "pkapp_smallbrain")]
    public static bool RemoveEyeball() {
      The.Player.RemoveSkill("PKAPP_TransparentEyeBall");
      return true;
    }
  }
}
