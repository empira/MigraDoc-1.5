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

using MigraDoc.DocumentObjectModel;
using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel.Shapes;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Renders a line format to an XGraphics object.
    /// </summary>
    internal class LineFormatRenderer
    {
        public LineFormatRenderer(LineFormat lineFormat, XGraphics gfx)
        {
            _lineFormat = lineFormat;
            _gfx = gfx;
        }

        private XColor GetColor()
        {
            Color clr = Colors.Black;

            if (_lineFormat != null && !_lineFormat.Color.IsEmpty)
                clr = _lineFormat.Color;

#if noCMYK
      return XColor.FromArgb((int)clr.Argb);
#else
            return ColorHelper.ToXColor(clr, _lineFormat.Document.UseCmykColor);
#endif
        }

        internal XUnit GetWidth()
        {
            if (_lineFormat == null)
                return 0;
            if (!_lineFormat._visible.IsNull && !_lineFormat.Visible)
                return 0;

            if (!_lineFormat._width.IsNull)
                return _lineFormat.Width.Point;

            if (!_lineFormat._color.IsNull || !_lineFormat._style.IsNull || _lineFormat.Visible)
                return 1;

            return 0;
        }

        internal void Render(XUnit xPosition, XUnit yPosition, XUnit width, XUnit height)
        {
            XUnit lineWidth = GetWidth();
            if (lineWidth > 0)
            {
                XPen pen = GetPen(lineWidth);
                _gfx.DrawRectangle(pen, xPosition, yPosition, width, height);
            }
        }

        XPen GetPen(XUnit width)
        {
            if (width == 0)
                return null;

            XPen pen = new XPen(GetColor(), width);
            switch (_lineFormat.DashStyle)
            {
                case DashStyle.Dash:
                    pen.DashStyle = XDashStyle.Dash;
                    break;

                case DashStyle.DashDot:
                    pen.DashStyle = XDashStyle.DashDot;
                    break;

                case DashStyle.DashDotDot:
                    pen.DashStyle = XDashStyle.DashDotDot;
                    break;

                case DashStyle.Solid:
                    pen.DashStyle = XDashStyle.Solid;
                    break;

                case DashStyle.SquareDot:
                    pen.DashStyle = XDashStyle.Dot;
                    break;
            }
            return pen;
        }

        readonly LineFormat _lineFormat;
        readonly XGraphics _gfx;
    }
}
