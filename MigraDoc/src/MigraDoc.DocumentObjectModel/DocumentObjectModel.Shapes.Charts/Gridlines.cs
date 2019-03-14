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

namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    /// <summary>
    /// Represents the gridlines on the axes.
    /// </summary>
    public class Gridlines : ChartObject
    {
        /// <summary>
        /// Initializes a new instance of the Gridlines class.
        /// </summary>
        public Gridlines()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Gridlines class with the specified parent.
        /// </summary>
        internal Gridlines(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Gridlines Clone()
        {
            return (Gridlines)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Gridlines gridlines = (Gridlines)base.DeepCopy();
            if (gridlines._lineFormat != null)
            {
                gridlines._lineFormat = gridlines._lineFormat.Clone();
                gridlines._lineFormat._parent = gridlines;
            }
            return gridlines;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the line format of the grid.
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
        #endregion

        #region Internal
        /// <summary>
        /// Converts Gridlines into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            Axis axisObject = _parent as Axis;

            int pos = serializer.BeginContent(axisObject.CheckGridlines(this));

            if (!IsNull("LineFormat"))
                _lineFormat.Serialize(serializer);

            serializer.EndContent();
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Gridlines))); }
        }
        static Meta _meta;
        #endregion
    }
}
