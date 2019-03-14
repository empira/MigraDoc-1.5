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

using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel.Shapes;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Renders fill formats.
    /// </summary>
    internal class FillFormatRenderer
    {
        public FillFormatRenderer(FillFormat fillFormat, XGraphics gfx)
        {
            _gfx = gfx;
            _fillFormat = fillFormat;
        }

        internal void Render(XUnit x, XUnit y, XUnit width, XUnit height)
        {
            XBrush brush = GetBrush();

            if (brush == null)
                return;

            _gfx.DrawRectangle(brush, x.Point, y.Point, width.Point, height.Point);
        }

        private bool IsVisible()
        {
            if (!_fillFormat._visible.IsNull)
                return _fillFormat.Visible;
            return !_fillFormat._color.IsNull;
        }

        private XBrush GetBrush()
        {
            if (_fillFormat == null || !IsVisible())
                return null;

#if noCMYK
      return new XSolidBrush(XColor.FromArgb(_fillFormat.Color.Argb));
#else
            return new XSolidBrush(ColorHelper.ToXColor(_fillFormat.Color, _fillFormat.Document.UseCmykColor));
#endif
        }

        readonly XGraphics _gfx;
        readonly FillFormat _fillFormat;
    }
}
