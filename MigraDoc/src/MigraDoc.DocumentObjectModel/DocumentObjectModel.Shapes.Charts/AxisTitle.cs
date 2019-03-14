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

namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    /// <summary>
    /// Represents the title of an axis.
    /// </summary>
    public class AxisTitle : ChartObject
    {
        /// <summary>
        /// Initializes a new instance of the AxisTitle class.
        /// </summary>
        public AxisTitle()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AxisTitle class with the specified parent.
        /// </summary>
        internal AxisTitle(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new AxisTitle Clone()
        {
            return (AxisTitle)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            AxisTitle axisTitle = (AxisTitle)base.DeepCopy();
            if (axisTitle._font != null)
            {
                axisTitle._font = axisTitle._font.Clone();
                axisTitle._font._parent = axisTitle;
            }
            return axisTitle;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the style name of the axis.
        /// </summary>
        public string Style
        {
            get { return _style.Value; }
            set { _style.Value = value; }
        }
        [DV]
        internal NString _style = NString.NullValue;

        /// <summary>
        /// Gets or sets the caption of the title.
        /// </summary>
        public string Caption
        {
            get { return _caption.Value; }
            set { _caption.Value = value; }
        }
        [DV]
        internal NString _caption = NString.NullValue;

        /// <summary>
        /// Gets the font object of the title.
        /// </summary>
        public Font Font
        {
            get { return _font ?? (_font = new Font(this)); }
            set
            {
                SetParent(value);
                _font = value;
            }
        }
        [DV]
        internal Font _font;

        /// <summary>
        /// Gets or sets the orientation of the caption.
        /// </summary>
        public Unit Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }
        [DV]
        internal Unit _orientation = Unit.NullValue;

        /// <summary>
        /// Gets or sets the alignment of the caption.
        /// </summary>
        public HorizontalAlignment Alignment
        {
            get { return (HorizontalAlignment)_alignment.Value; }
            set { _alignment.Value = (int)value; }
        }
        [DV(Type = typeof(HorizontalAlignment))]
        internal NEnum _alignment = NEnum.NullValue(typeof(HorizontalAlignment));

        /// <summary>
        /// Gets or sets the alignment of the caption.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)_verticalAlignment.Value; }
            set { _verticalAlignment.Value = (int)value; }
        }
        [DV(Type = typeof(VerticalAlignment))]
        internal NEnum _verticalAlignment = NEnum.NullValue(typeof(VerticalAlignment));
        #endregion

        #region Internal
        /// <summary>
        /// Converts AxisTitle into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            int pos = serializer.BeginContent("Title");

            if (!_style.IsNull)
                serializer.WriteSimpleAttribute("Style", Style);

            if (!IsNull("Font"))
                _font.Serialize(serializer);

            if (!_orientation.IsNull)
                serializer.WriteSimpleAttribute("Orientation", Orientation);

            if (!_alignment.IsNull)
                serializer.WriteSimpleAttribute("Alignment", Alignment);

            if (!_verticalAlignment.IsNull)
                serializer.WriteSimpleAttribute("VerticalAlignment", VerticalAlignment);

            if (!_caption.IsNull)
                serializer.WriteSimpleAttribute("Caption", Caption);

            serializer.EndContent();
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(AxisTitle))); }
        }
        static Meta _meta;
        #endregion
    }
}
