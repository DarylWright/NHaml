using NHaml.IO;
using NHaml.Parser;
using NHaml.Parser.Exceptions;
using NHaml.Parser.Rules;
using NUnit.Framework;
using System;

namespace NHaml.Tests.Parser
{
    [TestFixture]
    public class HamlNodeFactory_Tests
    {
        [Test]
        [TestCase(HamlRuleEnum.PlainText, typeof(HamlNodeTextContainer))]
        [TestCase(HamlRuleEnum.Tag, typeof(HamlNodeTag))]
        [TestCase(HamlRuleEnum.HamlComment, typeof(HamlNodeHamlComment))]
        [TestCase(HamlRuleEnum.XmlComment, typeof(HamlNodeXmlComment))]
        [TestCase(HamlRuleEnum.Evaluation, typeof(HamlNodeEval))]
        public void GetHamlNode_DifferentHamlLineTypes_ReturnsCorrectHamlNode(HamlRuleEnum rule, Type nodeType)
        {
            var line = new HamlLine("Blah", rule, "", 0);
            var result = HamlNodeFactory.GetHamlNode(line);
            Assert.That(result, Is.InstanceOf(nodeType));
        }

        [Test]
        [TestCase(HamlRuleEnum.ViewProperty, typeof(HamlNodeTag))]
        //[TestCase(HamlRuleEnum.DivId, typeof(HamlNodeTag))]
        public void GetHamlNode_TagSubTypes_ThrowsHamlUnknownRuleException(HamlRuleEnum rule, Type nodeType)
        {
            var line = new HamlLine("Blah", rule, "", 0);
            Assert.Throws<HamlUnknownRuleException>(() => HamlNodeFactory.GetHamlNode(line));
        }
    }
}
