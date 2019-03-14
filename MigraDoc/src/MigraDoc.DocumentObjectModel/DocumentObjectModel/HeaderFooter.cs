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
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Shapes;

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// Represents a header or footer object in a section.
    /// </summary>
    public class HeaderFooter : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the HeaderFooter class.
        /// </summary>
        public HeaderFooter()
        { }

        /// <summary>
        /// Initializes a new instance of the HeaderFooter class with the specified parent.
        /// </summary>
        internal HeaderFooter(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new HeaderFooter Clone()
        {
            return (HeaderFooter)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            HeaderFooter headerFooter = (HeaderFooter)base.DeepCopy();
            if (headerFooter._format != null)
            {
                headerFooter._format = headerFooter._format.Clone();
                headerFooter._format._parent = headerFooter;
            }
            if (headerFooter._elements != null)
            {
                headerFooter._elements = headerFooter._elements.Clone();
                headerFooter._elements._parent = headerFooter;
            }
            return headerFooter;
        }

        /// <summary>
        /// Adds a new paragraph to the header or footer.
        /// </summary>
        public Paragraph AddParagraph()
        {
            return Elements.AddParagraph();
        }

        /// <summary>
        /// Adds a new paragraph with the specified text to the header or footer.
        /// </summary>
        public Paragraph AddParagraph(string paragraphText)
        {
            return Elements.AddParagraph(paragraphText);
        }

        /// <summary>
        /// Adds a new chart with the specified type to the header or footer.
        /// </summary>
        public Chart AddChart(ChartType type)
        {
            return Elements.AddChart(type);
        }

        /// <summary>
        /// Adds a new chart to the header or footer.
        /// </summary>
        public Chart AddChart()
        {
            return Elements.AddChart();
        }

        /// <summary>
        /// Adds a new table to the header or footer.
        /// </summary>
        public Table AddTable()
        {
            return Elements.AddTable();
        }

        /// <summary>
        /// Adds a new Image to the header or footer.
        /// </summary>
        public Image AddImage(string fileName)
        {
            return Elements.AddImage(fileName);
        }

        /// <summary>
        /// Adds a new textframe to the header or footer.
        /// </summary>
        public TextFrame AddTextFrame()
        {
            return Elements.AddTextFrame();
        }

        /// <summary>
        /// Adds a new paragraph to the header or footer.
        /// </summary>
        public void Add(Paragraph paragraph)
        {
            Elements.Add(paragraph);
        }

        /// <summary>
        /// Adds a new chart to the header or footer.
        /// </summary>
        public void Add(Chart chart)
        {
            Elements.Add(chart);
        }

        /// <summary>
        /// Adds a new table to the header or footer.
        /// </summary>
        public void Add(Table table)
        {
            Elements.Add(table);
        }

        /// <summary>
        /// Adds a new image to the header or footer.
        /// </summary>
        public void Add(Image image)
        {
            Elements.Add(image);
        }

        /// <summary>
        /// Adds a new text frame to the header or footer.
        /// </summary>
        public void Add(TextFrame textFrame)
        {
            Elements.Add(textFrame);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns true if this is a headers, false otherwise.
        /// </summary>
        public bool IsHeader
        {
            get { return ((HeadersFooters)_parent).IsHeader; }
        }

        /// <summary>
        /// Returns true if this is a footer, false otherwise.
        /// </summary>
        public bool IsFooter
        {
            get { return ((HeadersFooters)_parent).IsFooter; }
        }

        /// <summary>
        /// Returns true if this is a first page header or footer, false otherwise.
        /// </summary>
        public bool IsFirstPage
        {
            get { return ((HeadersFooters)_parent)._firstPage == this; }
        }

        /// <summary>
        /// Returns true if this is an even page header or footer, false otherwise.
        /// </summary>
        public bool IsEvenPage
        {
            get { return ((HeadersFooters)_parent)._evenPage == this; }
        }

        /// <summary>
        /// Returns true if this is a primary header or footer, false otherwise.
        /// </summary>
        public bool IsPrimary
        {
            get { return ((HeadersFooters)_parent)._primary == this; }
        }

        /// <summary>
        /// Gets or sets the style name.
        /// </summary>
        public string Style
        {
            get { return _style.Value; }
            set
            {
                // Just save style name. 
                Style style = Document.Styles[value];
                if (style != null)
                    _style.Value = value;
                else
                    throw new ArgumentException("Invalid style name '" + value + "'.");
            }
        }
        [DV]
        internal NString _style = NString.NullValue;

        /// <summary>
        /// Gets or sets the paragraph format.
        /// </summary>
        public ParagraphFormat Format
        {
            get { return _format ?? (_format = new ParagraphFormat(this)); }
            set
            {
                SetParent(value);
                _format = value;
            }
        }
        [DV]
        internal ParagraphFormat _format;

        /// <summary>
        /// Gets the collection of document objects that defines the header or footer.
        /// </summary>
        public DocumentElements Elements
        {
            get { return _elements ?? (_elements = new DocumentElements(this)); }
            set
            {
                SetParent(value);
                _elements = value;
            }
        }
        [DV(ItemType = typeof(DocumentObject))]
        internal DocumentElements _elements;

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

        #region Internal
        /// <summary>
        /// Converts HeaderFooter into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            HeadersFooters headersfooters = (HeadersFooters)_parent;
            if (headersfooters.Primary == this)
                Serialize(serializer, "primary");
            else if (headersfooters.EvenPage == this)
                Serialize(serializer, "evenpage");
            else if (headersfooters.FirstPage == this)
                Serialize(serializer, "firstpage");
        }

        /// <summary>
        /// Converts HeaderFooter into DDL.
        /// </summary>
        internal void Serialize(Serializer serializer, string prefix)
        {
            serializer.WriteComment(_comment.Value);
            serializer.WriteLine("\\" + prefix + (IsHeader ? "header" : "footer"));

            int pos = serializer.BeginAttributes();
            if (!IsNull("Format"))
                _format.Serialize(serializer, "Format", null);
            serializer.EndAttributes(pos);

            serializer.BeginContent();
            if (!IsNull("Elements"))
                _elements.Serialize(serializer);
            serializer.EndContent();
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and its child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitHeaderFooter(this);

            if (visitChildren && _elements != null)
                ((IVisitable)_elements).AcceptVisitor(visitor, true);
        }

        /// <summary>
        /// Determines whether this instance is null (not set).
        /// </summary>
        public override bool IsNull()
        {
            return false;
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(HeaderFooter))); }
        }
        static Meta _meta;
        #endregion
    }
}
