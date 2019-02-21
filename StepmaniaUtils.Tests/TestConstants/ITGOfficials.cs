using System.Collections.Generic;
using System.Linq;

namespace StepmaniaUtils.Tests
{
    public static class ITGOfficials
    {
        public static IEnumerable<object[]> Data =>
            typeof(ITGOfficials).GetConstants().Select(c => new[] {c.GetRawConstantValue()});

        public const string ANUBIS = "TestData/ITGOfficial/Anubis.sm";
        public const string BEND_YOUR_MIND = "TestData/ITGOfficial/Bend your mind.sm";
        public const string BOOGIE_DOWN = "TestData/ITGOfficial/Boogie Down.sm";
        public const string BOUFF = "TestData/ITGOfficial/Bouff.sm";
        public const string BUBBLE_DANCER = "TestData/ITGOfficial/Bubble Dancer.sm";
        public const string CHANGES = "TestData/ITGOfficial/Changes.sm";
        public const string CHARLENE = "TestData/ITGOfficial/Charlene.sm";
        public const string CRAZY = "TestData/ITGOfficial/crazy.sm";
        public const string DA_ROOTS_FOLK_MIX = "TestData/ITGOfficial/Da Roots (Folk Mix).sm";
        public const string DAWN = "TestData/ITGOfficial/Dawn.sm";
        public const string DELIRIUM = "TestData/ITGOfficial/Delirium.sm";
        public const string WHY_ME = "TestData/ITGOfficial/Desire - Why Me.sm";
        public const string DISCONNECTED_HYPER = "TestData/ITGOfficial/Disconnected -Hyper-.sm";
        public const string DISCONNECTED_MOBIUS = "TestData/ITGOfficial/Disconnected -Mobius-.sm";
        public const string DISCONNECTED = "TestData/ITGOfficial/disconnected.sm";
        public const string DJ_PARTY = "TestData/ITGOfficial/DJ Party.sm";
        public const string DO_U_LOVE_ME = "TestData/ITGOfficial/Do U Love Me.sm";
        public const string DONT_PROMISE_ME = "TestData/ITGOfficial/dontpromiseme.sm";
        public const string DRIFTING_AWAY = "TestData/ITGOfficial/Drifting Away.sm";
        public const string DRIVING_FORCE_CLASSICAL = "TestData/ITGOfficial/drivingforceclassical.sm";
        public const string EUPHORIA = "TestData/ITGOfficial/Euphoria.sm";
        public const string FLYING_HIGH = "TestData/ITGOfficial/Filo Bedo - Flying High.sm";
        public const string FLY_AWAY = "TestData/ITGOfficial/Flyaway.sm";
        public const string FLY_WITH_ME = "TestData/ITGOfficial/flywithme.sm";
        public const string HAND_OF_TIME = "TestData/ITGOfficial/Handoftime.sm";
        public const string HAPPINESS_COMES = "TestData/ITGOfficial/Happiness Comes.sm";
        public const string HARDCORE_OF_THE_NORTH = "TestData/ITGOfficial/hardcoreofthenorth.sm";
        public const string HIP_HOP_JAM = "TestData/ITGOfficial/HipHopJam.sm";
        public const string HYBRID = "TestData/ITGOfficial/Hybrid.sm";
        public const string I_THINK_I_LIKE_THAT_SOUND = "TestData/ITGOfficial/I think I like that sound.sm";
        public const string ILL_GET_THERE_ANYWAY = "TestData/ITGOfficial/I'll Get There Anyway.sm";
        public const string INFECTION = "TestData/ITGOfficial/infection.sm";
        public const string JULY = "TestData/ITGOfficial/July.sm";
        public const string KAGAMI = "TestData/ITGOfficial/Kagami.sm";
        public const string KISS_ME_RED = "TestData/ITGOfficial/Kiss me red.sm";
        public const string LEMMINGS_ON_THE_RUN = "TestData/ITGOfficial/lemmings on the Run.sm";
        public const string LET_ME_BE_THE_ONE = "TestData/ITGOfficial/Let Me Be The One.sm";
        public const string LET_MY_LOVE_GO_BLIND = "TestData/ITGOfficial/letmylovegoblind.sm";
        public const string PA_THEME = "TestData/ITGOfficial/MC Frontalot - PA Theme.sm";
        public const string WHICH_MC_WAS_THAT = "TestData/ITGOfficial/MC Frontalot - Which MC Was That.sm";
        public const string MELLOW = "TestData/ITGOfficial/Mellow.sm";
        public const string MY_FAVOURITE_GAME = "TestData/ITGOfficial/My Favourite Game.sm";
        public const string MYTHOLOGY = "TestData/ITGOfficial/Mythology.sm";
        public const string TORN = "TestData/ITGOfficial/Natalie Browne - Torn.sm";
        public const string OASIS = "TestData/ITGOfficial/Oasis.sm";
        public const string ON_A_DAY_LIKE_TODAY = "TestData/ITGOfficial/On a Day Like Today.sm";
        public const string PANDEMONIUM = "TestData/ITGOfficial/Pandemonium.sm";
        public const string PERFECT = "TestData/ITGOfficial/Perfect.sm";
        public const string QUEEN_OF_LIGHT = "TestData/ITGOfficial/queenoflight.sm";
        public const string REMEMBER_DECEMBER = "TestData/ITGOfficial/Remember December.sm";
        public const string LAND_OF_THE_RISING_SUN = "TestData/ITGOfficial/Rikki & Daz  - Land of the rising sun.sm";
        public const string MOUTH = "TestData/ITGOfficial/Rochelle - Mouth.sm";
        public const string ROMEO_JULI8 = "TestData/ITGOfficial/romeojuli8.sm";
        public const string SOLINA = "TestData/ITGOfficial/Solina.sm";
        public const string TELL = "TestData/ITGOfficial/Tell.sm";
        public const string TENSION = "TestData/ITGOfficial/Tension.sm";
        public const string THE_GAME = "TestData/ITGOfficial/The Game.sm";
        public const string THE_BEGINNING = "TestData/ITGOfficial/thebeginning.sm";
        public const string TOUCH_ME = "TestData/ITGOfficial/Touch Me.sm";
        public const string TOUGH_ENOUGH = "TestData/ITGOfficial/Tough Enough.sm";
        public const string TURN_IT_ON = "TestData/ITGOfficial/turniton.sm";
        public const string UTOPIA = "TestData/ITGOfficial/Utopia.sm";
        public const string VERTEX = "TestData/ITGOfficial/VerTex.sm";
        public const string WALKING_ON_FIRE = "TestData/ITGOfficial/Walking on fire.sm";
        public const string WHILE_THA_REKKID_SPINZ = "TestData/ITGOfficial/While Tha Rekkid Spinz.sm";
        public const string XUXA = "TestData/ITGOfficial/Xuxa.sm";
        public const string ZODIAC = "TestData/ITGOfficial/Zodiac.sm";
    }
}