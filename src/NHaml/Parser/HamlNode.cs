﻿using System.Collections.Generic;
using System.Linq;
using NHaml.IO;
using NHaml.Parser.Rules;

namespace NHaml.Parser
{
    public abstract class HamlNode
    {
        private readonly IList<HamlNode> _children = new List<HamlNode>();
        private readonly HamlLine _line;

        protected HamlNode(HamlLine nodeLine)
        {
            _line = nodeLine;
            Content = nodeLine.Content;
            SourceFileLineNum = _line.SourceFileLineNo;
        }

        protected HamlNode(int sourceFileLineNum, string content)
            : this(new HamlLine(content, HamlRuleEnum.Unknown, "", sourceFileLineNum, true))
        { }

        protected abstract bool IsContentGeneratingTag { get; }

        public string Content { get; }

        public string Indent => _line.HamlRule == HamlRuleEnum.Document ? "" : _line.Indent;

        public bool IsMultiLine
        {
            get { return Children.Any(x => x.IsInline == false); }
        }

        protected bool IsInline => _line.IsInline;

        public int IndentCount => _line.HamlRule == HamlRuleEnum.Document ? -1 : _line.IndentCount;

        public int SourceFileLineNum { get; }

        public IEnumerable<HamlNode> Children => _children;

        public void AddChild(HamlNode hamlNode)
        {
            hamlNode.Parent = this;
            hamlNode.Previous = _children.LastOrDefault();
            if (hamlNode.Previous != null)
                hamlNode.Previous.Next = hamlNode;

            _children.Add(hamlNode);
        }

        public HamlNode Previous { get; private set; }
        public HamlNode Next { get; private set; }
        public HamlNode Parent { get; private set; }

        private HamlNode PreviousNonWhitespaceNode()
        {
            var node = Previous;
            while (node != null && node.IsWhitespaceNode())
                node = node.Previous;
            return node;
        }

        private HamlNode NextNonWhitespaceNode()
        {
            var node = Next;
            while (node != null && node.IsWhitespaceNode())
                node = node.Next;
            return node;
        }

        public bool IsWhitespaceNode()
        {
            var container = this as HamlNodeTextContainer;

            return container != null && container.IsWhitespace();
        }

        public bool IsLeadingWhitespaceTrimmed
        {
            get
            {
                if (IsSurroundingWhitespaceRemoved())
                    return true;

                var previousNonWhitespaceNode = PreviousNonWhitespaceNode();
                if (previousNonWhitespaceNode != null && previousNonWhitespaceNode.IsSurroundingWhitespaceRemoved())
                    return true;
                if (previousNonWhitespaceNode == null)
                {
                    var parentNode = ParentNonWhitespaceNode();
                    if (parentNode != null && parentNode.IsInternalWhitespaceTrimmed())
                        return true;
                }
                return false;
            }
        }

        public bool IsTrailingWhitespaceTrimmed
        {
            get
            {
                var nextNonWhitespaceNode = NextNonWhitespaceNode();
                if (nextNonWhitespaceNode != null && nextNonWhitespaceNode.IsSurroundingWhitespaceRemoved())
                    return true;
                
                if (nextNonWhitespaceNode == null)
                {
                    var parentNode = ParentNonWhitespaceNode();
                    if (parentNode != null && parentNode.IsInternalWhitespaceTrimmed())
                        return true;
                }
                return false;
            }
        }

        private bool IsInternalWhitespaceTrimmed()
        {
            var tag = this as HamlNodeTag;

            return tag != null && tag.WhitespaceRemoval == WhitespaceRemoval.Internal;
        }

        private bool IsSurroundingWhitespaceRemoved()
        {
            var tag = this as HamlNodeTag;

            return tag != null && tag.WhitespaceRemoval == WhitespaceRemoval.Surrounding;
        }

        private HamlNode ParentNonWhitespaceNode()
        {
            var parentNode = Parent;
            while (parentNode != null && parentNode.IsWhitespaceNode())
                parentNode = parentNode.Parent;
            return parentNode;
        }

        public void AppendInnerTagNewLine()
        {
            if (IsContentGeneratingTag)
                AddChild(new HamlNodeTextContainer(new HamlLine("\n", HamlRuleEnum.PlainText, "", SourceFileLineNum, false)));
        }

        public void AppendPostTagNewLine(HamlNode childNode, int lineNo)
        {
            if (childNode.IsContentGeneratingTag)
                AddChild(new HamlNodeTextContainer(new HamlLine("\n", HamlRuleEnum.PlainText, "", lineNo, false)));
        }

        public HamlNodePartial GetNextUnresolvedPartial()
        {
            foreach (var childNode in Children)
            {
                var partialNode = childNode as HamlNodePartial;
                if (partialNode != null && partialNode.IsResolved == false)
                    return (HamlNodePartial)childNode;

                var childPartialNode = childNode.GetNextUnresolvedPartial();
                if (childPartialNode != null)
                    return childPartialNode;
            }

            return null;
        }
    }
}
