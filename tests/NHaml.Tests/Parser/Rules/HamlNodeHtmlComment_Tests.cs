using NHaml.IO;
using NHaml.Parser;
using NHaml.Parser.Rules;
using NUnit.Framework;

namespace NHaml.Tests.Parser.Rules
{
    [TestFixture]
    public class HamlNodeHtmlComment_Tests
    {
        [Test]
        public void Constructor_NormalUse_PopulatesCommentTextProperty()
        {
            string comment = "Test comment";

            var node = new HamlNodeXmlComment(new HamlLine(comment, HamlRuleEnum.XmlComment, "", 0));
            Assert.That(node.Content, Is.EqualTo(comment));
        }    
    }
}
