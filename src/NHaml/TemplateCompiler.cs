using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

using NHaml.Backends;
using NHaml.Backends.CSharp3;
using NHaml.Configuration;
using NHaml.Properties;
using NHaml.Rules;
using NHaml.Utils;

namespace NHaml
{
  public sealed class TemplateCompiler
  {
    private static readonly Regex _pathCleaner
      = new Regex(@"[-\\/\.:\s]", RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly string[] DefaultAutoClosingTags
      = new[] {"META", "IMG", "LINK", "BR", "HR", "INPUT"};

    private static readonly string[] DefaultReferences
      = new[]
          {
            typeof(INotifyPropertyChanged).Assembly.Location,
            typeof(TemplateCompiler).Assembly.Location
          };

    private static readonly string[] DefaultUsings
      = new[] {"System", "System.IO", "NHaml", "NHaml.Utils"};

    private readonly StringSet _autoClosingTags =
      new StringSet(DefaultAutoClosingTags);

    private readonly MarkupRule[] _markupRules
      = new MarkupRule[128];

    private readonly StringSet _references
      = new StringSet(DefaultReferences);

    private readonly StringSet _usings
      = new StringSet(DefaultUsings);

    private ICompilerBackend _compilerBackend;

    private Type _viewBaseType
      = typeof(CompiledTemplate);

    public TemplateCompiler()
    {
      AddRule(new EofMarkupRule());
      AddRule(new DocTypeMarkupRule());
      AddRule(new TagMarkupRule());
      AddRule(new ClassMarkupRule());
      AddRule(new IdMarkupRule());
      AddRule(new EvalMarkupRule());
      AddRule(new SilentEvalMarkupRule());
      AddRule(new PreambleMarkupRule());
      AddRule(new CommentMarkupRule());
      AddRule(new EscapeMarkupRule());
      AddRule(new PartialMarkupRule());

      _compilerBackend = new CSharp3CompilerBackend();

      LoadFromConfiguration();
    }

    public ICompilerBackend CompilerBackend
    {
      get { return _compilerBackend; }
      set
      {
        Invariant.ArgumentNotNull(value, "value");
        _compilerBackend = value;
      }
    }

    public bool IsProduction { get; set; }

    public Type ViewBaseType
    {
      get { return _viewBaseType; }
      set
      {
        Invariant.ArgumentNotNull(value, "value");

        if (!typeof(CompiledTemplate).IsAssignableFrom(value))
        {
          throw new InvalidOperationException(Resources.InvalidViewBaseType);
        }

        _viewBaseType = value;
        _usings.Add(_viewBaseType.Namespace);
        _references.Add(_viewBaseType.Assembly.Location);
      }
    }

    public IEnumerable<string> Usings
    {
      get { return _usings; }
    }

    public IEnumerable<string> References
    {
      get { return _references; }
    }

    public void LoadFromConfiguration()
    {
      NHamlSection section = NHamlSection.Read();

      if (section == null)
      {
        return;
      }

      IsProduction = section.Production;

      // Todo: rebuild configuration
      if (!string.IsNullOrEmpty(section.CompilerBackend))
      {
        _compilerBackend = section.CreateCompilerBackend();
      }

      foreach (AssemblyConfigurationElement assemblyConfigurationElement in section.Assemblies)
      {
        AddReference(Assembly.Load(assemblyConfigurationElement.Name).Location);
      }

      foreach (NamespaceConfigurationElement namespaceConfigurationElement in section.Namespaces)
      {
        AddUsing(namespaceConfigurationElement.Name);
      }
    }

    public void AddRule(MarkupRule markupRule)
    {
      Invariant.ArgumentNotNull(markupRule, "markupRule");

      _markupRules[markupRule.Signifier] = markupRule;
    }

    public MarkupRule GetRule(InputLine inputLine)
    {
      Invariant.ArgumentNotNull(inputLine, "line");

      if (inputLine.Signifier >= 128)
      {
        return NullMarkupRule.Instance;
      }

      return _markupRules[inputLine.Signifier] ?? NullMarkupRule.Instance;
    }

    public bool IsAutoClosing(string tag)
    {
      Invariant.ArgumentNotEmpty(tag, "tag");

      return _autoClosingTags.Contains(tag.ToUpperInvariant());
    }

    public void AddUsing(string @namespace)
    {
      Invariant.ArgumentNotEmpty(@namespace, "namespace");

      _usings.Add(@namespace);
    }

    public void AddReference(string assemblyLocation)
    {
      Invariant.ArgumentNotEmpty(assemblyLocation, "assemblyLocation");

      _references.Add(assemblyLocation);
    }

    public void AddReferences(Type type)
    {
      AddReference(type.Assembly.Location);

      if (!type.IsGenericType)
      {
        return;
      }

      foreach (Type t in type.GetGenericArguments())
      {
        AddReferences(t);
      }
    }

    public TemplateActivator<CompiledTemplate> Compile(string templatePath, params Type[] genericArguments)
    {
      return Compile<CompiledTemplate>(templatePath, genericArguments);
    }

    [SuppressMessage("Microsoft.Design", "CA1004")]
    public TemplateActivator<TView> Compile<TView>(string templatePath, params Type[] genericArguments)
    {
      return Compile<TView>(templatePath, null, genericArguments);
    }

    public TemplateActivator<CompiledTemplate> Compile(string templatePath, string layoutPath, params Type[] genericArguments)
    {
      return Compile<CompiledTemplate>(templatePath, layoutPath, genericArguments);
    }

    [SuppressMessage("Microsoft.Design", "CA1004")]
    public TemplateActivator<TView> Compile<TView>(string templatePath, string layoutPath, params Type[] genericArguments)
    {
      return Compile<TView>(templatePath, layoutPath, null, genericArguments);
    }

    public TemplateActivator<CompiledTemplate> Compile(string templatePath, string layoutPath,
      ICollection<string> inputFiles, params Type[] genericArguments)
    {
      return Compile<CompiledTemplate>(templatePath, layoutPath, inputFiles, genericArguments);
    }

    [SuppressMessage("Microsoft.Design", "CA1004")]
    public TemplateActivator<TView> Compile<TView>(string templatePath, string layoutPath,
      ICollection<string> inputFiles, params Type[] genericArguments)
    {
      Invariant.ArgumentNotEmpty(templatePath, "templatePath");
      Invariant.FileExists(templatePath);

      if (!string.IsNullOrEmpty(layoutPath))
      {
        Invariant.FileExists(layoutPath);
      }

      foreach (Type type in genericArguments)
      {
        AddReferences(type);
      }

      var compilationContext
        = new CompilationContext(
          this,
          _compilerBackend.AttributeRenderer,
          _compilerBackend.SilentEvalRenderer,
          _compilerBackend.CreateTemplateClassBuilder(ViewBaseType, MakeClassName(templatePath), genericArguments),
          templatePath,
          layoutPath);

      Compile(compilationContext);

      if (inputFiles != null)
      {
        compilationContext.CollectInputFiles(inputFiles);
      }

      return CreateFastActivator<TView>(_compilerBackend.BuildView(compilationContext));
    }

    private void Compile(CompilationContext compilationContext)
    {
      while (compilationContext.CurrentNode.Next != null)
      {
        MarkupRule rule = compilationContext.TemplateCompiler.GetRule(compilationContext.CurrentInputLine);

        if (compilationContext.CurrentInputLine.IsMultiline && rule.MergeMultiline)
        {
          compilationContext.CurrentInputLine.Merge(compilationContext.NextInputLine);
          compilationContext.InputLines.Remove(compilationContext.NextNode);
        }
        else
        {
          rule.Process(compilationContext);
        }
      }

      compilationContext.CloseBlocks();
    }

    private static TemplateActivator<TResult> CreateFastActivator<TResult>(Type type)
    {
      var dynamicMethod = new DynamicMethod("activatefast__", type, null, type);

      ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
      ConstructorInfo constructor = type.GetConstructor(new Type[] {});

      if (constructor == null)
      {
        return null;
      }

      ilGenerator.Emit(OpCodes.Newobj, constructor);
      ilGenerator.Emit(OpCodes.Ret);

      return (TemplateActivator<TResult>)dynamicMethod.CreateDelegate(typeof(TemplateActivator<TResult>));
    }

    private static string MakeClassName(string templatePath)
    {
      return _pathCleaner.Replace(templatePath, "_").TrimStart('_');
    }
  }
}