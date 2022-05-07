using System.Collections.Generic;
using System.Linq;

namespace StepmaniaUtils.Tests.TestConstants
{
    public class Ssc
    {
        public static IEnumerable<object[]> Data =>
            typeof(Ssc).GetConstants().Select(c => new[] {c.GetRawConstantValue()});

        public const string SELFIE = "TestData/SSC/#SELFIE/#SELFIE.ssc";
        public const string STEPS = "TestData/SSC/(11) Way of the Wind/steps.ssc";
        public const string MEGABURN = "TestData/SSC/(12) Megaburn/megaburn.ssc";
        public const string EGG_DEALER = "TestData/SSC/(9) Egg Dealer/Egg Dealer.ssc";
        public const string CANTCONTROLMYSELF = "TestData/SSC/Can't Control Myself/cantcontrolmyself.ssc";
        public const string CANDYLAND = "TestData/SSC/Candyland (Pa's Lam System Remix)/Candyland.ssc";
        public const string CHICKEN_HEAD = "TestData/SSC/Chicken Head/Chicken Head.ssc";
        public const string GIRLS = "TestData/SSC/Girls Like To Swing/girls.ssc";
        public const string HELLO = "TestData/SSC/Hello/Hello.ssc";
        public const string PROPAGANDA_HABSTRAKT_REMIX = "TestData/SSC/Propaganda (Habstrakt Remix)/Propaganda (Habstrakt Remix).ssc";
        public const string WANDERING = "TestData/SSC/Wandering/wandering.ssc";
    }
}