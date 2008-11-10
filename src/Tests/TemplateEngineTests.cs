using System.Linq;

using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public class TemplateEngineTests : TestFixtureBase
  {
    [Test]
    public void DuplicateUsings()
    {
      _templateEngine.AddUsing("System");

      Assert.AreEqual(6, _templateEngine.Usings.Count());
    }

    [Test]
    public void TemplatesAreCached()
    {
      var templatePath = TemplatesFolder + @"CSharp2\AttributeEval.haml";

      var compiledTemplate1 = _templateEngine.Compile(templatePath);
      var compiledTemplate2 = _templateEngine.Compile(templatePath);

      Assert.AreSame(compiledTemplate1, compiledTemplate2);
    }

    [Test]
    public void TemplatesWithLayoutsAreCached()
    {
      var templatePath = TemplatesFolder + @"CSharp2\Welcome.haml";
      var layoutTemplatePath = TemplatesFolder + @"Application.haml";

      var compiledTemplate1 = _templateEngine.Compile(templatePath, layoutTemplatePath);
      var compiledTemplate2 = _templateEngine.Compile(templatePath, layoutTemplatePath);

      Assert.AreSame(compiledTemplate1, compiledTemplate2);
    }
  }
}