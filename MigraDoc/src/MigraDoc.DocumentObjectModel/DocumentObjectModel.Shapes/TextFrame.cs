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
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using MigraDoc.DocumentObjectModel.Tables;

namespace MigraDoc.DocumentObjectModel.Shapes
{
    /// <summary>
    /// Represents a text frame that can be freely placed.
    /// </summary>
    public class TextFrame : Shape, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the TextFrame class.
        /// </summary>
        public TextFrame()
        { }

        /// <summary>
        /// Initializes a new instance of the TextFrame class with the specified parent.
        /// </summary>
        internal TextFrame(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new TextFrame Clone()
        {
            return (TextFrame)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            TextFrame textFrame = (TextFrame)base.DeepCopy();
            if (textFrame._elements != null)
            {
                textFrame._elements = textFrame._elements.Clone();
                textFrame._elements._parent = textFrame;
            }
            return textFrame;
        }

        /// <summary>
        /// Adds a new paragraph to the text frame.
        /// </summary>
        public Paragraph AddParagraph()
        {
            return Elements.AddParagraph();
        }

        /// <summary>
        /// Adds a new paragraph with the specified text to the text frame.
        /// </summary>
        public Paragraph AddParagraph(string _paragraphText)
        {
            return Elements.AddParagraph(_paragraphText);
        }

        /// <summary>
        /// Adds a new chart with the specified type to the text frame.
        /// </summary>
        public Chart AddChart(ChartType _type)
        {
            return Elements.AddChart(_type);
        }

        /// <summary>
        /// Adds a new chart to the text frame.
        /// </summary>
        public Chart AddChart()
        {
            return Elements.AddChart();
        }

        /// <summary>
        /// Adds a new table to the text frame.
        /// </summary>
        public Table AddTable()
        {
            return Elements.AddTable();
        }

        /// <summary>
        /// Adds a new Image to the text frame.
        /// </summary>
        public Image AddImage(string _fileName)
        {
            return Elements.AddImage(_fileName);
        }

        /// <summary>
        /// Adds a new paragraph to the text frame.
        /// </summary>
        public void Add(Paragraph paragraph)
        {
            Elements.Add(paragraph);
        }

        /// <summary>
        /// Adds a new chart to the text frame.
        /// </summary>
        public void Add(Chart chart)
        {
            Elements.Add(chart);
        }

        /// <summary>
        /// Adds a new table to the text frame.
        /// </summary>
        public void Add(Table table)
        {
            Elements.Add(table);
        }

        /// <summary>
        /// Adds a new image to the text frame.
        /// </summary>
        public void Add(Image image)
        {
            Elements.Add(image);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Margin between the textframes content and its left edge.
        /// </summary>
        public Unit MarginLeft
        {
            get { return _marginLeft; }
            set { _marginLeft = value; }
        }
        [DV]
        internal Unit _marginLeft = Unit.NullValue;

        /// <summary>
        /// Gets or sets the Margin between the textframes content and its right edge.
        /// </summary>
        public Unit MarginRight
        {
            get { return _marginRight; }
            set { _marginRight = value; }
        }
        [DV]
        internal Unit _marginRight = Unit.NullValue;

        /// <summary>
        /// Gets or sets the Margin between the textframes content and its top edge.
        /// </summary>
        public Unit MarginTop
        {
            get { return _marginTop; }
            set { _marginTop = value; }
        }
        [DV]
        internal Unit _marginTop = Unit.NullValue;

        /// <summary>
        /// Gets or sets the Margin between the textframes content and its bottom edge.
        /// </summary>
        public Unit MarginBottom
        {
            get { return _marginBottom; }
            set { _marginBottom = value; }
        }
        [DV]
        internal Unit _marginBottom = Unit.NullValue;

        /// <summary>
        /// Sets all margins in one step with the same value.
        /// </summary>
        public Unit Margin
        {
            set
            {
                _marginLeft = value;
                _marginRight = value;
                _marginTop = value;
                _marginBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the text orientation for the texframe content.
        /// </summary>
        public TextOrientation Orientation
        {
            get { return (TextOrientation)_orientation.Value; }
            set { _orientation.Value = (int)value; }
        }
        [DV(Type = typeof(TextOrientation))]
        internal NEnum _orientation = NEnum.NullValue(typeof(TextOrientation));

        /// <summary>
        /// The document elements that build the textframe's content.
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
        private DocumentElements _elements;
        #endregion

        /// <summary>
        /// Allows the visitor object to visit the document object and its child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitTextFrame(this);

            if (visitChildren && _elements != null)
                ((IVisitable)_elements).AcceptVisitor(visitor, true);
        }

        #region Internal
        /// <summary>
        /// Converts TextFrame into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine("\\textframe");
            int pos = serializer.BeginAttributes();
            base.Serialize(serializer);
            if (!_marginLeft.IsNull)
                serializer.WriteSimpleAttribute("MarginLeft", MarginLeft);
            if (!_marginRight.IsNull)
                serializer.WriteSimpleAttribute("MarginRight", MarginRight);
            if (!_marginTop.IsNull)
                serializer.WriteSimpleAttribute("MarginTop", MarginTop);
            if (!_marginBottom.IsNull)
                serializer.WriteSimpleAttribute("MarginBottom", MarginBottom);
            if (!_orientation.IsNull)
                serializer.WriteSimpleAttribute("Orientation", Orientation);
            serializer.EndAttributes(pos);

            serializer.BeginContent();
            if (_elements != null)
                _elements.Serialize(serializer);
            serializer.EndContent();
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(TextFrame))); }
        }
        static Meta _meta;
        #endregion
    }
}