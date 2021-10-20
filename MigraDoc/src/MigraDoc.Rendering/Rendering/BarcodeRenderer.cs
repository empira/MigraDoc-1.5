#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Klaus Potzesny (mailto:Klaus.Potzesny@pdfsharp.com)
//   Adam Brengesjö (mailto:ca.brengesjo@gmail.com)
//
// Copyright (c) 2001-2009 empira Software GmbH, Cologne (Germany)
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

using PdfSharp.Drawing;
using PdfSharp.Drawing.BarCodes;
using MigraDoc.DocumentObjectModel.Shapes;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Renders images.
    /// </summary>
    internal class BarcodeRenderer : ShapeRenderer
    {
        internal BarcodeRenderer(XGraphics gfx, Barcode barcode, FieldInfos fieldInfos)
            : base(gfx, barcode, fieldInfos)
        {
            _barcode = barcode;
            BarcodeRenderInfo renderInfo = new BarcodeRenderInfo();
			renderInfo.DocumentObject = _shape;
			_renderInfo = renderInfo;
		}

        internal BarcodeRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos)
            : base(gfx, renderInfo, fieldInfos)
        {
            _barcode = (Barcode)renderInfo.DocumentObject;
        }

        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            BarcodeFormatInfo formatInfo = (BarcodeFormatInfo)_renderInfo.FormatInfo;

            formatInfo.Height = _barcode.Height.Point;
            formatInfo.Width = _barcode.Width.Point;

            base.Format(area, previousFormatInfo);
        }

        protected override XUnit ShapeHeight
        {
            get
            {
                BarcodeFormatInfo formatInfo = (BarcodeFormatInfo)_renderInfo.FormatInfo;
                return formatInfo.Height + _lineFormatRenderer.GetWidth();
            }
        }

        protected override XUnit ShapeWidth
        {
            get
            {
                BarcodeFormatInfo formatInfo = (BarcodeFormatInfo)_renderInfo.FormatInfo;
                return formatInfo.Width + _lineFormatRenderer.GetWidth();
            }
        }

        internal override void Render()
        {
            RenderFilling();

            BarcodeFormatInfo formatInfo = (BarcodeFormatInfo)_renderInfo.FormatInfo;
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            XRect destRect = new XRect(contentArea.X, contentArea.Y, formatInfo.Width, formatInfo.Height);

            BarCode gfxBarcode = null;

            if (_barcode.Type == BarcodeType.Barcode39)
                gfxBarcode = new Code3of9Standard(_barcode.Code);
            else if (_barcode.Type == BarcodeType.Barcode25i)
                gfxBarcode = new Code2of5Interleaved();

            // if gfxBarcode is null, the barcode type is not supported
            if (gfxBarcode != null)
            {
				if (_barcode.Text)
					gfxBarcode.TextLocation = TextLocation.Below;
                gfxBarcode.Direction = CodeDirection.LeftToRight;
                gfxBarcode.Size = new XSize(ShapeWidth, ShapeHeight);

                _gfx.DrawBarCode(gfxBarcode, XBrushes.Black, destRect.Location);
            }

            RenderLine();
        }

        Barcode _barcode;
    }
}
