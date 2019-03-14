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

namespace MigraDoc.DocumentObjectModel.Shapes
{
    /// <summary>
    /// Represents a barcode in the document or paragraph. !!!Still under Construction!!!
    /// </summary>
    public class Barcode : Shape
    {
        /// <summary>
        /// Initializes a new instance of the Barcode class.
        /// </summary>
        internal Barcode()
        { }

        /// <summary>
        /// Initializes a new instance of the Barcode class with the specified parent.
        /// </summary>
        internal Barcode(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Barcode Clone()
        {
            return (Barcode)DeepCopy();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text orientation for the barcode content.
        /// </summary>
        public TextOrientation Orientation
        {
            get { return (TextOrientation)_orientation.Value; }
            set { _orientation.Value = (int)value; }
        }
        [DV(Type = typeof(TextOrientation))]
        internal NEnum _orientation = NEnum.NullValue(typeof(TextOrientation));

        /// <summary>
        /// Gets or sets the type of the barcode.
        /// </summary>
        public BarcodeType Type
        {
            get { return (BarcodeType)_type.Value; }
            set { _type.Value = (int)value; }
        }
        [DV(Type = typeof(BarcodeType))]
        internal NEnum _type = NEnum.NullValue(typeof(BarcodeType));

        /// <summary>
        /// Gets or sets a value indicating whether bars shall appear beside the barcode
        /// </summary>
        public bool BearerBars
        {
            get { return _bearerBars.Value; }
            set { _bearerBars.Value = value; }
        }
        [DV]
        internal NBool _bearerBars = NBool.NullValue;

        /// <summary>
        /// Gets or sets the a value indicating whether the barcode's code is rendered.
        /// </summary>
        public bool Text
        {
            get { return _text.Value; }
            set { _text.Value = value; }
        }
        [DV]
        internal NBool _text = NBool.NullValue;

        /// <summary>
        /// Gets or sets code the barcode represents.
        /// </summary>
        public string Code
        {
            get { return _code.Value; }
            set { _code.Value = value; }
        }
        [DV]
        internal NString _code = NString.NullValue;

        /// <summary>
        /// ???
        /// </summary>
        public double LineRatio
        {
            get { return _lineRatio.Value; }
            set { _lineRatio.Value = value; }
        }
        [DV]
        internal NDouble _lineRatio = NDouble.NullValue;

        /// <summary>
        /// ???
        /// </summary>
        public double LineHeight
        {
            get { return _lineHeight.Value; }
            set { _lineHeight.Value = value; }
        }
        [DV]
        internal NDouble _lineHeight = NDouble.NullValue;

        /// <summary>
        /// ???
        /// </summary>
        public double NarrowLineWidth
        {
            get { return _narrowLineWidth.Value; }
            set { _narrowLineWidth.Value = value; }
        }
        [DV]
        internal NDouble _narrowLineWidth = NDouble.NullValue;
        #endregion

        #region Internal
        /// <summary>
        /// Converts Barcode into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            if (_code.Value == "")
                throw new InvalidOperationException(DomSR.MissingObligatoryProperty("Name", "BookmarkField"));

            serializer.WriteLine("\\barcode(\"" + Code + "\")");

            int pos = serializer.BeginAttributes();

            base.Serialize(serializer);

            if (!_orientation.IsNull)
                serializer.WriteSimpleAttribute("Orientation", Orientation);
            if (!_bearerBars.IsNull)
                serializer.WriteSimpleAttribute("BearerBars", BearerBars);
            if (!_text.IsNull)
                serializer.WriteSimpleAttribute("Text", Text);
            if (!_type.IsNull)
                serializer.WriteSimpleAttribute("Type", Type);
            if (!_lineRatio.IsNull)
                serializer.WriteSimpleAttribute("LineRatio", LineRatio);
            if (!_lineHeight.IsNull)
                serializer.WriteSimpleAttribute("LineHeight", LineHeight);
            if (!_narrowLineWidth.IsNull)
                serializer.WriteSimpleAttribute("NarrowLineWidth", NarrowLineWidth);

            serializer.EndAttributes(pos);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Barcode))); }
        }
        static Meta _meta;
        #endregion
    }
}
