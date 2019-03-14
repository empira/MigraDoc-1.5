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
    /// Represents a table in a document.
    /// </summary>
    public class Table : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Table class.
        /// </summary>
        public Table()
        { }

        /// <summary>
        /// Initializes a new instance of the Table class with the specified parent.
        /// </summary>
        internal Table(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Table Clone()
        {
            return (Table)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Table table = (Table)base.DeepCopy();
            if (table._columns != null)
            {
                table._columns = table._columns.Clone();
                table._columns._parent = table;
            }
            if (table._rows != null)
            {
                table._rows = table._rows.Clone();
                table._rows._parent = table;
            }
            if (table._format != null)
            {
                table._format = table._format.Clone();
                table._format._parent = table;
            }
            if (table._borders != null)
            {
                table._borders = table._borders.Clone();
                table._borders._parent = table;
            }
            if (table._shading != null)
            {
                table._shading = table._shading.Clone();
                table._shading._parent = table;
            }
            return table;
        }

        /// <summary>
        /// Adds a new column to the table. Allowed only before any row was added.
        /// </summary>
        public Column AddColumn()
        {
            return Columns.AddColumn();
        }

        /// <summary>
        /// Adds a new column of the specified width to the table. Allowed only before any row was added.
        /// </summary>
        public Column AddColumn(Unit width)
        {
            Column clm = Columns.AddColumn();
            clm.Width = width;
            return clm;
        }

        /// <summary>
        /// Adds a new row to the table. Allowed only if at least one column was added.
        /// </summary>
        public Row AddRow()
        {
            return Rows.AddRow();
        }

        /// <summary>
        /// Returns true if no cell exists in the table.
        /// </summary>
        public bool IsEmpty
        {
            get { return Rows.Count == 0 || Columns.Count == 0; }
        }

        /// <summary>
        /// Sets a shading of the specified Color in the specified Tablerange.
        /// </summary>
        public void SetShading(int clm, int row, int clms, int rows, Color clr)
        {
            int rowsCount = _rows.Count;
            int clmsCount = _columns.Count;

            if (row < 0 || row >= rowsCount)
                throw new ArgumentOutOfRangeException("row", "Invalid row index.");

            if (clm < 0 || clm >= clmsCount)
                throw new ArgumentOutOfRangeException("clm", "Invalid column index.");

            if (rows <= 0 || row + rows > rowsCount)
                throw new ArgumentOutOfRangeException("rows", "Invalid row count.");

            if (clms <= 0 || clm + clms > clmsCount)
                throw new ArgumentOutOfRangeException("clms", "Invalid column count.");

            int maxRow = row + rows - 1;
            int maxClm = clm + clms - 1;
            for (int r = row; r <= maxRow; r++)
            {
                Row currentRow = _rows[r];
                for (int c = clm; c <= maxClm; c++)
                    currentRow[c].Shading.Color = clr;
            }
        }

        /// <summary>
        /// Sets the borders surrounding the specified range of the table.
        /// </summary>
        public void SetEdge(int clm, int row, int clms, int rows,
          Edge edge, BorderStyle style, Unit width, Color clr)
        {
            Border border;
            int maxRow = row + rows - 1;
            int maxClm = clm + clms - 1;
            for (int r = row; r <= maxRow; r++)
            {
                Row currentRow = _rows[r];
                for (int c = clm; c <= maxClm; c++)
                {
                    Cell currentCell = currentRow[c];
                    if ((edge & Edge.Top) == Edge.Top && r == row)
                    {
                        border = currentCell.Borders.Top;
                        border.Style = style;
                        border.Width = width;
                        if (clr != Color.Empty)
                            border.Color = clr;
                    }
                    if ((edge & Edge.Left) == Edge.Left && c == clm)
                    {
                        border = currentCell.Borders.Left;
                        border.Style = style;
                        border.Width = width;
                        if (clr != Color.Empty)
                            border.Color = clr;
                    }
                    if ((edge & Edge.Bottom) == Edge.Bottom && r == maxRow)
                    {
                        border = currentCell.Borders.Bottom;
                        border.Style = style;
                        border.Width = width;
                        if (clr != Color.Empty)
                            border.Color = clr;
                    }
                    if ((edge & Edge.Right) == Edge.Right && c == maxClm)
                    {
                        border = currentCell.Borders.Right;
                        border.Style = style;
                        border.Width = width;
                        if (clr != Color.Empty)
                            border.Color = clr;
                    }
                    if ((edge & Edge.Horizontal) == Edge.Horizontal && r < maxRow)
                    {
                        border = currentCell.Borders.Bottom;
                        border.Style = style;
                        border.Width = width;
                        if (clr != Color.Empty)
                            border.Color = clr;
                    }
                    if ((edge & Edge.Vertical) == Edge.Vertical && c < maxClm)
                    {
                        border = currentCell.Borders.Right;
                        border.Style = style;
                        border.Width = width;
                        if (clr != Color.Empty)
                            border.Color = clr;
                    }
                    if ((edge & Edge.DiagonalDown) == Edge.DiagonalDown)
                    {
                        border = currentCell.Borders.DiagonalDown;
                        border.Style = style;
                        border.Width = width;
                        if (clr != Color.Empty)
                            border.Color = clr;
                    }
                    if ((edge & Edge.DiagonalUp) == Edge.DiagonalUp)
                    {
                        border = currentCell.Borders.DiagonalUp;
                        border.Style = style;
                        border.Width = width;
                        if (clr != Color.Empty)
                            border.Color = clr;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the borders surrounding the specified range of the table.
        /// </summary>
        public void SetEdge(int clm, int row, int clms, int rows, Edge edge, BorderStyle style, Unit width)
        {
            SetEdge(clm, row, clms, rows, edge, style, width, Color.Empty);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Columns collection of the table.
        /// </summary>
        public Columns Columns
        {
            get { return _columns ?? (_columns = new Columns(this)); }
            set
            {
                SetParent(value);
                _columns = value;
            }
        }
        [DV]
        internal Columns _columns;

        /// <summary>
        /// Gets the Rows collection of the table.
        /// </summary>
        public Rows Rows
        {
            get { return _rows ?? (_rows = new Rows(this)); }
            set
            {
                SetParent(value);
                _rows = value;
            }
        }
        [DV]
        internal Rows _rows;

        /// <summary>
        /// Sets or gets the default style name for all rows and columns of the table.
        /// </summary>
        public string Style
        {
            get { return _style.Value; }
            set { _style.Value = value; }
        }
        [DV]
        internal NString _style = NString.NullValue;

        /// <summary>
        /// Gets the default ParagraphFormat for all rows and columns of the table.
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
        /// Gets or sets the default top padding for all cells of the table.
        /// </summary>
        public Unit TopPadding
        {
            get { return _topPadding; }
            set { _topPadding = value; }
        }
        [DV]
        internal Unit _topPadding = Unit.NullValue;

        /// <summary>
        /// Gets or sets the default bottom padding for all cells of the table.
        /// </summary>
        public Unit BottomPadding
        {
            get { return _bottomPadding; }
            set { _bottomPadding = value; }
        }
        [DV]
        internal Unit _bottomPadding = Unit.NullValue;

        /// <summary>
        /// Gets or sets the default left padding for all cells of the table.
        /// </summary>
        public Unit LeftPadding
        {
            get { return _leftPadding; }
            set { _leftPadding = value; }
        }
        [DV]
        internal Unit _leftPadding = Unit.NullValue;

        /// <summary>
        /// Gets or sets the default right padding for all cells of the table.
        /// </summary>
        public Unit RightPadding
        {
            get { return _rightPadding; }
            set { _rightPadding = value; }
        }
        [DV]
        internal Unit _rightPadding = Unit.NullValue;

        /// <summary>
        /// Gets the default Borders object for all cells of the column.
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
        /// Gets the default Shading object for all cells of the column.
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
        /// Gets or sets a value indicating whether
        /// to keep all the table rows on the same page.
        /// </summary>
        public bool KeepTogether
        {
            get { return _keepTogether.Value; }
            set { _keepTogether.Value = value; }
        }
        [DV]
        internal NBool _keepTogether = NBool.NullValue;

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
        /// Converts Table into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(_comment.Value);

            serializer.WriteLine("\\table");

            int pos = serializer.BeginAttributes();

            if (_style.Value != String.Empty)
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

            if (!IsNull("Borders"))
                _borders.Serialize(serializer, null);

            if (!IsNull("Shading"))
                _shading.Serialize(serializer);

            serializer.EndAttributes(pos);

            serializer.BeginContent();
            Columns.Serialize(serializer);
            Rows.Serialize(serializer);
            serializer.EndContent();
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and its child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitTable(this);

            ((IVisitable)_columns).AcceptVisitor(visitor, visitChildren);
            ((IVisitable)_rows).AcceptVisitor(visitor, visitChildren);
        }

        /// <summary>
        /// Gets the cell with the given row and column indices.
        /// </summary>
        public Cell this[int rwIdx, int clmIdx]
        {
            get { return Rows[rwIdx].Cells[clmIdx]; }
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Table))); }
        }
        static Meta _meta;
        #endregion
    }
}
