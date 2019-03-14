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

using System.Diagnostics;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Drawing;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Renders a single Border.
    /// </summary>
    internal class BordersRenderer
    {
        internal BordersRenderer(Borders borders, XGraphics gfx)
        {
            Debug.Assert(borders.Document != null);
            _gfx = gfx;
            _borders = borders;
        }

        private Border GetBorder(BorderType type)
        {
            return _borders.GetBorderReadOnly(type);
        }

        private XColor GetColor(BorderType type)
        {
            Color clr = Colors.Black;

            Border border = GetBorder(type);
            if (border != null && !border.Color.IsEmpty)
                clr = border.Color;
            else if (!_borders.Color.IsEmpty)
                clr = _borders.Color;

#if noCMYK
      return XColor.FromArgb((int)clr.Argb);
#else
            //      bool cmyk = false; // BUG CMYK
            //      if (_borders.Document != null)
            //        cmyk = _borders.Document.UseCmykColor;
            //#if DEBUG
            //      else
            //        GetT ype();
            //#endif
            return ColorHelper.ToXColor(clr, _borders.Document.UseCmykColor);
#endif
        }

        private BorderStyle GetStyle(BorderType type)
        {
            BorderStyle style = BorderStyle.Single;

            Border border = GetBorder(type);
            if (border != null && !border._style.IsNull)
                style = border.Style;
            else if (!_borders._style.IsNull)
                style = _borders.Style;
            return style;
        }

        internal XUnit GetWidth(BorderType type)
        {
            if (_borders == null)
                return 0;

            Border border = GetBorder(type);

            if (border != null)
            {
                if (!border._visible.IsNull && !border.Visible)
                    return 0;

                if (!border._width.IsNull)
                    return border.Width.Point;

                if (!border._color.IsNull || !border._style.IsNull || border.Visible)
                {
                    if (!_borders._width.IsNull)
                        return _borders.Width.Point;

                    return 0.5;
                }
            }
            else if (!(type == BorderType.DiagonalDown || type == BorderType.DiagonalUp))
            {
                if (!_borders._visible.IsNull && !_borders.Visible)
                    return 0;

                if (!_borders._width.IsNull)
                    return _borders.Width.Point;

                if (!_borders._color.IsNull || !_borders._style.IsNull || _borders.Visible)
                    return 0.5;
            }
            return 0;
        }

        /// <summary>
        /// Renders the border top down.
        /// </summary>
        /// <param name="type">The type of the border.</param>
        /// <param name="left">The left position of the border.</param>
        /// <param name="top">The top position of the border.</param>
        /// <param name="height">The height on which to render the border.</param>
        internal void RenderVertically(BorderType type, XUnit left, XUnit top, XUnit height)
        {
            XUnit borderWidth = GetWidth(type);
            if (borderWidth == 0)
                return;

            left += borderWidth / 2;
            _gfx.DrawLine(GetPen(type), left, top + height, left, top);
        }

        /// <summary>
        /// Renders the border top down.
        /// </summary>
        /// <param name="type">The type of the border.</param>
        /// <param name="left">The left position of the border.</param>
        /// <param name="top">The top position of the border.</param>
        /// <param name="width">The width on which to render the border.</param>
        internal void RenderHorizontally(BorderType type, XUnit left, XUnit top, XUnit width)
        {
            XUnit borderWidth = GetWidth(type);
            if (borderWidth == 0)
                return;

            top += borderWidth / 2;
            _gfx.DrawLine(GetPen(type), left + width, top, left, top);
        }


        internal void RenderDiagonally(BorderType type, XUnit left, XUnit top, XUnit width, XUnit height)
        {
            XUnit borderWidth = GetWidth(type);
            if (borderWidth == 0)
                return;

            XGraphicsState state = _gfx.Save();
            _gfx.IntersectClip(new XRect(left, top, width, height));

            if (type == BorderType.DiagonalDown)
                _gfx.DrawLine(GetPen(type), left, top, left + width, top + height);
            else if (type == BorderType.DiagonalUp)
                _gfx.DrawLine(GetPen(type), left, top + height, left + width, top);

            _gfx.Restore(state);
        }

        internal void RenderRounded(RoundedCorner roundedCorner, XUnit x, XUnit y, XUnit width, XUnit height)
        {
            if (roundedCorner == RoundedCorner.None)
                return;

            // As source we use the vertical borders.
            // If not set originally, they have been set to the horizontal border values in TableRenderer.EqualizeRoundedCornerBorders().
            BorderType borderType = BorderType.Top;
            if (roundedCorner == RoundedCorner.TopLeft || roundedCorner == RoundedCorner.BottomLeft)
                borderType = BorderType.Left;
            if (roundedCorner == RoundedCorner.TopRight || roundedCorner == RoundedCorner.BottomRight)
                borderType = BorderType.Right;

            XUnit borderWidth = GetWidth(borderType);
            XPen borderPen = GetPen(borderType);

            if (borderWidth == 0)
                return;


            x -= borderWidth / 2;
            y -= borderWidth / 2;
            XUnit ellipseWidth = width * 2 + borderWidth;
            XUnit ellipseHeight = height * 2 + borderWidth;

            switch (roundedCorner)
            {
                case RoundedCorner.TopLeft:
                    _gfx.DrawArc(borderPen, new XRect(x, y, ellipseWidth, ellipseHeight), 180, 90);
                    break;
                case RoundedCorner.TopRight:
                    _gfx.DrawArc(borderPen, new XRect(x - width, y, ellipseWidth, ellipseHeight), 270, 90);
                    break;
                case RoundedCorner.BottomRight:
                    _gfx.DrawArc(borderPen, new XRect(x - width, y - height, ellipseWidth, ellipseHeight), 0, 90);
                    break;
                case RoundedCorner.BottomLeft:
                    _gfx.DrawArc(borderPen, new XRect(x, y - height, ellipseWidth, ellipseHeight), 90, 90);
                    break;
            }
        }

        private XPen GetPen(BorderType type)
        {
            XUnit borderWidth = GetWidth(type);
            if (borderWidth == 0)
                return null;

            XPen pen = new XPen(GetColor(type), borderWidth);
            BorderStyle style = GetStyle(type);
            switch (style)
            {
                case BorderStyle.DashDot:
                    pen.DashStyle = XDashStyle.DashDot;
                    break;

                case BorderStyle.DashDotDot:
                    pen.DashStyle = XDashStyle.DashDotDot;
                    break;

                case BorderStyle.DashLargeGap:
                    pen.DashPattern = new double[] { 3, 3 };
                    break;

                case BorderStyle.DashSmallGap:
                    pen.DashPattern = new double[] { 5, 1 };
                    break;

                case BorderStyle.Dot:
                    pen.DashStyle = XDashStyle.Dot;
                    break;

                case BorderStyle.Single:
                default:
                    pen.DashStyle = XDashStyle.Solid;
                    break;
            }
            return pen;
        }

        internal bool IsRendered(BorderType borderType)
        {
            if (_borders == null)
                return false;

            switch (borderType)
            {
                case BorderType.Left:
                    if (_borders._left == null || _borders._left.IsNull())
                        return false;
                    return GetWidth(borderType) > 0;

                case BorderType.Right:
                    if (_borders._right == null || _borders._right.IsNull())
                        return false;
                    return GetWidth(borderType) > 0;

                case BorderType.Top:
                    if (_borders._top == null || _borders._top.IsNull())
                        return false;
                    return GetWidth(borderType) > 0;

                case BorderType.Bottom:
                    if (_borders._bottom == null || _borders._bottom.IsNull())
                        return false;

                    return GetWidth(borderType) > 0;

                case BorderType.DiagonalDown:
                    if (_borders._diagonalDown == null || _borders._diagonalDown.IsNull())
                        return false;
                    return GetWidth(borderType) > 0;

                case BorderType.DiagonalUp:
                    if (_borders._diagonalUp == null || _borders._diagonalUp.IsNull())
                        return false;

                    return GetWidth(borderType) > 0;
            }
            return false;
        }

        readonly XGraphics _gfx;
        readonly Borders _borders;
    }
}
