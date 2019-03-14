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
    /// Represents the columns of a table.
    /// </summary>
    public class Columns : DocumentObjectCollection, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Columns class.
        /// </summary>
        public Columns()
        { }

        /// <summary>
        /// Initializes a new instance of the Columns class containing columns of the specified widths.
        /// </summary>
        public Columns(params Unit[] widths)
        {
            foreach (Unit width in widths)
            {
                Column clm = new Column();
                clm.Width = width;
                Add(clm);
            }
        }

        /// <summary>
        /// Initializes a new instance of the Columns class with the specified parent.
        /// </summary>
        internal Columns(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Columns Clone()
        {
            return (Columns)base.DeepCopy();
        }

        /// <summary>
        /// Adds a new column to the columns collection. Allowed only before any row was added.
        /// </summary>
        public Column AddColumn()
        {
            if (Table.Rows.Count > 0)
                throw new InvalidOperationException("Cannot add column because rows collection is not empty.");

            Column column = new Column();
            Add(column);
            return column;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the table the columns collection belongs to.
        /// </summary>
        public Table Table
        {
            get { return _parent as Table; }
        }

        /// <summary>
        /// Gets a column by its index.
        /// </summary>
        public new Column this[int index]
        {
            get { return base[index] as Column; }
        }

        /// <summary>
        /// Gets or sets the default width of all columns.
        /// </summary>
        public Unit Width
        {
            get { return _width; }
            set { _width = value; }
        }
        [DV]
        internal Unit _width = Unit.NullValue;

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
        /// Converts Columns into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(_comment.Value);
            serializer.WriteLine("\\columns");

            int pos = serializer.BeginAttributes();

            if (!_width.IsNull)
                serializer.WriteSimpleAttribute("Width", Width);

            serializer.EndAttributes(pos);

            serializer.BeginContent();
            int clms = Count;
            if (clms > 0)
            {
                for (int clm = 0; clm < clms; clm++)
                    this[clm].Serialize(serializer);
            }
            else
                serializer.WriteComment("Invalid - no columns defined. Table will not render.");
            serializer.EndContent();
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and its child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitColumns(this);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Columns))); }
        }
        static Meta _meta;
        #endregion
    }
}
