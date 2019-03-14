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

namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    /// <summary>
    /// Represents charts with different types.
    /// </summary>
    public class Chart : Shape, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Chart class.
        /// </summary>
        public Chart()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Chart class with the specified parent.
        /// </summary>
        internal Chart(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Initializes a new instance of the Chart class with the specified chart type.
        /// </summary>
        public Chart(ChartType type)
            : this()
        {
            Type = type;
        }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Chart Clone()
        {
            return (Chart)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Chart chart = (Chart)base.DeepCopy();
            if (chart._format != null)
            {
                chart._format = chart._format.Clone();
                chart._format._parent = chart;
            }
            if (chart._xAxis != null)
            {
                chart._xAxis = chart._xAxis.Clone();
                chart._xAxis._parent = chart;
            }
            if (chart._yAxis != null)
            {
                chart._yAxis = chart._yAxis.Clone();
                chart._yAxis._parent = chart;
            }
            if (chart._zAxis != null)
            {
                chart._zAxis = chart._zAxis.Clone();
                chart._zAxis._parent = chart;
            }
            if (chart._seriesCollection != null)
            {
                chart._seriesCollection = chart._seriesCollection.Clone();
                chart._seriesCollection._parent = chart;
            }
            if (chart._xValues != null)
            {
                chart._xValues = chart._xValues.Clone();
                chart._xValues._parent = chart;
            }
            if (chart._headerArea != null)
            {
                chart._headerArea = chart._headerArea.Clone();
                chart._headerArea._parent = chart;
            }
            if (chart._bottomArea != null)
            {
                chart._bottomArea = chart._bottomArea.Clone();
                chart._bottomArea._parent = chart;
            }
            if (chart._topArea != null)
            {
                chart._topArea = chart._topArea.Clone();
                chart._topArea._parent = chart;
            }
            if (chart._footerArea != null)
            {
                chart._footerArea = chart._footerArea.Clone();
                chart._footerArea._parent = chart;
            }
            if (chart._leftArea != null)
            {
                chart._leftArea = chart._leftArea.Clone();
                chart._leftArea._parent = chart;
            }
            if (chart._rightArea != null)
            {
                chart._rightArea = chart._rightArea.Clone();
                chart._rightArea._parent = chart;
            }
            if (chart._plotArea != null)
            {
                chart._plotArea = chart._plotArea.Clone();
                chart._plotArea._parent = chart;
            }
            if (chart._dataLabel != null)
            {
                chart._dataLabel = chart._dataLabel.Clone();
                chart._dataLabel._parent = chart;
            }
            return chart;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the base type of the chart.
        /// ChartType of the series can be overwritten.
        /// </summary>
        public ChartType Type
        {
            get { return (ChartType)_type.Value; }
            set { _type.Value = (int)value; }
        }
        [DV(Type = typeof(ChartType))]
        internal NEnum _type = NEnum.NullValue(typeof(ChartType));

        /// <summary>
        /// Gets or sets the default style name of the whole chart.
        /// </summary>
        public string Style
        {
            get { return _style.Value; }
            set { _style.Value = value; }
        }
        [DV]
        internal NString _style = NString.NullValue;

        /// <summary>
        /// Gets the default paragraph format of the whole chart.
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
        /// Gets the X-Axis of the Chart.
        /// </summary>
        public Axis XAxis
        {
            get { return _xAxis ?? (_xAxis = new Axis(this)); }
            set
            {
                SetParent(value);
                _xAxis = value;
            }
        }
        [DV]
        internal Axis _xAxis;

        /// <summary>
        /// Gets the Y-Axis of the Chart.
        /// </summary>
        public Axis YAxis
        {
            get { return _yAxis ?? (_yAxis = new Axis(this)); }
            set
            {
                SetParent(value);
                _yAxis = value;
            }
        }
        [DV]
        internal Axis _yAxis;

        /// <summary>
        /// Gets the Z-Axis of the Chart.
        /// </summary>
        public Axis ZAxis
        {
            get { return _zAxis ?? (_zAxis = new Axis(this)); }
            set
            {
                SetParent(value);
                _zAxis = value;
            }
        }
        [DV]
        internal Axis _zAxis;

        /// <summary>
        /// Gets the collection of the data series.
        /// </summary>
        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection ?? (_seriesCollection = new SeriesCollection(this)); }
            set
            {
                SetParent(value);
                _seriesCollection = value;
            }
        }
        [DV(ItemType = typeof(Series))]
        internal SeriesCollection _seriesCollection;

        /// <summary>
        /// Gets the collection of the values written on the X-Axis.
        /// </summary>
        public XValues XValues
        {
            get { return _xValues ?? (_xValues = new XValues(this)); }
            set
            {
                SetParent(value);
                _xValues = value;
            }
        }
        [DV(ItemType = typeof(Series))]
        internal XValues _xValues;

        /// <summary>
        /// Gets the header area of the chart.
        /// </summary>
        public TextArea HeaderArea
        {
            get { return _headerArea ?? (_headerArea = new TextArea(this)); }
            set
            {
                SetParent(value);
                _headerArea = value;
            }
        }
        [DV]
        internal TextArea _headerArea;

        /// <summary>
        /// Gets the bottom area of the chart.
        /// </summary>
        public TextArea BottomArea
        {
            get { return _bottomArea ?? (_bottomArea = new TextArea(this)); }
            set
            {
                SetParent(value);
                _bottomArea = value;
            }
        }
        [DV]
        internal TextArea _bottomArea;

        /// <summary>
        /// Gets the top area of the chart.
        /// </summary>
        public TextArea TopArea
        {
            get { return _topArea ?? (_topArea = new TextArea(this)); }
            set
            {
                SetParent(value);
                _topArea = value;
            }
        }
        [DV]
        internal TextArea _topArea;

        /// <summary>
        /// Gets the footer area of the chart.
        /// </summary>
        public TextArea FooterArea
        {
            get { return _footerArea ?? (_footerArea = new TextArea(this)); }
            set
            {
                SetParent(value);
                _footerArea = value;
            }
        }
        [DV]
        internal TextArea _footerArea;

        /// <summary>
        /// Gets the left area of the chart.
        /// </summary>
        public TextArea LeftArea
        {
            get { return _leftArea ?? (_leftArea = new TextArea(this)); }
            set
            {
                SetParent(value);
                _leftArea = value;
            }
        }
        [DV]
        internal TextArea _leftArea;

        /// <summary>
        /// Gets the right area of the chart.
        /// </summary>
        public TextArea RightArea
        {
            get { return _rightArea ?? (_rightArea = new TextArea(this)); }
            set
            {
                SetParent(value);
                _rightArea = value;
            }
        }
        [DV]
        internal TextArea _rightArea;

        /// <summary>
        /// Gets the plot (drawing) area of the chart.
        /// </summary>
        public PlotArea PlotArea
        {
            get { return _plotArea ?? (_plotArea = new PlotArea(this)); }
            set
            {
                SetParent(value);
                _plotArea = value;
            }
        }
        [DV]
        internal PlotArea _plotArea;

        /// <summary>
        /// Gets or sets a value defining how blanks in the data series should be shown.
        /// </summary>
        public BlankType DisplayBlanksAs
        {
            get { return (BlankType)_displayBlanksAs.Value; }
            set { _displayBlanksAs.Value = (int)value; }
        }
        [DV(Type = typeof(BlankType))]
        internal NEnum _displayBlanksAs = NEnum.NullValue(typeof(BlankType));

        /// <summary>
        /// Gets or sets whether XAxis Labels should be merged.
        /// </summary>
        public bool PivotChart
        {
            get { return _pivotChart.Value; }
            set { _pivotChart.Value = value; }
        }
        [DV]
        internal NBool _pivotChart = NBool.NullValue;

        /// <summary>
        /// Gets the DataLabel of the chart.
        /// </summary>
        public DataLabel DataLabel
        {
            get { return _dataLabel ?? (_dataLabel = new DataLabel(this)); }
            set
            {
                SetParent(value);
                _dataLabel = value;
            }
        }
        [DV]
        internal DataLabel _dataLabel;

        /// <summary>
        /// Gets or sets whether the chart has a DataLabel.
        /// </summary>
        public bool HasDataLabel
        {
            get { return _hasDataLabel.Value; }
            set { _hasDataLabel.Value = value; }
        }
        [DV]
        internal NBool _hasDataLabel = NBool.NullValue;
        #endregion

        /// <summary>
        /// Determines the type of the given axis.
        /// </summary>
        internal string CheckAxis(Axis axis)
        {
            if ((_xAxis != null) && (axis == _xAxis))
                return "xaxis";
            if ((_yAxis != null) && (axis == _yAxis))
                return "yaxis";
            if ((_zAxis != null) && (axis == _zAxis))
                return "zaxis";

            return "";
        }

        /// <summary>
        /// Determines the type of the given textarea.
        /// </summary>
        internal string CheckTextArea(TextArea textArea)
        {
            if ((_headerArea != null) && (textArea == _headerArea))
                return "headerarea";
            if ((_footerArea != null) && (textArea == _footerArea))
                return "footerarea";
            if ((_leftArea != null) && (textArea == _leftArea))
                return "leftarea";
            if ((_rightArea != null) && (textArea == _rightArea))
                return "rightarea";
            if ((_topArea != null) && (textArea == _topArea))
                return "toparea";
            if ((_bottomArea != null) && (textArea == _bottomArea))
                return "bottomarea";

            return "";
        }

        #region Internal
        /// <summary>
        /// Converts Chart into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine("\\chart(" + Type + ")");
            int pos = serializer.BeginAttributes();

            base.Serialize(serializer);
            if (!_displayBlanksAs.IsNull)
                serializer.WriteSimpleAttribute("DisplayBlanksAs", DisplayBlanksAs);
            if (!_pivotChart.IsNull)
                serializer.WriteSimpleAttribute("PivotChart", PivotChart);
            if (!_hasDataLabel.IsNull)
                serializer.WriteSimpleAttribute("HasDataLabel", HasDataLabel);

            if (!_style.IsNull)
                serializer.WriteSimpleAttribute("Style", Style);
            if (!IsNull("Format"))
                _format.Serialize(serializer, "Format", null);
            if (!IsNull("DataLabel"))
                _dataLabel.Serialize(serializer);
            serializer.EndAttributes(pos);

            serializer.BeginContent();

            if (!IsNull("PlotArea"))
                _plotArea.Serialize(serializer);
            if (!IsNull("HeaderArea"))
                _headerArea.Serialize(serializer);
            if (!IsNull("FooterArea"))
                _footerArea.Serialize(serializer);
            if (!IsNull("TopArea"))
                _topArea.Serialize(serializer);
            if (!IsNull("BottomArea"))
                _bottomArea.Serialize(serializer);
            if (!IsNull("LeftArea"))
                _leftArea.Serialize(serializer);
            if (!IsNull("RightArea"))
                _rightArea.Serialize(serializer);

            if (!IsNull("XAxis"))
                _xAxis.Serialize(serializer);
            if (!IsNull("YAxis"))
                _yAxis.Serialize(serializer);
            if (!IsNull("ZAxis"))
                _zAxis.Serialize(serializer);

            if (!IsNull("SeriesCollection"))
                _seriesCollection.Serialize(serializer);
            if (!IsNull("XValues"))
                _xValues.Serialize(serializer);

            serializer.EndContent();
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and its child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitChart(this);
            if (visitChildren)
            {
                if (_bottomArea != null)
                    ((IVisitable)_bottomArea).AcceptVisitor(visitor, true);

                if (_footerArea != null)
                    ((IVisitable)_footerArea).AcceptVisitor(visitor, true);

                if (_headerArea != null)
                    ((IVisitable)_headerArea).AcceptVisitor(visitor, true);

                if (_leftArea != null)
                    ((IVisitable)_leftArea).AcceptVisitor(visitor, true);

                if (_rightArea != null)
                    ((IVisitable)_rightArea).AcceptVisitor(visitor, true);

                if (_topArea != null)
                    ((IVisitable)_topArea).AcceptVisitor(visitor, true);
            }
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Chart))); }
        }
        static Meta _meta;
        #endregion
    }
}
