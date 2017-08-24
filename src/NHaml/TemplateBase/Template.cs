using System;
using System.IO;
using System.Collections.Generic;
using NHaml.Crosscutting;

namespace NHaml.TemplateBase
{
    /// <summary>
    /// This is the base class for dynamic Haml templates.
    /// </summary>
    public abstract class Template
    {
        // ReSharper disable UnusedMember.Global
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable VirtualMemberNeverOverriden.Global
        public IDictionary<string, object> ViewData { get; set; }

        private XmlVersion _xmlVersion;
        protected bool HasCodeBlockRepeated;

        /// <summary>
        /// Renders the Haml document to the <see cref="TextWriter"/> <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> this method renders to.</param>
        public void Render(TextWriter writer)
        {
            //TODO: HtmlVersion is not relevant to Xaml
            Render(writer, XmlVersion.XHtml, ViewData ?? new Dictionary<string, object>());
        }

        /// <summary>
        /// Renders the Haml document to the <see cref="TextWriter"/> <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> this method renders to.</param>
        /// <param name="xmlVersion"></param>
        public void Render(TextWriter writer, XmlVersion xmlVersion)
        {
            Render(writer, xmlVersion, ViewData ?? new Dictionary<string, object>());
        }

        /// <summary>
        /// Renders the Haml document to the <see cref="TextWriter"/> <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> this method renders to.</param>
        /// <param name="xmlVersion"></param>
        /// <param name="viewData"></param>
        public void Render(TextWriter writer, XmlVersion xmlVersion, IDictionary<string, object> viewData)
        {
            Invariant.ArgumentNotNull(writer, "textWriter");
            _xmlVersion = xmlVersion;
            ViewData = viewData;
            HasCodeBlockRepeated = false;
            CoreRender(writer);
        }

        /// <summary>
        /// Handles the core rendering for the Haml template.
        /// </summary>
        /// <remarks>
        /// This method gets overriden by the dynamic <see cref="Template"/> subclass.
        /// </remarks>
        /// <param name="textWriter">The <see cref="TextWriter"/> this method renders to.</param>
        protected virtual void CoreRender(TextWriter textWriter)
        {
        }

        protected string RenderValueOrKeyAsString(string keyName)
        {
            return !string.IsNullOrEmpty(keyName) && ViewData.ContainsKey(keyName)
                ? Convert.ToString(ViewData[keyName])
                : keyName;
        }

        protected string RenderAttributeNameValuePair(string name, string value, char quoteToUse)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value) || value.ToLower() == "false")
                return "";
            if (value.ToLower() == "true" || string.IsNullOrEmpty(value))
                return _xmlVersion == XmlVersion.XHtml
                           ? " " + name + "=" + quoteToUse + name + quoteToUse
                           : " " + name;
            return " " + name + "=" + quoteToUse + value + quoteToUse;
        }

        protected string AppendSelfClosingTagSuffix()
        {
            //return _xmlVersion == XmlVersion.XHtml ? " />" : ">";
            return " />";
        }

        protected void SetViewData(IDictionary<string, object> viewData)
        {
            ViewData = viewData;
        }

        public void SetHtmlVersion(XmlVersion xmlVersion)
        {
            _xmlVersion = xmlVersion;
        }

        public void WriteNewLineIfRepeated(TextWriter writer)
        {
            if (HasCodeBlockRepeated) writer.WriteLine();
            HasCodeBlockRepeated = true;
        }

        public string GetDocType(string docTypeId)
        {
            return DocTypeFactory.GetDocType(docTypeId, _xmlVersion);
        }
        // ReSharper restore VirtualMemberNeverOverriden.Global
        // ReSharper restore UnusedMember.Global
        // ReSharper restore MemberCanBePrivate.Global
    }
}
