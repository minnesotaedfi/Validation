using Engine.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest
{
    public class ExtensionMethodsTests
    {
        [TestClass]
        public class WhenConvertingStringToStreamAndBack
        {
            [TestMethod]
            public void Should_convert_successfully()
            {
                const string src =
                    @"Lorem ipsum dolor sit amet, his dico dolore petentium ut, sonet nobis putant at est! Ponderum forensibus has et, persius alienum iracundia eu ius. Ne nec patrioque persecuti, quo vocibus temporibus at, nam eu justo accusata. Fugit vitae ridens pro cu, usu nulla persecuti in!
Te cum legere debitis? Te quo alii democritum. Eu qui cibo duis illud, est an quaestio moderatius? Sea debet posidonium at, mel utroque probatus ut, eu ius nonumes epicuri. Esse purto probo in vix, id alia simul sea. Sit facer inimicus eu, dico consequuntur usu cu, te eos postea atomorum.
At sea esse audire accumsan. Enim inani elitr eam ea, mollis noluisse splendide has ei, accusam dolores suscipit cu has? Cum quodsi veritus volutpat in, eu quod euismod eum? Has ea probo errem, in mel salutatus dignissim? Ne wisi novum ius, vis novum iudico cu. Sea agam debet at, ex recteque neglegentur pro.
Ut civibus detracto nam. Ne his summo exerci, eligendi incorrupte ne mea, clita suscipit appetere quo eu. In diceret adipisci mea. Nec eu lorem fugit, modus atqui legere mel ne? Est eu nostrud probatus recteque!
Vim cu etiam dolorum, suscipiantur conclusionemque his et? Tacimates gloriatur te vis, pro id diceret eruditi scaevola! Mea ne sonet vocent invenire, quod reformidans duo no! Id cum tollit volutpat adipiscing! Vim in vero partem theophrastus.
Mel ut suas aperiri lucilius. Maiorum rationibus cu vis, solum aliquip utroque et mea. At blandit suavitate vix, in eos ipsum accusata, vix agam latine civibus cu. Facer dicam legimus eu sea.
At eos unum etiam errem, summo libris eum ex, quo nulla insolens ne. Mea vidit disputationi consequuntur ea? Vix eu tale impedit accommodare, id nec animal neglegentur, appareat vulputate constituam id his! Mei id aeterno apeirian partiendo, omnes saepe partiendo ad mea, ridens delenit mentitum pri ex? Timeam menandri ne eos, vidit atomorum laboramus eu sea!
Eu rebum doming pertinacia vix, in qui albucius hendrerit reprimique. Te illud mucius officiis duo? Ei sea gloriatur interesset, dolore corpora oportere in nam. Id exerci instructior per, cum propriae copiosae similique ex. Ius ne constituto contentiones. Quis etiam pri eu, eum ea alterum recusabo?
Ad usu tritani laoreet lucilius? No vix partem vulputate elaboraret? Te eius tritani instructior est, vix doming volutpat ei, natum adhuc sanctus eu vel! Cu eam impedit rationibus? Phaedrum recteque ea has! Sale commune qualisque est ne!
Cu sea intellegat elaboraret, ne zril periculis mel, omnes insolens qui in! Sint case eu duo, nec affert accusam omittam ei. Vel et quod deseruisse consetetur, id partem timeam incorrupte ius. Mea ne offendit appetere, eligendi adolescens nam ea? Ne aeque senserit qui?";
                using (var stream = src.ToStream())
                {
                    var result = stream.ReadToString();
                    Assert.AreEqual(src, result);
                }
            }
        }
    }
}
