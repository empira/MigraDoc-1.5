#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Klaus Potzesny
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

using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Renders a Shading to an XGraphics object.
    /// </summary>
    internal class ShadingRenderer
    {
        public ShadingRenderer(XGraphics gfx, Shading shading)
        {
            _gfx = gfx;
            _shading = shading;
            RealizeBrush();
        }
        
        internal void Render(XUnit x, XUnit y, XUnit width, XUnit height)
        {
            if (_shading == null || _brush == null)
                return;

            _gfx.DrawRectangle(_brush, x.Point, y.Point, width.Point, height.Point);
        }

        internal void Render(XUnit x, XUnit y, XUnit width, XUnit height, RoundedCorner roundedCorner)
        {
            // If there is no rounded corner, we can use the usual Render method.
            if (roundedCorner == RoundedCorner.None)
            {
                Render(x, y, width, height);
                return;
            }

            if (_shading == null || _brush == null)
                return;

            XGraphicsPath path = new XGraphicsPath();

            switch (roundedCorner)
            {
                case RoundedCorner.TopLeft:
                    path.AddArc(new XRect(x, y, width * 2, height * 2), 180, 90); // Error in CORE: _corePath.AddArc().
                    path.AddLine(new XPoint(x + width, y), new XPoint(x + width, y + height));
                    break;
                case RoundedCorner.TopRight:
                    path.AddArc(new XRect(x - width, y, width * 2, height * 2), 270, 90); // Error in CORE: _corePath.AddArc().
                    path.AddLine(new XPoint(x + width, y + height), new XPoint(x, y + height));
                    break;
                case RoundedCorner.BottomRight:
                    path.AddArc(new XRect(x - width, y - height, width * 2, height * 2), 0, 90); // Error in CORE: _corePath.AddArc().
                    path.AddLine(new XPoint(x, y + height), new XPoint(x, y));
                    break;
                case RoundedCorner.BottomLeft:
                    path.AddArc(new XRect(x, y - height, width * 2, height * 2), 90, 90); // Error in CORE: _corePath.AddArc().
                    path.AddLine(new XPoint(x, y), new XPoint(x + width, y));
                    break;
            }

            path.CloseFigure();
            _gfx.DrawPath(_brush, path);
        }

        private bool IsVisible()
        {
            if (!_shading._visible.IsNull)
                return _shading.Visible;
            else
                return !_shading._color.IsNull;
        }

        private void RealizeBrush()
        {
            if (_shading == null)
                return;
            if (IsVisible())
            {
#if noCMYK
                this.brush = new XSolidBrush(XColor.FromArgb((int)this.shading.Color.Argb));
#else
                _brush = new XSolidBrush(ColorHelper.ToXColor(_shading.Color, _shading.Document.UseCmykColor));
#endif
            }
        }
        readonly Shading _shading;
        XBrush _brush;
        readonly XGraphics _gfx;
    }
}
