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

using MigraDoc.DocumentObjectModel.Internals;
using MigraDoc.DocumentObjectModel.Visitors;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Shapes;

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// Represents a footnote in a paragraph.
    /// </summary>
    public class Footnote : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Footnote class.
        /// </summary>
        public Footnote()
        {
            //NYI: Nested footnote check!
        }

        /// <summary>
        /// Initializes a new instance of the Footnote class with the specified parent.
        /// </summary>
        internal Footnote(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Initializes a new instance of the Footnote class with a text the Footnote shall content.
        /// </summary>
        internal Footnote(string content)
            : this()
        {
            Elements.AddParagraph(content);
        }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Footnote Clone()
        {
            return (Footnote)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Footnote footnote = (Footnote)base.DeepCopy();
            if (footnote._elements != null)
            {
                footnote._elements = footnote._elements.Clone();
                footnote._elements._parent = footnote;
            }
            if (footnote._format != null)
            {
                footnote._format = footnote._format.Clone();
                footnote._format._parent = footnote;
            }
            return footnote;
        }

        /// <summary>
        /// Adds a new paragraph to the footnote.
        /// </summary>
        public Paragraph AddParagraph()
        {
            return Elements.AddParagraph();
        }

        /// <summary>
        /// Adds a new paragraph with the specified text to the footnote.
        /// </summary>
        public Paragraph AddParagraph(string text)
        {
            return Elements.AddParagraph(text);
        }

        /// <summary>
        /// Adds a new table to the footnote.
        /// </summary>
        public Table AddTable()
        {
            return Elements.AddTable();
        }

        /// <summary>
        /// Adds a new image to the footnote.
        /// </summary>
        public Image AddImage(string name)
        {
            return Elements.AddImage(name);
        }

        /// <summary>
        /// Adds a new paragraph to the footnote.
        /// </summary>
        public void Add(Paragraph paragraph)
        {
            Elements.Add(paragraph);
        }

        /// <summary>
        /// Adds a new table to the footnote.
        /// </summary>
        public void Add(Table table)
        {
            Elements.Add(table);
        }

        /// <summary>
        /// Adds a new image to the footnote.
        /// </summary>
        public void Add(Image image)
        {
            Elements.Add(image);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the collection of paragraph elements that defines the footnote.
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
        /// Gets or sets the character to be used to mark the footnote.
        /// </summary>
        public string Reference
        {
            get { return _reference.Value; }
            set { _reference.Value = value; }
        }
        [DV]
        internal NString _reference = NString.NullValue;

        /// <summary>
        /// Gets or sets the style name of the footnote.
        /// </summary>
        public string Style
        {
            get { return _style.Value; }
            set { _style.Value = value; }
        }
        [DV]
        internal NString _style = NString.NullValue;

        /// <summary>
        /// Gets the format of the footnote.
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
        #endregion

        #region Internal
        /// <summary>
        /// Converts Footnote into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine("\\footnote");

            int pos = serializer.BeginAttributes();
            if (_reference.Value != string.Empty)
                serializer.WriteSimpleAttribute("Reference", Reference);
            if (_style.Value != string.Empty)
                serializer.WriteSimpleAttribute("Style", Style);

            if (!IsNull("Format"))
                _format.Serialize(serializer, "Format", null);

            serializer.EndAttributes(pos);

            pos = serializer.BeginContent();
            if (!IsNull("Elements"))
                _elements.Serialize(serializer);
            serializer.EndContent(pos);
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and its child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitFootnote(this);

            if (visitChildren && _elements != null)
                ((IVisitable)_elements).AcceptVisitor(visitor, true);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Footnote))); }
        }
        static Meta _meta;
        #endregion
    }
}
