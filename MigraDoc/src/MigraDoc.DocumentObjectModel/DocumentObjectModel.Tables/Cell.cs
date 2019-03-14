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
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Shapes.Charts;

namespace MigraDoc.DocumentObjectModel.Tables
{
    /// <summary>
    /// Represents a cell of a table.
    /// </summary>
    public class Cell : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Cell class.
        /// </summary>
        public Cell()
        { }

        /// <summary>
        /// Initializes a new instance of the Cell class with the specified parent.
        /// </summary>
        internal Cell(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Cell Clone()
        {
            return (Cell)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Cell cell = (Cell)base.DeepCopy();
            // Remove all references to the original object hierarchy.
            cell.ResetCachedValues();
            // TODO Call ResetCachedValues() for all classes where this is needed!
            if (cell._format != null)
            {
                cell._format = cell._format.Clone();
                cell._format._parent = cell;
            }
            if (cell._borders != null)
            {
                cell._borders = cell._borders.Clone();
                cell._borders._parent = cell;
            }
            if (cell._shading != null)
            {
                cell._shading = cell._shading.Clone();
                cell._shading._parent = cell;
            }
            if (cell._elements != null)
            {
                cell._elements = cell._elements.Clone();
                cell._elements._parent = cell;
            }
            return cell;
        }

        /// <summary>
        /// Resets the cached values.
        /// </summary>
        internal override void ResetCachedValues()
        {
            base.ResetCachedValues();
            _row = null;
            _clm = null;
            _table = null;
        }

        /// <summary>
        /// Adds a new paragraph to the cell.
        /// </summary>
        public Paragraph AddParagraph()
        {
            return Elements.AddParagraph();
        }

        /// <summary>
        /// Adds a new paragraph with the specified text to the cell.
        /// </summary>
        public Paragraph AddParagraph(string paragraphText)
        {
            return Elements.AddParagraph(paragraphText);
        }

        /// <summary>
        /// Adds a new chart with the specified type to the cell.
        /// </summary>
        public Chart AddChart(ChartType type)
        {
            return Elements.AddChart(type);
        }

        /// <summary>
        /// Adds a new chart to the cell.
        /// </summary>
        public Chart AddChart()
        {
            return Elements.AddChart();
        }

        /// <summary>
        /// Adds a new Image to the cell.
        /// </summary>
        public Image AddImage(string fileName)
        {
            return Elements.AddImage(fileName);
        }

        /// <summary>
        /// Adds a new textframe to the cell.
        /// </summary>
        public TextFrame AddTextFrame()
        {
            return Elements.AddTextFrame();
        }

        /// <summary>
        /// Adds a new paragraph to the cell.
        /// </summary>
        public void Add(Paragraph paragraph)
        {
            Elements.Add(paragraph);
        }

        /// <summary>
        /// Adds a new chart to the cell.
        /// </summary>
        public void Add(Chart chart)
        {
            Elements.Add(chart);
        }

        /// <summary>
        /// Adds a new image to the cell.
        /// </summary>
        public void Add(Image image)
        {
            Elements.Add(image);
        }

        /// <summary>
        /// Adds a new text frame to the cell.
        /// </summary>
        public void Add(TextFrame textFrame)
        {
            Elements.Add(textFrame);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the table the cell belongs to.
        /// </summary>
        public Table Table
        {
            get
            {
                if (_table == null)
                {
                    Cells cls = Parent as Cells;
                    if (cls != null)
                        _table = cls.Table;
                }
                return _table;
            }
        }
        Table _table;

        /// <summary>
        /// Gets the column the cell belongs to.
        /// </summary>
        public Column Column
        {
            get
            {
                if (_clm == null)
                {
                    Cells cells = Parent as Cells;
                    for (int index = 0; index < cells.Count; ++index)
                    {
                        if (cells[index] == this)
                            _clm = Table.Columns[index];
                    }
                }
                return _clm;
            }
        }
        Column _clm;

        /// <summary>
        /// Gets the row the cell belongs to.
        /// </summary>
        public Row Row
        {
            get
            {
                if (_row == null)
                {
                    Cells cells = Parent as Cells;
                    _row = cells.Row;
                }
                return _row;
            }
        }
        Row _row;

        /// <summary>
        /// Sets or gets the style name.
        /// </summary>
        public string Style
        {
            get { return _style.Value; }
            set { _style.Value = value; }
        }
        [DV]
        internal NString _style = NString.NullValue;

        /// <summary>
        /// Gets the ParagraphFormat object of the paragraph.
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
        /// Gets or sets the vertical alignment of the cell.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)_verticalAlignment.Value; }
            set { _verticalAlignment.Value = (int)value; }
        }
        [DV(Type = typeof(VerticalAlignment))]
        internal NEnum _verticalAlignment = NEnum.NullValue(typeof(VerticalAlignment));

        /// <summary>
        /// Gets the Borders object.
        /// </summary>
        public Borders Borders
        {
            get
            {
                if (_borders == null)
                {
                    if (Document == null) // BUG CMYK
                        GetType();
                    _borders = new Borders(this);
                }
                return _borders;
            }
            set
            {
                SetParent(value);
                _borders = value;
            }
        }
        [DV]
        internal Borders _borders;

        /// <summary>
        /// Gets the shading object.
        /// </summary>
        public Shading Shading
        {
            get { return _shading ?? (_shading = new Shading(this)); }
            set
            {
                SetParent(value);
                _shading = value;
            }
        }
        [DV]
        internal Shading _shading;

        /// <summary>
        /// Specifies if the Cell should be rendered as a rounded corner.
        /// </summary>
        public RoundedCorner RoundedCorner
        {
            get { return _roundedCorner; }
            set
            {
                _roundedCorner = value;
            }
        }
        [DV]
        internal RoundedCorner _roundedCorner;

        /// <summary>
        /// Gets or sets the number of cells to be merged right.
        /// </summary>
        public int MergeRight
        {
            get { return _mergeRight.Value; }
            set { _mergeRight.Value = value; }
        }
        [DV]
        internal NInt _mergeRight = NInt.NullValue;

        /// <summary>
        /// Gets or sets the number of cells to be merged down.
        /// </summary>
        public int MergeDown
        {
            get { return _mergeDown.Value; }
            set { _mergeDown.Value = value; }
        }
        [DV]
        internal NInt _mergeDown = NInt.NullValue;

        /// <summary>
        /// Gets the collection of document objects that defines the cell.
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
        /// Converts Cell into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(_comment.Value);
            serializer.WriteLine("\\cell");

            int pos = serializer.BeginAttributes();

            if (_style.Value != String.Empty)
                serializer.WriteSimpleAttribute("Style", Style);

            if (!IsNull("Format"))
                _format.Serialize(serializer, "Format", null);

            if (!_mergeDown.IsNull)
                serializer.WriteSimpleAttribute("MergeDown", MergeDown);

            if (!_mergeRight.IsNull)
                serializer.WriteSimpleAttribute("MergeRight", MergeRight);

            if (!_verticalAlignment.IsNull)
                serializer.WriteSimpleAttribute("VerticalAlignment", VerticalAlignment);

            if (!IsNull("Borders"))
                _borders.Serialize(serializer, null);

            if (!IsNull("Shading"))
                _shading.Serialize(serializer);

            if (_roundedCorner != RoundedCorner.None)
                serializer.WriteSimpleAttribute("RoundedCorner", RoundedCorner);

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
            visitor.VisitCell(this);

            if (visitChildren && _elements != null)
                ((IVisitable)_elements).AcceptVisitor(visitor, visitChildren);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Cell))); }
        }
        static Meta _meta;
        #endregion
    }
}