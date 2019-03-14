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
using MigraDoc.DocumentObjectModel;
using PdfSharp.Drawing;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Represents a formatted header or footer.
    /// </summary>
    internal class FormattedHeaderFooter : IAreaProvider
    {
        internal FormattedHeaderFooter(HeaderFooter headerFooter, DocumentRenderer documentRenderer, FieldInfos fieldInfos)
        {
            _headerFooter = headerFooter;
            _fieldInfos = fieldInfos;
            _documentRenderer = documentRenderer;
        }

        internal void Format(XGraphics gfx)
        {
            _gfx = gfx;
            _isFirstArea = true;
            _formatter = new TopDownFormatter(this, _documentRenderer, _headerFooter.Elements);
            _formatter.FormatOnAreas(gfx, false);
            _contentHeight = RenderInfo.GetTotalHeight(GetRenderInfos());
        }

        Area IAreaProvider.GetNextArea()
        {
            if (_isFirstArea)
                return new Rectangle(ContentRect.X, ContentRect.Y, ContentRect.Width, double.MaxValue);

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

            return new RenderInfo[0];
        }

        internal Rectangle ContentRect
        {
            get { return _contentRect; }
            set { _contentRect = value; }
        }
        private Rectangle _contentRect;

        XUnit ContentHeight
        {
            get { return _contentHeight; }
        }
        private XUnit _contentHeight;


        bool IAreaProvider.PositionVertically(LayoutInfo layoutInfo)
        {
            IAreaProvider formattedDoc = _documentRenderer.FormattedDocument;
            return formattedDoc.PositionVertically(layoutInfo);
        }

        bool IAreaProvider.PositionHorizontally(LayoutInfo layoutInfo)
        {
            IAreaProvider formattedDoc = _documentRenderer.FormattedDocument;
            return formattedDoc.PositionHorizontally(layoutInfo);
        }

        HeaderFooter _headerFooter;
        FieldInfos _fieldInfos;
        TopDownFormatter _formatter;
        List<RenderInfo> _renderInfos;
        XGraphics _gfx;
        bool _isFirstArea;
        readonly DocumentRenderer _documentRenderer;
    }
}
