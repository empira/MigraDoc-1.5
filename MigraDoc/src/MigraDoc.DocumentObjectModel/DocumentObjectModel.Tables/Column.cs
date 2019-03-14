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

namespace MigraDoc.DocumentObjectModel.Tables
{
    /// <summary>
    /// Represents a column of a table.
    /// </summary>
    public class Column : DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the Column class.
        /// </summary>
        public Column()
        { }

        /// <summary>
        /// Initializes a new instance of the Column class with the specified parent.
        /// </summary>
        internal Column(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Column Clone()
        {
            return (Column)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Column column = (Column)base.DeepCopy();
            column.ResetCachedValues();
            if (column._format != null)
            {
                column._format = column._format.Clone();
                column._format._parent = column;
            }
            if (column._borders != null)
            {
                column._borders = column._borders.Clone();
                column._borders._parent = column;
            }
            if (column._shading != null)
            {
                column._shading = column._shading.Clone();
                column._shading._parent = column;
            }
            return column;
        }

        /// <summary>
        /// Resets the cached values.
        /// </summary>
        internal override void ResetCachedValues()
        {
            base.ResetCachedValues();
            _table = null;
            _index = NInt.NullValue;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the table the Column belongs to.
        /// </summary>
        public Table Table
        {
            get
            {
                if (_table == null)
                {
                    Columns clms = Parent as Columns;
                    if (clms != null)
                        _table = clms.Parent as Table;
                }
                return _table;
            }
        }
        Table _table;

        /// <summary>
        /// Gets the index of the column. First column has index 0.
        /// </summary>
        public int Index
        {
            get
            {
                if (_index == NInt.NullValue /*IsNull("Index")*/)
                {
                    Columns clms = (Columns)Parent;
                    // One for all and all for one.
                    for (int i = 0; i < clms.Count; ++i)
                    {
                        clms[i]._index = i;
                    }
                }
                return _index;
            }
        }
        [DV]
        internal NInt _index = NInt.NullValue;

        /// <summary>
        /// Gets a cell by its row index. The first cell has index 0.
        /// </summary>
        public Cell this[int index]
        {
            get
            {
                //Check.ArgumentOutOfRange(index >= 0 && index < table.Rows.Count, "index");
                return Table.Rows[index][_index];
            }
        }

        /// <summary>
        /// Sets or gets the default style name for all cells of the column.
        /// </summary>
        public string Style
        {
            get { return _style.Value; }
            set { _style.Value = value; }
        }
        [DV]
        internal NString _style = NString.NullValue;

        /// <summary>
        /// Gets the default ParagraphFormat for all cells of the column.
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
        /// Gets or sets the width of a column.
        /// </summary>
        public Unit Width
        {
            get { return _width; }
            set { _width = value; }
        }
        [DV]
        internal Unit _width = Unit.NullValue;

        /// <summary>
        /// Gets or sets the default left padding for all cells of the column.
        /// </summary>
        public Unit LeftPadding
        {
            get { return _leftPadding; }
            set { _leftPadding = value; }
        }
        [DV]
        internal Unit _leftPadding = Unit.NullValue;

        /// <summary>
        /// Gets or sets the default right padding for all cells of the column.
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
        /// Gets or sets the number of columns that should be kept together with
        /// current column in case of a page break.
        /// </summary>
        public int KeepWith
        {
            get { return _keepWith.Value; }
            set { _keepWith.Value = value; }
        }
        [DV]
        internal NInt _keepWith = NInt.NullValue;

        /// <summary>
        /// Gets or sets a value which define whether the column is a header.
        /// </summary>
        public bool HeadingFormat
        {
            get { return _headingFormat.Value; }
            set { _headingFormat.Value = value; }
        }
        [DV]
        internal NBool _headingFormat = NBool.NullValue;

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
        /// Converts Column into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(_comment.Value);
            serializer.WriteLine("\\column");

            int pos = serializer.BeginAttributes();

            if (_style.Value != String.Empty)
                serializer.WriteSimpleAttribute("Style", Style);

            if (!IsNull("Format"))
                _format.Serialize(serializer, "Format", null);

            if (!_headingFormat.IsNull)
                serializer.WriteSimpleAttribute("HeadingFormat", HeadingFormat);

            if (!_leftPadding.IsNull)
                serializer.WriteSimpleAttribute("LeftPadding", LeftPadding);

            if (!_rightPadding.IsNull)
                serializer.WriteSimpleAttribute("RightPadding", RightPadding);

            if (!_width.IsNull)
                serializer.WriteSimpleAttribute("Width", Width);

            if (!_keepWith.IsNull)
                serializer.WriteSimpleAttribute("KeepWith", KeepWith);

            if (!IsNull("Borders"))
                _borders.Serialize(serializer, null);

            if (!IsNull("Shading"))
                _shading.Serialize(serializer);

            serializer.EndAttributes(pos);

            // columns has no content
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Column))); }
        }
        static Meta _meta;
        #endregion
    }
}
