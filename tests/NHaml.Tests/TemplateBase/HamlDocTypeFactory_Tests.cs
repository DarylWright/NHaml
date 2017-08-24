using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml.TemplateBase;
using NUnit.Framework;

namespace NHaml.Tests.TemplateBase
{
    [TestFixture]
    public class HamlDocTypeFactory_Tests
    {
        [Test]
        [TestCase("", XmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">")]
        [TestCase("strict", XmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">")]
        [TestCase("frameset", XmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Frameset//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd"">")]
        [TestCase("5", XmlVersion.XHtml, @"<!DOCTYPE html>")]
        [TestCase("1.1", XmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">")]
        [TestCase("basic", XmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML Basic 1.1//EN"" ""http://www.w3.org/TR/xhtml-basic/xhtml-basic11.dtd"">")]
        [TestCase("mobile", XmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//WAPFORUM//DTD XHTML Mobile 1.2//EN"" ""http://www.openmobilealliance.org/tech/DTD/xhtml-mobile12.dtd"">")]
        [TestCase("RDFa", XmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML+RDFa 1.0//EN"" ""http://www.w3.org/MarkUp/DTD/xhtml-rdfa-1.dtd"">")]
        [TestCase("", XmlVersion.Html4, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">")]
        [TestCase("strict", XmlVersion.Html4, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">")]
        [TestCase("frameset", XmlVersion.Html4, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01 Frameset//EN"" ""http://www.w3.org/TR/html4/frameset.dtd"">")]
        [TestCase("", XmlVersion.Html5, @"<!DOCTYPE html>")]
        [TestCase("XML", XmlVersion.XHtml, @"<?xml version=""1.0"" encoding=""utf-8"" ?>")]
        [TestCase("XML blah", XmlVersion.XHtml, @"<?xml version=""1.0"" encoding=""blah"" ?>")]
        [TestCase("XAML", XmlVersion.Xaml, @"<?xml version=""1.0"" encoding=""utf-8"" ?>")]
        [TestCase("XAML blah", XmlVersion.Xaml, @"<?xml version=""1.0"" encoding=""blah"" ?>")]
        public void Walk_ReturnsCorrectDocType(string docTypeId, XmlVersion xmlVersion, string expectedDocType)
        {
            var docType = DocTypeFactory.GetDocType(docTypeId, xmlVersion);
            Assert.That(docType, Is.EqualTo(expectedDocType));
        }
    }
}
