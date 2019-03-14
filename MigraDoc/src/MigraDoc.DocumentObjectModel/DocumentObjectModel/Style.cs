#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Stefan Lange
//   Klaus Potzesny
//   David Stephensen
//
// Copyright (c) 2001-2019 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://www.migradoc.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using MigraDoc.DocumentObjectModel.Internals;
using MigraDoc.DocumentObjectModel.Visitors;

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// Represents style templates for paragraph or character formatting.
    /// </summary>
    public sealed class Style : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Style class.
        /// </summary>
        internal Style()
        { }

        /// <summary>
        /// Initializes a new instance of the Style class with the specified parent.
        /// </summary>
        internal Style(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Initializes a new instance of the Style class with name and base style name.
        /// </summary>
        public Style(string name, string baseStyleName)
            : this()
        {
            // baseStyleName can be null or empty
            if (name == null)
                throw new ArgumentNullException("name");
            if (name == "")
                throw new ArgumentException("name");

            _name.Value = name;
            _baseStyle.Value = baseStyleName;
        }

        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Style Clone()
        {
            return (Style)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Style style = (Style)base.DeepCopy();
            if (style._paragraphFormat != null)
            {
                style._paragraphFormat = style._paragraphFormat.Clone();
                style._paragraphFormat._parent = style;
            }
            return style;
        }

        /// <summary>
        /// Returns the value with the specified name and value access.
        /// </summary>
        public override object GetValue(string name, GV flags) //newStL
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name == "")
                throw new ArgumentException("name");

            if (name.ToLower().StartsWith("font"))
                return ParagraphFormat.GetValue(name);
            return base.GetValue(name, flags);
        }

        #region Properties
        /// <summary>
        /// Indicates whether the style is read-only. 
        /// </summary>
        public bool IsReadOnly
        {
            get { return _readOnly; }
            internal set { _readOnly = value; }
        }
        bool _readOnly;

        /// <summary>
        /// Gets the font of ParagraphFormat. 
        /// Calling style.Font is just a shortcut to style.ParagraphFormat.Font.
        /// </summary>
        [DV]
        public Font Font
        {
            get { return ParagraphFormat.Font; }
            // SetParent will be called inside ParagraphFormat.
            set { ParagraphFormat.Font = value; }
        }

        /// <summary>
        /// Gets the name of the style.
        /// </summary>
        public string Name
        {
            get { return _name.Value; }
        }
        [DV]
        internal NString _name = NString.NullValue;

        /// <summary>
        /// Gets the ParagraphFormat. To prevent read-only styles from being modified, a copy of its ParagraphFormat
        /// is returned in this case.
        /// </summary>
        public ParagraphFormat ParagraphFormat
        {
            get
            {
                if (_paragraphFormat == null)
                    _paragraphFormat = new ParagraphFormat(this);
                if (_readOnly)
                    return _paragraphFormat.Clone();
                return _paragraphFormat;
            }
            set
            {
                SetParent(value);
                _paragraphFormat = value;
            }
        }
        [DV]
        internal ParagraphFormat _paragraphFormat;

        /// <summary>
        /// Gets or sets the name of the base style.
        /// </summary>
        public string BaseStyle
        {
            get { return _baseStyle.Value; }
            set
            {
                if (value == null || value == "" && _baseStyle.Value != "")
                    throw new ArgumentException(DomSR.EmptyBaseStyle);

                // Self assignment is allowed
                if (String.Compare(_baseStyle.Value, value, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    _baseStyle.Value = value;  // character case may change...
                    return;
                }

                if (String.Compare(_name.Value, DefaultParagraphName, StringComparison.OrdinalIgnoreCase) == 0 ||
                    String.Compare(_name.Value, DefaultParagraphFontName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    string msg = String.Format("Style '{0}' has no base style and that cannot be altered.", _name);
                    throw new ArgumentException(msg);
                }

                Styles styles = (Styles)_parent;
                // The base style must exists
                int idxBaseStyle = styles.GetIndex(value);
                if (idxBaseStyle == -1)
                {
                    string msg = String.Format("Base style '{0}' does not exist.", value);
                    throw new ArgumentException(msg);
                }
                if (idxBaseStyle > 1)
                {
                    // Is this style in the base style chain of the new base style
                    Style style = styles[idxBaseStyle] as Style;
                    while (style != null)
                    {
                        if (style == this)
                        {
                            string msg = String.Format("Base style '{0}' leads to a circular dependency.", value);
                            throw new ArgumentException(msg);
                        }
                        style = styles[style.BaseStyle];
                    }
                }

                // Now setting new base style is safe
                _baseStyle.Value = value;
            }
        }
        [DV]
        internal NString _baseStyle = NString.NullValue;

        /// <summary>
        /// Gets the StyleType of the style.
        /// </summary>
        public StyleType Type
        {
            get
            {
                //old
                //if (IsNull("Type"))
                //{
                //  if (String.Compare (this .baseStyle.Value, DefaultParagraphFontName, true) == 0)
                //    SetValue("Type", StyleType.Character);
                //  else
                //  {
                //    Style bsStyle = GetBaseStyle();
                //    if (bsStyle == null)
                //      throw new ArgumentException("User defined style has no valid base Style.");
                //
                //    SetValue("Type", bsStyle.Type);
                //  }
                //}
                //return styleType;

                if (_styleType.IsNull)
                {
                    if (String.Compare(_baseStyle.Value, DefaultParagraphFontName, StringComparison.OrdinalIgnoreCase) == 0)
                        _styleType.Value = (int)StyleType.Character;
                    else
                    {
                        Style baseStyle = GetBaseStyle();
                        if (baseStyle == null)
                            throw new InvalidOperationException("User defined style has no valid base Style.");

                        _styleType.Value = (int)baseStyle.Type;
                    }
                }
                return (StyleType)_styleType.Value;
            }
        }
        [DV(Type = typeof(StyleType))]
        internal NEnum _styleType = NEnum.NullValue(typeof(StyleType));

        /// <summary>
        /// Determines whether the style is the style Normal or DefaultParagraphFont.
        /// </summary>
        internal bool IsRootStyle
        {
            get
            {
                return String.Compare(Name, DefaultParagraphFontName, StringComparison.OrdinalIgnoreCase) == 0 ||
                       String.Compare(Name, DefaultParagraphName, StringComparison.OrdinalIgnoreCase) == 0;
            }
        }

        /// <summary>
        /// Get the BaseStyle of the current style.
        /// </summary>
        public Style GetBaseStyle()
        {
            if (IsRootStyle)
                return null;

            Styles styles = Parent as Styles;
            if (styles == null)
                throw new InvalidOperationException("A parent object is required for this operation; access failed");
            if (_baseStyle.Value == "")
                throw new ArgumentException("User defined Style defined without a BaseStyle");

            // TODO Remove German remarks!
            //REVIEW KlPo4StLa Spezialbehandlung für den DefaultParagraphFont krüppelig(DefaultParagraphFont wird bei Zugriff über styles["name"] nicht zurückgeliefert).
            if (_baseStyle.Value == DefaultParagraphFontName)
                return styles[0];

            return styles[_baseStyle.Value];
        }

        /// <summary>
        /// Indicates whether the style is a predefined (build in) style.
        /// </summary>
        public bool BuildIn
        {
            get { return _buildIn.Value; }
        }
        [DV]
        internal NBool _buildIn = NBool.NullValue;
        // TODO: rename to builtIn.

        /// <summary>
        /// Gets or sets a comment associated with this object.
        /// </summary>
        public string Comment
        {
            get { return _comment.Value; }
            set { _comment.Value = value; }
        }
        [DV]
        internal NString _comment = NString.NullValue;
        #endregion

        // Names of the root styles. Root styles have no BaseStyle.

        /// <summary>
        /// Name of the default character style.
        /// </summary>
        public const string DefaultParagraphFontName = StyleNames.DefaultParagraphFont;

        /// <summary>
        /// Name of the default paragraph style.
        /// </summary>
        public const string DefaultParagraphName = StyleNames.Normal;

        #region Internal
        /// <summary>
        /// Converts Style into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
#if DEBUG_ // Test
      if (Name == StyleNames.Heading1 || Name == StyleNames.Heading2)
        Name.GetType();
#endif

            // For build-in styles all properties that differ from their default values
            // are serialized.
            // For user-defined styles all non-null properties are serialized.
            Styles buildInStyles = Styles.BuildInStyles;
            Style refStyle = null;
            Font refFont = null;
            ParagraphFormat refFormat = null;

            serializer.WriteComment(_comment.Value);
            if (_buildIn.Value)
            {
                // BaseStyle is never null, but empty only for "Normal" and "DefaultParagraphFont"
                if (BaseStyle == "")
                {
                    // case: style is "Normal"
                    if (String.Compare(_name.Value, DefaultParagraphName, StringComparison.OrdinalIgnoreCase) != 0)
                        throw new ArgumentException("Internal Error: BaseStyle not set.");

                    refStyle = buildInStyles[buildInStyles.GetIndex(Name)];
                    refFormat = refStyle.ParagraphFormat;
                    refFont = refFormat.Font;
                    string name = DdlEncoder.QuoteIfNameContainsBlanks(Name);
                    serializer.WriteLineNoCommit(name);
                }
                else
                {
                    // case: any build-in style except "Normal"
                    refStyle = buildInStyles[buildInStyles.GetIndex(Name)];
                    refFormat = refStyle.ParagraphFormat;
                    refFont = refFormat.Font;
                    if (String.Compare(BaseStyle, refStyle.BaseStyle, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // case: build-in style with unmodified base style name
                        string name = DdlEncoder.QuoteIfNameContainsBlanks(Name);
                        serializer.WriteLineNoCommit(name);
                        // It's fine if we have the predefined base style, but ...
                        // ... the base style may have been modified or may even have a modified base style.
                        // Methinks it's wrong to compare with the built-in style, so let's compare with the
                        // real base style:
                        refStyle = Document.Styles[Document.Styles.GetIndex(_baseStyle.Value)];
                        refFormat = refStyle.ParagraphFormat;
                        refFont = refFormat.Font;
                        // Note: we must write "Underline = none" if the base style has "Underline = single" - we cannot
                        // detect this if we compare with the built-in style that has no underline.
                        // Known problem: Default values like "OutlineLevel = Level1" will now be serialized
                        // TODO: optimize...
                    }
                    else
                    {
                        // case: build-in style with modified base style name
                        string name = DdlEncoder.QuoteIfNameContainsBlanks(Name);
                        string baseName = DdlEncoder.QuoteIfNameContainsBlanks(BaseStyle);
                        serializer.WriteLine(name + " : " + baseName);
                        refStyle = Document.Styles[Document.Styles.GetIndex(_baseStyle.Value)];
                        refFormat = refStyle.ParagraphFormat;
                        refFont = refFormat.Font;
                    }
                }
            }
            else
            {
                // case: user-defined style; base style always exists

                string name = DdlEncoder.QuoteIfNameContainsBlanks(Name);
                string baseName = DdlEncoder.QuoteIfNameContainsBlanks(BaseStyle);
                serializer.WriteLine(name + " : " + baseName);

#if true
                Style refStyle0 = Document.Styles[Document.Styles.GetIndex(_baseStyle.Value)];
                refStyle = Document.Styles[_baseStyle.Value];
                refFormat = refStyle != null ? refStyle.ParagraphFormat : null;
#else
        refFormat = null;
#endif
            }

            serializer.BeginContent();

            if (!IsNull("ParagraphFormat"))
            {
                if (!ParagraphFormat.IsNull("Font"))
                    Font.Serialize(serializer, refFormat != null ? refFormat.Font : null);

                if (Type == StyleType.Paragraph)
                    ParagraphFormat.Serialize(serializer, "ParagraphFormat", refFormat);
            }

            serializer.EndContent();
        }

        /// <summary>
        /// Sets all properties to Null that have the same value as the base style.
        /// </summary>
        private void Optimize()
        {
            // just here as a reminder to do it...
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and its child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitStyle(this);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Style))); }
        }
        static Meta _meta;
        #endregion
    }
}
