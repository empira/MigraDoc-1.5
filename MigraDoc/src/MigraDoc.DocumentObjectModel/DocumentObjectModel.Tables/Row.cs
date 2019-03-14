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

namespace MigraDoc.DocumentObjectModel.Tables
{
    /// <summary>
    /// Represents a row of a table.
    /// </summary>
    public class Row : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Row class.
        /// </summary>
        public Row()
        { }

        /// <summary>
        /// Initializes a new instance of the Row class with the specified parent.
        /// </summary>
        internal Row(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Row Clone()
        {
            return (Row)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Row row = (Row)base.DeepCopy();
            row.ResetCachedValues();
            if (row._format != null)
            {
                row._format = row._format.Clone();
                row._format._parent = row;
            }
            if (row._borders != null)
            {
                row._borders = row._borders.Clone();
                row._borders._parent = row;
            }
            if (row._shading != null)
            {
                row._shading = row._shading.Clone();
                row._shading._parent = row;
            }
            if (row._cells != null)
            {
                row._cells = row._cells.Clone();
                row._cells._parent = row;
            }
            return row;
        }

        /// <summary>
        /// Resets the cached values.
        /// </summary>
        internal override void ResetCachedValues()
        {
            base.ResetCachedValues();
            _table = null;
            index = NInt.NullValue;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the table the row belongs to.
        /// </summary>
        public Table Table
        {
            get
            {
                if (_table == null)
                {
                    Rows rws = Parent as Rows;
                    if (rws != null)
                        _table = rws.Table;
                }
                return _table;
            }
        }
        Table _table;

        /// <summary>
        /// Gets the index of the row. First row has index 0.
        /// </summary>
        public int Index
        {
            get
            {
                if (index == NInt.NullValue /*IsNull("index")*/)
                {
                    Rows rws = (Rows)_parent;
                    // One for all and all for one.
                    for (int i = 0; i < rws.Count; ++i)
                    {
                        rws[i].index = i;
                    }
                }
                return index;
            }
        }
        [DV]
        internal NInt index = NInt.NullValue;

        /// <summary>
        /// Gets a cell by its column index. The first cell has index 0.
        /// </summary>
        public Cell this[int index]
        {
            get { return Cells[index]; }
        }

        /// <summary>
        /// Gets or sets the default style name for all cells of the row.
        /// </summary>
        public string Style
        {
            get { return _style.Value; }
            set { _style.Value = value; }
        }
        [DV]
        internal NString _style = NString.NullValue;

        /// <summary>
        /// Gets the default ParagraphFormat for all cells of the row.
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
        /// Gets or sets the default vertical alignment for all cells of the row.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)_verticalAlignment.Value; }
            set { _verticalAlignment.Value = (int)value; }
        }
        [DV(Type = typeof(VerticalAlignment))]
        internal NEnum _verticalAlignment = NEnum.NullValue(typeof(VerticalAlignment));

        /// <summary>
        /// Gets or sets the height of the row.
        /// </summary>
        public Unit Height
        {
            get { return _height; }
            set { _height = value; }
        }
        [DV]
        internal Unit _height = Unit.NullValue;

        /// <summary>
        /// Gets or sets the rule which is used to determine the height of the row.
        /// </summary>
        public RowHeightRule HeightRule
        {
            get { return (RowHeightRule)_heightRule.Value; }
            set { _heightRule.Value = (int)value; }
        }
        [DV(Type = typeof(RowHeightRule))]
        internal NEnum _heightRule = NEnum.NullValue(typeof(RowHeightRule));

        /// <summary>
        /// Gets or sets the default value for all cells of the row.
        /// </summary>
        public Unit TopPadding
        {
            get { return _topPadding; }
            set { _topPadding = value; }
        }
        [DV]
        internal Unit _topPadding = Unit.NullValue;

        /// <summary>
        /// Gets or sets the default value for all cells of the row.
        /// </summary>
        public Unit BottomPadding
        {
            get { return _bottomPadding; }
            set { _bottomPadding = value; }
        }
        [DV]
        internal Unit _bottomPadding = Unit.NullValue;

        /// <summary>
        /// Gets or sets a value which define whether the row is a header.
        /// </summary>
        public bool HeadingFormat
        {
            get { return _headingFormat.Value; }
            set { _headingFormat.Value = value; }
        }
        [DV]
        internal NBool _headingFormat = NBool.NullValue;

        /// <summary>
        /// Gets the default Borders object for all cells of the row.
        /// </summary>
        public Borders Borders
        {
            get { return _borders ?? (_borders = new Borders(this)); }
            set
            {
                SetParent(value);
                _borders = value;
            }
        }
        [DV]
        internal Borders _borders;

        /// <summary>
        /// Gets the default Shading object for all cells of the row.
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
        /// Gets or sets the number of rows that should be
        /// kept together with the current row in case of a page break.
        /// </summary>
        public int KeepWith
        {
            get { return _keepWith.Value; }
            set { _keepWith.Value = value; }
        }
        [DV]
        internal NInt _keepWith = NInt.NullValue;

        /// <summary>
        /// Gets the Cells collection of the table.
        /// </summary>
        public Cells Cells
        {
            get { return _cells ?? (_cells = new Cells(this)); }
            set
            {
                SetParent(value);
                _cells = value;
            }
        }
        [DV]
        internal Cells _cells;

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
        /// Converts Row into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(_comment.Value);
            serializer.WriteLine("\\row");

            int pos = serializer.BeginAttributes();

            if (_style.Value != String.Empty)
                serializer.WriteSimpleAttribute("Style", Style);

            if (!IsNull("Format"))
                _format.Serialize(serializer, "Format", null);

            if (!_height.IsNull)
                serializer.WriteSimpleAttribute("Height", Height);

            if (!_heightRule.IsNull)
                serializer.WriteSimpleAttribute("HeightRule", HeightRule);

            if (!_topPadding.IsNull)
                serializer.WriteSimpleAttribute("TopPadding", TopPadding);

            if (!_bottomPadding.IsNull)
                serializer.WriteSimpleAttribute("BottomPadding", BottomPadding);

            if (!_headingFormat.IsNull)
                serializer.WriteSimpleAttribute("HeadingFormat", HeadingFormat);

            if (!_verticalAlignment.IsNull)
                serializer.WriteSimpleAttribute("VerticalAlignment", VerticalAlignment);

            if (!_keepWith.IsNull)
                serializer.WriteSimpleAttribute("KeepWith", KeepWith);

            //Borders & Shading
            if (!IsNull("Borders"))
                _borders.Serialize(serializer, null);

            if (!IsNull("Shading"))
                _shading.Serialize(serializer);

            serializer.EndAttributes(pos);

            serializer.BeginContent();
            if (!IsNull("Cells"))
                _cells.Serialize(serializer);
            serializer.EndContent();
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and its child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitRow(this);

            foreach (Cell cell in _cells)
                ((IVisitable)cell).AcceptVisitor(visitor, visitChildren);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Row))); }
        }
        static Meta _meta;
        #endregion
    }
}
