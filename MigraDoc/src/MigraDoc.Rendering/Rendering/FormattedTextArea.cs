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

using System;
using System.Collections.Generic;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using PdfSharp.Drawing;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Represents a formatted text area.
    /// </summary>
    internal class FormattedTextArea : IAreaProvider
    {
        internal FormattedTextArea(DocumentRenderer documentRenderer, TextArea textArea, FieldInfos fieldInfos)
        {
            TextArea = textArea;
            _fieldInfos = fieldInfos;
            _documentRenderer = documentRenderer;
        }

        internal void Format(XGraphics gfx)
        {
            _gfx = gfx;
            _isFirstArea = true;
            _formatter = new TopDownFormatter(this, _documentRenderer, TextArea.Elements);
            _formatter.FormatOnAreas(gfx, false);
        }

        internal XUnit InnerWidth
        {
            set { _innerWidth = value; }
            get
            {
                if (double.IsNaN(_innerWidth))
                {
                    if (!TextArea._width.IsNull)
                        _innerWidth = TextArea.Width.Point;
                    else
                        _innerWidth = CalcInherentWidth();
                }
                return _innerWidth;
            }
        }
        XUnit _innerWidth = double.NaN;

        internal XUnit InnerHeight
        {
            get
            {
                if (TextArea._height.IsNull)
                    return ContentHeight + TextArea.TopPadding + TextArea.BottomPadding;
                return TextArea.Height.Point;
            }
        }


        XUnit CalcInherentWidth()
        {
            XUnit inherentWidth = 0;
            foreach (DocumentObject obj in TextArea.Elements)
            {
                Renderer renderer = Renderer.Create(_gfx, _documentRenderer, obj, _fieldInfos);
                if (renderer != null)
                {
                    renderer.Format(new Rectangle(0, 0, double.MaxValue, double.MaxValue), null);
                    inherentWidth = Math.Max(renderer.RenderInfo.LayoutInfo.MinWidth, inherentWidth);
                }
            }
            inherentWidth += TextArea.LeftPadding;
            inherentWidth += TextArea.RightPadding;
            return inherentWidth;
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

        internal XUnit ContentHeight
        {
            get { return RenderInfo.GetTotalHeight(GetRenderInfos()); }
        }

        Rectangle CalcContentRect()
        {
            XUnit width = InnerWidth - TextArea.LeftPadding - TextArea.RightPadding;
            XUnit height = double.MaxValue;
            return new Rectangle(0, 0, width, height);
        }

        bool IAreaProvider.PositionVertically(LayoutInfo layoutInfo)
        {
            return false;
        }

        bool IAreaProvider.PositionHorizontally(LayoutInfo layoutInfo)
        {
            return false;
        }

        internal readonly TextArea TextArea;

        readonly FieldInfos _fieldInfos;
        TopDownFormatter _formatter;
        List<RenderInfo> _renderInfos;
        XGraphics _gfx;
        bool _isFirstArea;
        readonly DocumentRenderer _documentRenderer;
    }
}
