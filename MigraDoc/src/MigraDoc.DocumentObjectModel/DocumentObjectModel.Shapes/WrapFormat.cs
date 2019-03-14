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

namespace MigraDoc.DocumentObjectModel.Shapes
{
    /// <summary>
    /// Define how the shape should be wrapped between the texts.
    /// </summary>
    public class WrapFormat : DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the WrapFormat class.
        /// </summary>
        public WrapFormat()
        { }

        /// <summary>
        /// Initializes a new instance of the WrapFormat class with the specified parent.
        /// </summary>
        internal WrapFormat(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new WrapFormat Clone()
        {
            return (WrapFormat)DeepCopy();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the wrapping style.
        /// </summary>
        public WrapStyle Style
        {
            get { return (WrapStyle)_style.Value; }
            set { _style.Value = (int)value; }
        }
        [DV(Type = typeof(WrapStyle))]
        internal NEnum _style = NEnum.NullValue(typeof(WrapStyle));

        /// <summary>
        /// Gets or sets the distance between the top side of the shape with the adjacent text.
        /// </summary>
        public Unit DistanceTop
        {
            get { return _distanceTop; }
            set { _distanceTop = value; }
        }
        [DV]
        Unit _distanceTop = Unit.NullValue;

        /// <summary>
        /// Gets or sets the distance between the bottom side of the shape with the adjacent text.
        /// </summary>
        public Unit DistanceBottom
        {
            get { return _distanceBottom; }
            set { _distanceBottom = value; }
        }
        [DV]
        Unit _distanceBottom = Unit.NullValue;

        /// <summary>
        /// Gets or sets the distance between the left side of the shape with the adjacent text.
        /// </summary>
        public Unit DistanceLeft
        {
            get { return _distanceLeft; }
            set { _distanceLeft = value; }
        }
        [DV]
        Unit _distanceLeft = Unit.NullValue;

        /// <summary>
        /// Gets or sets the distance between the right side of the shape with the adjacent text.
        /// </summary>
        public Unit DistanceRight
        {
            get { return _distanceRight; }
            set { _distanceRight = value; }
        }
        [DV]
        Unit _distanceRight = Unit.NullValue;
        #endregion

        #region Internal
        /// <summary>
        /// Converts WrapFormat into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            int pos = serializer.BeginContent("WrapFormat");
            if (!_style.IsNull)
                serializer.WriteSimpleAttribute("Style", Style);
            if (!_distanceTop.IsNull)
                serializer.WriteSimpleAttribute("DistanceTop", DistanceTop);
            if (!_distanceLeft.IsNull)
                serializer.WriteSimpleAttribute("DistanceLeft", DistanceLeft);
            if (!_distanceRight.IsNull)
                serializer.WriteSimpleAttribute("DistanceRight", DistanceRight);
            if (!_distanceBottom.IsNull)
                serializer.WriteSimpleAttribute("DistanceBottom", DistanceBottom);
            serializer.EndContent();
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(WrapFormat))); }
        }
        static Meta _meta;
        #endregion
    }
}