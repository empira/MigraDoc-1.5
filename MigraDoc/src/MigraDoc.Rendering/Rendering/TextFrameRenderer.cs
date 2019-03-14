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
    /// Renders textframes.
    /// </summary>
    internal class TextFrameRenderer : ShapeRenderer
    {
        internal TextFrameRenderer(XGraphics gfx, TextFrame textframe, FieldInfos fieldInfos)
            : base(gfx, textframe, fieldInfos)
        {
            _textframe = textframe;
            TextFrameRenderInfo renderInfo = new TextFrameRenderInfo();
            renderInfo.DocumentObject = _shape;
            _renderInfo = renderInfo;
        }

        internal TextFrameRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos)
            : base(gfx, renderInfo, fieldInfos)
        {
            _textframe = (TextFrame)renderInfo.DocumentObject;
        }

        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            FormattedTextFrame formattedTextFrame = new FormattedTextFrame(_textframe, _documentRenderer, _fieldInfos);
            formattedTextFrame.Format(_gfx);
            ((TextFrameFormatInfo)_renderInfo.FormatInfo).FormattedTextFrame = formattedTextFrame;
            base.Format(area, previousFormatInfo);
        }

        internal override LayoutInfo InitialLayoutInfo
        {
            get { return base.InitialLayoutInfo; }
        }

        internal override void Render()
        {
            RenderFilling();
            RenderContent();
            RenderLine();
        }

        void RenderContent()
        {
            FormattedTextFrame formattedTextFrame = ((TextFrameFormatInfo)_renderInfo.FormatInfo).FormattedTextFrame;
            RenderInfo[] renderInfos = formattedTextFrame.GetRenderInfos();
            if (renderInfos == null)
                return;

            XGraphicsState state = Transform();
            RenderByInfos(renderInfos);
            ResetTransform(state);
        }

        XGraphicsState Transform()
        {
            Area frameContentArea = _renderInfo.LayoutInfo.ContentArea;
            XGraphicsState state = _gfx.Save();
            XUnit xPosition;
            XUnit yPosition;
            switch (_textframe.Orientation)
            {
                case TextOrientation.Downward:
                case TextOrientation.Vertical:
                case TextOrientation.VerticalFarEast:
                    xPosition = frameContentArea.X + frameContentArea.Width;
                    yPosition = frameContentArea.Y;
                    _gfx.TranslateTransform(xPosition, yPosition);
                    _gfx.RotateTransform(90);
                    break;

                case TextOrientation.Upward:
                    state = _gfx.Save();
                    xPosition = frameContentArea.X;
                    yPosition = frameContentArea.Y + frameContentArea.Height;
                    _gfx.TranslateTransform(xPosition, yPosition);
                    _gfx.RotateTransform(-90);
                    break;

                default:
                    xPosition = frameContentArea.X;
                    yPosition = frameContentArea.Y;
                    _gfx.TranslateTransform(xPosition, yPosition);
                    break;
            }
            return state;
        }

        void ResetTransform(XGraphicsState state)
        {
            if (state != null)
                _gfx.Restore(state);
        }

        readonly TextFrame _textframe;
    }
}
