using WarForCybertron.Model;

namespace WarForCybertron.Service.Helpers
{
    public static class TransformerHelpers
    {
        public static Transformer TestForCowardice(Transformer autobot, Transformer decepticon)
        {
            var strength = AutobotPrevails(autobot.Strength, decepticon.Strength, 3);

            // if both conditions match, then determine which Transformer won, with Autobots equating to true
            if (!(strength ^ decepticon.Courage < 5))
            {
                return strength ? autobot : decepticon;
            }

            return null;
        }

        private static bool AutobotPrevails(int autobotProperty, int decepticonProperty, int differential) => autobotProperty > decepticonProperty + differential;

        public static Transformer TransformerBattle(Transformer autobot, Transformer decepticon)
        {
            Transformer transformer;

            // if Optimus is on one team and Predaking is on another team (i.e. two Transformers who possess God-Mode), the battle ends with no victors on either side
            if (autobot.GodMode && decepticon.GodMode)
            {
                return null;
            }
            else if (autobot.GodMode || decepticon.GodMode) // whichever Transformer possesses God-Mode is the automatic winner
            {
                transformer = autobot.GodMode ? autobot : decepticon;
            }
            else
            {
                var cowardice = TestForCowardice(autobot, decepticon);

                transformer = cowardice ?? (AutobotPrevails(autobot.Skill, decepticon.Skill, 5) ? autobot : CompareOverallRating(autobot, decepticon));
            }

            return transformer;
        }

        private static Transformer CompareOverallRating(Transformer autobot, Transformer decepticon)
        {
            Transformer transformer = null;
            
            if (autobot.OverallRating != decepticon.OverallRating)
            {
                transformer = AutobotPrevails(autobot.OverallRating, decepticon.OverallRating, 0) ? autobot : decepticon;
            }

            return transformer;
        }
    }
}
