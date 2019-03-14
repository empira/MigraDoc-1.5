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

using System.Collections.Generic;
using MigraDoc.DocumentObjectModel.Shapes;
using PdfSharp.Drawing;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Represents a formatted text frame.
    /// </summary>
    internal class FormattedTextFrame : IAreaProvider
    {
        internal FormattedTextFrame(TextFrame textframe, DocumentRenderer documentRenderer, FieldInfos fieldInfos)
        {
            _textframe = textframe;
            _fieldInfos = fieldInfos;
            _documentRenderer = documentRenderer;
        }

        internal void Format(XGraphics gfx)
        {
            _gfx = gfx;
            _isFirstArea = true;
            _formatter = new TopDownFormatter(this, _documentRenderer, _textframe.Elements);
            _formatter.FormatOnAreas(gfx, false);
            _contentHeight = RenderInfo.GetTotalHeight(GetRenderInfos());
        }

        Area IAreaProvider.GetNextArea()
        {
            if (_isFirstArea)
                return CalcContentRect();
            return null;
        }

        Area IAreaProvider.ProbeNextArea()
        {
            return null;
        }

        FieldInfos IAreaProvider.AreaFieldInfos
        {
            get { return _fieldInfos; }
        }

        void IAreaProvider.StoreRenderInfos(List<RenderInfo> renderInfos)
        {
            _renderInfos = renderInfos;
        }

        bool IAreaProvider.IsAreaBreakBefore(LayoutInfo layoutInfo)
        {
            return false;
        }

        internal RenderInfo[] GetRenderInfos()
        {
            if (_renderInfos != null)
                return _renderInfos.ToArray();
            return null;
        }

        Rectangle CalcContentRect()
        {
            LineFormatRenderer lfr = new LineFormatRenderer(_textframe.LineFormat, _gfx);
            XUnit lineWidth = lfr.GetWidth();
            XUnit width;
            XUnit xOffset = lineWidth / 2;
            XUnit yOffset = lineWidth / 2;

            if (_textframe.Orientation == TextOrientation.Horizontal ||
              _textframe.Orientation == TextOrientation.HorizontalRotatedFarEast)
            {
                width = _textframe.Width.Point;
                xOffset += _textframe.MarginLeft;
                yOffset += _textframe.MarginTop;
                width -= xOffset;
                width -= _textframe.MarginRight + lineWidth / 2;
            }
            else
            {
                width = _textframe.Height.Point;
                if (_textframe.Orientation == TextOrientation.Upward)
                {
                    xOffset += _textframe.MarginBottom;
                    yOffset += _textframe.MarginLeft;
                    width -= xOffset;
                    width -= _textframe.MarginTop + lineWidth / 2;
                }
                else
                {
                    xOffset += _textframe.MarginTop;
                    yOffset += _textframe.MarginRight;
                    width -= xOffset;
                    width -= _textframe.MarginBottom + lineWidth / 2;
                }
            }
            XUnit height = double.MaxValue;
            return new Rectangle(xOffset, yOffset, width, height);
        }

        XUnit ContentHeight
        {
            get { return _contentHeight; }
        }

        bool IAreaProvider.PositionVertically(LayoutInfo layoutInfo)
        {
            return false;
        }

        bool IAreaProvider.PositionHorizontally(LayoutInfo layoutInfo)
        {
            Rectangle rect = CalcContentRect();
            switch (layoutInfo.HorizontalAlignment)
            {
                case ElementAlignment.Near:
                    if (layoutInfo.Left != 0)
                    {
                        layoutInfo.ContentArea.X += layoutInfo.Left;
                        return true;
                    }
                    return false;

                case ElementAlignment.Far:
                    XUnit xPos = rect.X + rect.Width;
                    xPos -= layoutInfo.ContentArea.Width;
                    xPos -= layoutInfo.MarginRight;
                    layoutInfo.ContentArea.X = xPos;
                    return true;

                case ElementAlignment.Center:
                    xPos = rect.Width;
                    xPos -= layoutInfo.ContentArea.Width;
                    xPos = rect.X + xPos / 2;
                    layoutInfo.ContentArea.X = xPos;
                    return true;
            }
            return false;
        }

        readonly TextFrame _textframe;
        readonly FieldInfos _fieldInfos;
        TopDownFormatter _formatter;
        List<RenderInfo> _renderInfos;
        XGraphics _gfx;
        bool _isFirstArea;
        XUnit _contentHeight;
        readonly DocumentRenderer _documentRenderer;
    }
}
