using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NHaml.TemplateBase;
using NUnit.Framework;

namespace NHaml.Tests.TemplateBase
{
    [TestFixture]
    public class Template_Tests
    {
        private class DummyTemplate : Template
        {
            public string RenderValueOrKeyAsString(IDictionary<string, object> dictionary, string keyName)
            {
                base.SetViewData(dictionary);
                return base.RenderValueOrKeyAsString(keyName);
            }

            public new string RenderAttributeNameValuePair(string name, string value, char quoteToUse)
            {
                return base.RenderAttributeNameValuePair(name, value, quoteToUse);
            }

            public string AppendSelfClosingTagSuffix(XmlVersion xmlVersion)
            {
                base.SetHtmlVersion(xmlVersion);
                return base.AppendSelfClosingTagSuffix();
            }
        }
        
        #region RenderValueOrKeyAsString

        [Test]
        [TestCase("FakeKeyName", "KeyName")]
        [TestCase("RealKeyName", "RealValue")]
        public void RenderValueOrKeyAsString_RealVsFakeKey_RendersKeyOrValueCorrectly(string keyName, string expecedValue)
        {
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("RealKeyName", "RealValue");

            var template = new DummyTemplate();
            string result  = template.RenderValueOrKeyAsString(dictionary, keyName);

            Assert.That(result, Is.StringContaining(expecedValue));
        }

        #endregion

        #region RenderAttributeNameValuePair

        [Test]
        [TestCase("a", "b", " a=\"b\"")]
        [TestCase("", "a", "")]
        [TestCase("a", "false", "")]
        public void RenderAttributeNameValuePair_VaryingNameValuePairs_GeneratesCorrectValue(string name, string value, string expectedOutput)
        {
            var template = new DummyTemplate();
            string result = template.RenderAttributeNameValuePair(name, value, '\"');
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCase("checked", "true", XmlVersion.XHtml, " checked=\"checked\"")]
        [TestCase("checked", "TRUE", XmlVersion.XHtml, " checked=\"checked\"")]
        [TestCase("checked", "", XmlVersion.XHtml, "")]
        [TestCase("checked", "false", XmlVersion.XHtml, "")]
        [TestCase("checked", "FALSE", XmlVersion.XHtml, "")]
        [TestCase("checked", "true", XmlVersion.Html5, " checked")]
        [TestCase("checked", "", XmlVersion.Html5, "")]
        [TestCase("checked", "true", XmlVersion.Html4, " checked")]
        [TestCase("checked", "", XmlVersion.Html4, "")]
        public void RenderAttributeNameValuePair_BooleanAttribute_WritesCorrectAttributes(string name, string value, XmlVersion xmlVersion, string expectedOutput)
        {
            var template = new DummyTemplate();
            template.SetHtmlVersion(xmlVersion);
            string result = template.RenderAttributeNameValuePair(name, value, '\"');

            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCase('\"')]
        [TestCase('\'')]
        public void RenderAttributeNameValuePair_VaryingQuoteTypes_RendersCorrectQuotes(char quoteToUse)
        {
            const string name = "name";
            const string value = "value";
            var template = new DummyTemplate();
            string result = template.RenderAttributeNameValuePair(name, value, quoteToUse);

            string expectedOutput = " " + name + "=" + quoteToUse + value + quoteToUse;
            Assert.That(result, Is.EqualTo(expectedOutput));
        }
        #endregion

        #region AppendSelfClosingTagSuffix

        [Test]
        [TestCase(XmlVersion.Html4, " />")]
        [TestCase(XmlVersion.XHtml, " />")]
        [TestCase(XmlVersion.Html5, " />")]
        public void AppendSelfClosingTagSuffix_VaryingHtmlVersion_AppendsCorrectOutput(XmlVersion xmlVersion, string expectedOutput)
        {
            var template = new DummyTemplate();
            string result = template.AppendSelfClosingTagSuffix(xmlVersion);

            Assert.That(result, Is.EqualTo(expectedOutput));
        }
        
        #endregion
    }
}
