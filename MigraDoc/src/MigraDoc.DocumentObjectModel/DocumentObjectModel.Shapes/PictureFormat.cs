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

#pragma warning disable 1591

namespace MigraDoc.DocumentObjectModel.Shapes
{
    /// <summary>
    /// A PictureFormat object.
    /// Used to set more detailed image attributes
    /// </summary>
    public class PictureFormat : DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the PictureFormat class.
        /// </summary>
        public PictureFormat()
        { }

        /// <summary>
        /// Initializes a new instance of the PictureFormat class with the specified parent.
        /// </summary>
        internal PictureFormat(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new PictureFormat Clone()
        {
            return (PictureFormat)DeepCopy();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the part cropped from the left of the image.
        /// </summary>
        public Unit CropLeft
        {
            get { return _cropLeft; }
            set { _cropLeft = value; }
        }
        [DV]
        protected Unit _cropLeft = Unit.NullValue;

        /// <summary>
        /// Gets or sets the part cropped from the right of the image.
        /// </summary>
        public Unit CropRight
        {
            get { return _cropRight; }
            set { _cropRight = value; }
        }
        [DV]
        protected Unit _cropRight = Unit.NullValue;

        /// <summary>
        /// Gets or sets the part cropped from the top of the image.
        /// </summary>
        public Unit CropTop
        {
            get { return _cropTop; }
            set { _cropTop = value; }
        }
        [DV]
        protected Unit _cropTop = Unit.NullValue;

        /// <summary>
        /// Gets or sets the part cropped from the bottom of the image.
        /// </summary>
        public Unit CropBottom
        {
            get { return _cropBottom; }
            set { _cropBottom = value; }
        }
        [DV]
        protected Unit _cropBottom = Unit.NullValue;
        #endregion

        #region Internal
        /// <summary>
        /// Converts PictureFormat into DDL
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.BeginContent("PictureFormat");
            if (!_cropLeft.IsNull)
                serializer.WriteSimpleAttribute("CropLeft", CropLeft);
            if (!_cropRight.IsNull)
                serializer.WriteSimpleAttribute("CropRight", CropRight);
            if (!_cropTop.IsNull)
                serializer.WriteSimpleAttribute("CropTop", CropTop);
            if (!_cropBottom.IsNull)
                serializer.WriteSimpleAttribute("CropBottom", CropBottom);
            serializer.EndContent();
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(PictureFormat))); }
        }
        static Meta _meta;
        #endregion
    }
}
