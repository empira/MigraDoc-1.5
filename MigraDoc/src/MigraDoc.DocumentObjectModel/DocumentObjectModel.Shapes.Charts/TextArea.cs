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
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Visitors;

namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    /// <summary>
    /// An area object in the chart which contain text or legend.
    /// </summary>
    public class TextArea : ChartObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the TextArea class.
        /// </summary>
        internal TextArea()
        { }

        /// <summary>
        /// Initializes a new instance of the TextArea class with the specified parent.
        /// </summary>
        internal TextArea(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new TextArea Clone()
        {
            return (TextArea)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            TextArea textArea = (TextArea)base.DeepCopy();
            if (textArea._format != null)
            {
                textArea._format = textArea._format.Clone();
                textArea._format._parent = textArea;
            }
            if (textArea._lineFormat != null)
            {
                textArea._lineFormat = textArea._lineFormat.Clone();
                textArea._lineFormat._parent = textArea;
            }
            if (textArea._fillFormat != null)
            {
                textArea._fillFormat = textArea._fillFormat.Clone();
                textArea._fillFormat._parent = textArea;
            }
            if (textArea._elements != null)
            {
                textArea._elements = textArea._elements.Clone();
                textArea._elements._parent = textArea;
            }
            return textArea;
        }

        /// <summary>
        /// Adds a new paragraph to the text area.
        /// </summary>
        public Paragraph AddParagraph()
        {
            return Elements.AddParagraph();
        }

        /// <summary>
        /// Adds a new paragraph with the specified text to the text area.
        /// </summary>
        public Paragraph AddParagraph(string paragraphText)
        {
            return Elements.AddParagraph(paragraphText);
        }

        /// <summary>
        /// Adds a new table to the text area.
        /// </summary>
        public Table AddTable()
        {
            return Elements.AddTable();
        }

        /// <summary>
        /// Adds a new Image to the text area.
        /// </summary>
        public Image AddImage(string fileName)
        {
            return Elements.AddImage(fileName);
        }

        /// <summary>
        /// Adds a new legend to the text area.
        /// </summary>
        public Legend AddLegend()
        {
            return Elements.AddLegend();
        }

        /// <summary>
        /// Adds a new paragraph to the text area.
        /// </summary>
        public void Add(Paragraph paragraph)
        {
            Elements.Add(paragraph);
        }

        /// <summary>
        /// Adds a new table to the text area.
        /// </summary>
        public void Add(Table table)
        {
            Elements.Add(table);
        }

        /// <summary>
        /// Adds a new image to the text area.
        /// </summary>
        public void Add(Image image)
        {
            Elements.Add(image);
        }

        /// <summary>
        /// Adds a new legend to the text area.
        /// </summary>
        public void Add(Legend legend)
        {
            Elements.Add(legend);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the height of the area.
        /// </summary>
        public Unit Height
        {
            get { return _height; }
            set { _height = value; }
        }
        [DV]
        internal Unit _height = Unit.NullValue;

        /// <summary>
        /// Gets or sets the width of the area.
        /// </summary>
        public Unit Width
        {
            get { return _width; }
            set { _width = value; }
        }
        [DV]
        internal Unit _width = Unit.NullValue;

        /// <summary>
        /// Gets or sets the default style name of the area.
        /// </summary>
        public string Style
        {
            get { return _style.Value; }
            set { _style.Value = value; }
        }
        [DV]
        internal NString _style = NString.NullValue;

        /// <summary>
        /// Gets or sets the default paragraph format of the area.
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
        /// Gets the line format of the area's border.
        /// </summary>
        public LineFormat LineFormat
        {
            get { return _lineFormat ?? (_lineFormat = new LineFormat(this)); }
            set
            {
                SetParent(value);
                _lineFormat = value;
            }
        }
        [DV]
        internal LineFormat _lineFormat;

        /// <summary>
        /// Gets the background filling of the area.
        /// </summary>
        public FillFormat FillFormat
        {
            get { return _fillFormat ?? (_fillFormat = new FillFormat(this)); }
            set
            {
                SetParent(value);
                _fillFormat = value;
            }
        }
        [DV]
        internal FillFormat _fillFormat;

        /// <summary>
        /// Gets or sets the left padding of the area.
        /// </summary>
        public Unit LeftPadding
        {
            get { return _leftPadding; }
            set { _leftPadding = value; }
        }
        [DV]
        internal Unit _leftPadding = Unit.NullValue;

        /// <summary>
        /// Gets or sets the right padding of the area.
        /// </summary>
        public Unit RightPadding
        {
            get { return _rightPadding; }
            set { _rightPadding = value; }
        }
        [DV]
        internal Unit _rightPadding = Unit.NullValue;

        /// <summary>
        /// Gets or sets the top padding of the area.
        /// </summary>
        public Unit TopPadding
        {
            get { return _topPadding; }
            set { _topPadding = value; }
        }
        [DV]
        internal Unit _topPadding = Unit.NullValue;

        /// <summary>
        /// Gets or sets the bottom padding of the area.
        /// </summary>
        public Unit BottomPadding
        {
            get { return _bottomPadding; }
            set { _bottomPadding = value; }
        }
        [DV]
        internal Unit _bottomPadding = Unit.NullValue;

        /// <summary>
        /// Gets or sets the Vertical alignment of the area.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)_verticalAlignment.Value; }
            set { _verticalAlignment.Value = (int)value; }
        }
        [DV(Type = typeof(VerticalAlignment))]
        internal NEnum _verticalAlignment = NEnum.NullValue(typeof(VerticalAlignment));

        /// <summary>
        /// Gets the document objects that creates the text area.
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
        #endregion

        #region Internal
        /// <summary>
        /// Converts TextArea into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            Chart chartObject = _parent as Chart;

            serializer.WriteLine("\\" + chartObject.CheckTextArea(this));
            int pos = serializer.BeginAttributes();

            if (!_style.IsNull)
                serializer.WriteSimpleAttribute("Style", Style);
            if (!IsNull("Format"))
                _format.Serialize(serializer, "Format", null);

            if (!_topPadding.IsNull)
                serializer.WriteSimpleAttribute("TopPadding", TopPadding);
            if (!_leftPadding.IsNull)
                serializer.WriteSimpleAttribute("LeftPadding", LeftPadding);
            if (!_rightPadding.IsNull)
                serializer.WriteSimpleAttribute("RightPadding", RightPadding);
            if (!_bottomPadding.IsNull)
                serializer.WriteSimpleAttribute("BottomPadding", BottomPadding);

            if (!_width.IsNull)
                serializer.WriteSimpleAttribute("Width", Width);
            if (!_height.IsNull)
                serializer.WriteSimpleAttribute("Height", Height);

            if (!_verticalAlignment.IsNull)
                serializer.WriteSimpleAttribute("VerticalAlignment", VerticalAlignment);

            if (!IsNull("LineFormat"))
                _lineFormat.Serialize(serializer);
            if (!IsNull("FillFormat"))
                _fillFormat.Serialize(serializer);

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
            get { return _meta ?? (_meta = new Meta(typeof(TextArea))); }
        }
        static Meta _meta;
        #endregion

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitTextArea(this);
            if (_elements != null && visitChildren)
                ((IVisitable)_elements).AcceptVisitor(visitor, visitChildren);
        }
    }
}
