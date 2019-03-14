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
    /// Represents a MigraDoc document.
    /// </summary>
    public sealed class Document : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Document class.
        /// </summary>
        public Document()
        {
            _styles = new Styles(this);
        }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Document Clone()
        {
            return (Document)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Document document = (Document)base.DeepCopy();
            if (document._info != null)
            {
                document._info = document._info.Clone();
                document._info._parent = document;
            }
            if (document._styles != null)
            {
                document._styles = document._styles.Clone();
                document._styles._parent = document;
            }
            if (document._sections != null)
            {
                document._sections = document._sections.Clone();
                document._sections._parent = document;
            }
            return document;
        }

        /// <summary>
        /// Internal function used by renderers to bind this instance to it. 
        /// </summary>
        public void BindToRenderer(object renderer)
        {
            if (_renderer != null && renderer != null && !ReferenceEquals(_renderer, renderer))
            {
                throw new InvalidOperationException("The document is already bound to another renderer. " +
                  "A MigraDoc document can be rendered by only one renderer, because the rendering process " +
                  "modifies its internal structure. If you want to render a MigraDoc document on different renderers, " +
                  "you must create a copy of it using the Clone function.");
            }
            _renderer = renderer;
        }
        object _renderer;

        /// <summary>
        /// Indicates whether the document is bound to a renderer. A bound document must not be modified anymore.
        /// Modifying it leads to undefined results of the rendering process.
        /// </summary>
        public bool IsBoundToRenderer
        {
            get { return _renderer != null; }
        }

        /// <summary>
        /// Adds a new section to the document.
        /// </summary>
        public Section AddSection()
        {
            return Sections.AddSection();
        }

        /// <summary>
        /// Adds a new style to the document styles.
        /// </summary>
        /// <param name="name">Name of the style.</param>
        /// <param name="baseStyle">Name of the base style.</param>
        public Style AddStyle(string name, string baseStyle)
        {
            if (name == null || baseStyle == null)
                throw new ArgumentNullException(name == null ? "name" : "baseStyle");
            if (name == "" || baseStyle == "")
                throw new ArgumentException(name == "" ? "name" : "baseStyle");

            return Styles.AddStyle(name, baseStyle);
        }

        /// <summary>
        /// Adds a new section to the document.
        /// </summary>
        public void Add(Section section)
        {
            Sections.Add(section);
        }

        /// <summary>
        /// Adds a new style to the document styles.
        /// </summary>
        public void Add(Style style)
        {
            Styles.Add(style);
        }

        /// <summary>
        /// Adds an embedded file to the document.
        /// </summary>
        /// <param name="name">The name used to refer and to entitle the embedded file.</param>
        /// <param name="path">The path of the file to embed.</param>
        public void AddEmbeddedFile(string name, string path)
        {
            EmbeddedFiles.Add(name, path);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the last section of the document, or null, if the document has no sections.
        /// </summary>
        public Section LastSection
        {
            get
            {
                return (_sections != null && _sections.Count > 0) ?
                  _sections.LastObject as Section : null;
            }
        }

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

        /// <summary>
        /// Gets the document info.
        /// </summary>
        public DocumentInfo Info
        {
            get { return _info ?? (_info = new DocumentInfo(this)); }
            set
            {
                SetParent(value);
                _info = value;
            }
        }
        [DV]
        internal DocumentInfo _info;

        /// <summary>
        /// Gets or sets the styles of the document.
        /// </summary>
        public Styles Styles
        {
            get { return _styles ?? (_styles = new Styles(this)); }
            set
            {
                SetParent(value);
                _styles = value;
            }
        }
        [DV]
        internal Styles _styles;

        /// <summary>
        /// Gets or sets the default tab stop position.
        /// </summary>
        public Unit DefaultTabStop
        {
            get { return _defaultTabStop; }
            set { _defaultTabStop = value; }
        }
        [DV]
        internal Unit _defaultTabStop = Unit.NullValue;

        /// <summary>
        /// Gets the default page setup.
        /// </summary>
        public PageSetup DefaultPageSetup
        {
            get { return PageSetup.DefaultPageSetup; }
        }

        /// <summary>
        /// Gets or sets the location of the Footnote.
        /// </summary>
        public FootnoteLocation FootnoteLocation
        {
            get { return (FootnoteLocation)_footnoteLocation.Value; }
            set { _footnoteLocation.Value = (int)value; }
        }
        [DV(Type = typeof(FootnoteLocation))]
        internal NEnum _footnoteLocation = NEnum.NullValue(typeof(FootnoteLocation));

        /// <summary>
        /// Gets or sets the rule which is used to determine the footnote number on a new page.
        /// </summary>
        public FootnoteNumberingRule FootnoteNumberingRule
        {
            get { return (FootnoteNumberingRule)_footnoteNumberingRule.Value; }
            set { _footnoteNumberingRule.Value = (int)value; }
        }
        [DV(Type = typeof(FootnoteNumberingRule))]
        internal NEnum _footnoteNumberingRule = NEnum.NullValue(typeof(FootnoteNumberingRule));

        /// <summary>
        /// Gets or sets the type of number which is used for the footnote.
        /// </summary>
        public FootnoteNumberStyle FootnoteNumberStyle
        {
            get { return (FootnoteNumberStyle)_footnoteNumberStyle.Value; }
            set { _footnoteNumberStyle.Value = (int)value; }
        }
        [DV(Type = typeof(FootnoteNumberStyle))]
        internal NEnum _footnoteNumberStyle = NEnum.NullValue(typeof(FootnoteNumberStyle));

        /// <summary>
        /// Gets or sets the starting number of the footnote.
        /// </summary>
        public int FootnoteStartingNumber
        {
            get { return _footnoteStartingNumber.Value; }
            set { _footnoteStartingNumber.Value = value; }
        }
        [DV]
        internal NInt _footnoteStartingNumber = NInt.NullValue;

        /// <summary>
        /// Gets or sets the path for images used by the document.
        /// </summary>
        public string ImagePath
        {
            get { return _imagePath.Value; }
            set { _imagePath.Value = value; }
        }
        [DV]
        internal NString _imagePath = NString.NullValue;

        /// <summary>
        /// Gets or sets a value indicating whether to use the CMYK color model when rendered as PDF.
        /// </summary>
        public bool UseCmykColor
        {
            get { return _useCmykColor.Value; }
            set { _useCmykColor.Value = value; }
        }
        [DV]
        internal NBool _useCmykColor = NBool.NullValue;

        /// <summary>
        /// Gets the sections of the document.
        /// </summary>
        public Sections Sections
        {
            get { return _sections ?? (_sections = new Sections(this)); }
            set
            {
                SetParent(value);
                _sections = value;
            }
        }
        [DV]
        internal Sections _sections;

        /// <summary>
        /// Gets the embedded documents of the document.
        /// </summary>
        public EmbeddedFiles EmbeddedFiles
        {
            get { return _embeddedFiles ?? (_embeddedFiles = new EmbeddedFiles()); }
            set
            {
                SetParent(value);
                _embeddedFiles = value;
            }
        }
        [DV]
        internal EmbeddedFiles _embeddedFiles;
        #endregion

        /// <summary>
        /// Gets the DDL file name.
        /// </summary>
        public string DdlFile
        {
            get { return _ddlFile; }
        }
        internal string _ddlFile = "";

        #region Internal
        /// <summary>
        /// Converts Document into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(_comment.Value);
            serializer.WriteLine("\\document");

            int pos = serializer.BeginAttributes();
            if (!IsNull("Info"))
                Info.Serialize(serializer);
            if (!_defaultTabStop.IsNull)
                serializer.WriteSimpleAttribute("DefaultTabStop", DefaultTabStop);
            if (!_footnoteLocation.IsNull)
                serializer.WriteSimpleAttribute("FootnoteLocation", FootnoteLocation);
            if (!_footnoteNumberingRule.IsNull)
                serializer.WriteSimpleAttribute("FootnoteNumberingRule", FootnoteNumberingRule);
            if (!_footnoteNumberStyle.IsNull)
                serializer.WriteSimpleAttribute("FootnoteNumberStyle", FootnoteNumberStyle);
            if (!_footnoteStartingNumber.IsNull)
                serializer.WriteSimpleAttribute("FootnoteStartingNumber", FootnoteStartingNumber);
            if (!_imagePath.IsNull)
                serializer.WriteSimpleAttribute("ImagePath", ImagePath);
            if (!_useCmykColor.IsNull)
                serializer.WriteSimpleAttribute("UseCmykColor", UseCmykColor);
            serializer.EndAttributes(pos);

            serializer.BeginContent();
            if (!IsNull("EmbeddedFiles"))
                EmbeddedFiles.Serialize(serializer);

            Styles.Serialize(serializer);

            if (!IsNull("Sections"))
                Sections.Serialize(serializer);
            serializer.EndContent();
            serializer.Flush();
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and all its child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitDocument(this);
            if (visitChildren)
            {
                ((IVisitable)Styles).AcceptVisitor(visitor, true);
                ((IVisitable)Sections).AcceptVisitor(visitor, true);
            }
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Document))); }
        }
        static Meta _meta;
        #endregion
    }
}
