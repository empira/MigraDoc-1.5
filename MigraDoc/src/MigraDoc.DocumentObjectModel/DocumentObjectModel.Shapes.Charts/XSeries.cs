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
    /// Represents a series of data on the X-Axis.
    /// </summary>
    public class XSeries : ChartObject
    {
        /// <summary>
        /// Initializes a new instance of the XSeries class.
        /// </summary>
        public XSeries()
        {
            _xSeriesElements = new XSeriesElements();
        }

        /// <summary>
        /// The actual value container of the XSeries.
        /// </summary>
        [DV]
        protected internal XSeriesElements _xSeriesElements;

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new XSeries Clone()
        {
            return (XSeries)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            XSeries xSeries = (XSeries)base.DeepCopy();
            if (xSeries._xSeriesElements != null)
            {
                xSeries._xSeriesElements = xSeries._xSeriesElements.Clone();
                xSeries._xSeriesElements._parent = xSeries;
            }
            return xSeries;
        }

        /// <summary>
        /// Adds a blank to the XSeries.
        /// </summary>
        public void AddBlank()
        {
            _xSeriesElements.AddBlank();
        }

        /// <summary>
        /// Adds a value to the XSeries.
        /// </summary>
        public XValue Add(string value)
        {
            return _xSeriesElements.Add(value);
        }

        /// <summary>
        /// Adds an array of values to the XSeries.
        /// </summary>
        public void Add(params string[] values)
        {
            _xSeriesElements.Add(values);
        }
        #endregion

        #region Internal
        /// <summary>
        /// Converts XSeries into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine("\\xvalues");

            serializer.BeginContent();
            _xSeriesElements.Serialize(serializer);
            serializer.WriteLine("");
            serializer.EndContent();
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(XSeries))); }
        }
        static Meta _meta;
        #endregion
    }
}
