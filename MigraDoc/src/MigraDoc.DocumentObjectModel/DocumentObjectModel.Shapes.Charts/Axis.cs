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
    /// This class represents an axis in a chart.
    /// </summary>
    public class Axis : ChartObject
    {
        /// <summary>
        /// Initializes a new instance of the Axis class.
        /// </summary>
        public Axis()
        { }

        /// <summary>
        /// Initializes a new instance of the Axis class with the specified parent.
        /// </summary>
        internal Axis(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Axis Clone()
        {
            return (Axis)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Axis axis = (Axis)base.DeepCopy();
            if (axis._title != null)
            {
                axis._title = axis._title.Clone();
                axis._title._parent = axis;
            }
            if (axis._tickLabels != null)
            {
                axis._tickLabels = axis._tickLabels.Clone();
                axis._tickLabels._parent = axis;
            }
            if (axis._lineFormat != null)
            {
                axis._lineFormat = axis._lineFormat.Clone();
                axis._lineFormat._parent = axis;
            }
            if (axis._majorGridlines != null)
            {
                axis._majorGridlines = axis._majorGridlines.Clone();
                axis._majorGridlines._parent = axis;
            }
            if (axis._minorGridlines != null)
            {
                axis._minorGridlines = axis._minorGridlines.Clone();
                axis._minorGridlines._parent = axis;
            }
            return axis;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the title of the axis.
        /// </summary>
        public AxisTitle Title
        {
            get { return _title ?? (_title = new AxisTitle(this)); }
            set
            {
                SetParent(value);
                _title = value;
            }
        }
        [DV]
        internal AxisTitle _title;

        /// <summary>
        /// Gets or sets the minimum value of the axis.
        /// </summary>
        public double MinimumScale
        {
            get { return _minimumScale.Value; }
            set { _minimumScale.Value = value; }
        }
        [DV]
        internal NDouble _minimumScale = NDouble.NullValue;

        /// <summary>
        /// Gets or sets the maximum value of the axis.
        /// </summary>
        public double MaximumScale
        {
            get { return _maximumScale.Value; }
            set { _maximumScale.Value = value; }
        }
        [DV]
        internal NDouble _maximumScale = NDouble.NullValue;

        /// <summary>
        /// Gets or sets the interval of the primary tick.
        /// </summary>
        public double MajorTick
        {
            get { return _majorTick.Value; }
            set { _majorTick.Value = value; }
        }
        [DV]
        internal NDouble _majorTick = NDouble.NullValue;

        /// <summary>
        /// Gets or sets the interval of the secondary tick.
        /// </summary>
        public double MinorTick
        {
            get { return _minorTick.Value; }
            set { _minorTick.Value = value; }
        }
        [DV]
        internal NDouble _minorTick = NDouble.NullValue;

        /// <summary>
        /// Gets or sets the type of the primary tick mark.
        /// </summary>
        public TickMarkType MajorTickMark
        {
            get { return (TickMarkType)_majorTickMark.Value; }
            set { _majorTickMark.Value = (int)value; }
        }
        [DV(Type = typeof(TickMarkType))]
        internal NEnum _majorTickMark = NEnum.NullValue(typeof(TickMarkType));

        /// <summary>
        /// Gets or sets the type of the secondary tick mark.
        /// </summary>
        public TickMarkType MinorTickMark
        {
            get { return (TickMarkType)_minorTickMark.Value; }
            set { _minorTickMark.Value = (int)value; }
        }
        [DV(Type = typeof(TickMarkType))]
        internal NEnum _minorTickMark = NEnum.NullValue(typeof(TickMarkType));

        /// <summary>
        /// Gets the label of the primary tick.
        /// </summary>
        public TickLabels TickLabels
        {
            get { return _tickLabels ?? (_tickLabels = new TickLabels(this)); }
            set
            {
                SetParent(value);
                _tickLabels = value;
            }
        }
        [DV]
        internal TickLabels _tickLabels;

        /// <summary>
        /// Gets the format of the axis line.
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
        /// Gets the primary gridline object.
        /// </summary>
        public Gridlines MajorGridlines
        {
            get { return _majorGridlines ?? (_majorGridlines = new Gridlines(this)); }
            set
            {
                SetParent(value);
                _majorGridlines = value;
            }
        }
        [DV]
        internal Gridlines _majorGridlines;

        /// <summary>
        /// Gets the secondary gridline object.
        /// </summary>
        public Gridlines MinorGridlines
        {
            get { return _minorGridlines ?? (_minorGridlines = new Gridlines(this)); }
            set
            {
                SetParent(value);
                _minorGridlines = value;
            }
        }
        [DV]
        internal Gridlines _minorGridlines;

        /// <summary>
        /// Gets or sets, whether the axis has a primary gridline object.
        /// </summary>
        public bool HasMajorGridlines
        {
            get { return _hasMajorGridlines.Value; }
            set { _hasMajorGridlines.Value = value; }
        }
        [DV]
        internal NBool _hasMajorGridlines = NBool.NullValue;

        /// <summary>
        /// Gets or sets, whether the axis has a secondary gridline object.
        /// </summary>
        public bool HasMinorGridlines
        {
            get { return _hasMinorGridlines.Value; }
            set { _hasMinorGridlines.Value = value; }
        }
        [DV]
        internal NBool _hasMinorGridlines = NBool.NullValue;
        #endregion

        /// <summary>
        /// Determines whether the specified gridlines object is a MajorGridlines or an MinorGridlines.
        /// </summary>
        internal string CheckGridlines(Gridlines gridlines)
        {
            if ((_majorGridlines != null) && (gridlines == _majorGridlines))
                return "MajorGridlines";
            if ((_minorGridlines != null) && (gridlines == _minorGridlines))
                return "MinorGridlines";

            return "";
        }

        #region Internal
        /// <summary>
        /// Converts Axis into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            Chart chartObject = _parent as Chart;

            serializer.WriteLine("\\" + chartObject.CheckAxis(this));
            int pos = serializer.BeginAttributes();

            if (!_minimumScale.IsNull)
                serializer.WriteSimpleAttribute("MinimumScale", MinimumScale);
            if (!_maximumScale.IsNull)
                serializer.WriteSimpleAttribute("MaximumScale", MaximumScale);
            if (!_majorTick.IsNull)
                serializer.WriteSimpleAttribute("MajorTick", MajorTick);
            if (!_minorTick.IsNull)
                serializer.WriteSimpleAttribute("MinorTick", MinorTick);
            if (!_hasMajorGridlines.IsNull)
                serializer.WriteSimpleAttribute("HasMajorGridLines", HasMajorGridlines);
            if (!_hasMinorGridlines.IsNull)
                serializer.WriteSimpleAttribute("HasMinorGridLines", HasMinorGridlines);
            if (!_majorTickMark.IsNull)
                serializer.WriteSimpleAttribute("MajorTickMark", MajorTickMark);
            if (!_minorTickMark.IsNull)
                serializer.WriteSimpleAttribute("MinorTickMark", MinorTickMark);

            if (!IsNull("Title"))
                _title.Serialize(serializer);

            if (!IsNull("LineFormat"))
                _lineFormat.Serialize(serializer);

            if (!IsNull("MajorGridlines"))
                _majorGridlines.Serialize(serializer);

            if (!IsNull("MinorGridlines"))
                _minorGridlines.Serialize(serializer);

            if (!IsNull("TickLabels"))
                _tickLabels.Serialize(serializer);

            serializer.EndAttributes(pos);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Axis))); }
        }
        static Meta _meta;
        #endregion
    }
}
