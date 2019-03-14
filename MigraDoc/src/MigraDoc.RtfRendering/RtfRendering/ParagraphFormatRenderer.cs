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

namespace MigraDoc.RtfRendering
{
    /// <summary>
    /// Class to render a ParagraphFormat to RTF.
    /// </summary>
    internal class ParagraphFormatRenderer : RendererBase
    {
        /// <summary>
        /// Initializes a new instance of the Paragraph Renderer class.
        /// </summary>
        internal ParagraphFormatRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer)
            : base(domObj, docRenderer)
        {
            _format = domObj as ParagraphFormat;
        }

        /// <summary>
        /// Renders a ParagraphFormat object.
        /// </summary>
        internal override void Render()
        {
            _useEffectiveValue = true; //unfortunately has to be true always.

            Translate("Alignment", "q");
            Translate("SpaceBefore", "sb");
            Translate("SpaceAfter", "sa");

            TranslateBool("WidowControl", "widctlpar", "nowidctlpar", false);
            Translate("PageBreakBefore", "pagebb");
            Translate("KeepTogether", "keep");
            Translate("KeepWithNext", "keepn");
            Translate("FirstLineIndent", "fi");

            Translate("LeftIndent", "li");
            Translate("LeftIndent", "lin");

            Translate("RightIndent", "ri");
            Translate("RightIndent", "rin");

            object ol = GetValueAsIntended("OutlineLevel");
            if (ol != null && ((OutlineLevel)ol) != OutlineLevel.BodyText)
                Translate("OutlineLevel", "outlinelevel");

            Unit lineSpc = (Unit)GetValueAsIntended("LineSpacing");
            LineSpacingRule lineSpcRule = (LineSpacingRule)GetValueAsIntended("LineSpacingRule");

            switch (lineSpcRule)
            {
                case LineSpacingRule.Exactly: // A bit strange, but follows the RTF specification:
                    _rtfWriter.WriteControl("sl", ToTwips(-lineSpc.Point));
                    break;

                case LineSpacingRule.AtLeast:
                    Translate("LineSpacing", "sl");
                    break;

                case LineSpacingRule.Multiple:
                    _rtfWriter.WriteControl("sl", ToRtfUnit(lineSpc, RtfUnit.Lines));
                    break;

                case LineSpacingRule.Double:
                    _rtfWriter.WriteControl("sl", 480); // equals 12 * 2 * 20 (Standard line height * 2  in twips)
                    break;

                case LineSpacingRule.OnePtFive:
                    _rtfWriter.WriteControl("sl", 360); // equals 12 * 1.5 * 20 (Standard lineheight * 1.5  in twips)
                    break;
            }
            Translate("LineSpacingRule", "slmult");
            object shad = GetValueAsIntended("Shading");
            if (shad != null)
                new ShadingRenderer((DocumentObject)shad, _docRenderer).Render();

            object font = GetValueAsIntended("Font");
            if (font != null)
                RendererFactory.CreateRenderer((Font)font, _docRenderer).Render();

            object brdrs = GetValueAsIntended("Borders");
            if (brdrs != null)
            {
                BordersRenderer brdrsRndrr = new BordersRenderer((Borders)brdrs, _docRenderer);
                brdrsRndrr.ParentFormat = _format;
                brdrsRndrr.Render();
            }

            object tabStops = GetValueAsIntended("TabStops");
            if (tabStops != null)
                RendererFactory.CreateRenderer((TabStops)tabStops, _docRenderer).Render();

            // TODO: ListInfo is still under construction.
            object listInfo = GetValueAsIntended("ListInfo");
            if (listInfo != null)
            {
                int nr = ListInfoOverrideRenderer.GetListNumber((ListInfo)listInfo);
                if (nr > 0)
                    _rtfWriter.WriteControl("ls", nr);
            }
        }

        readonly ParagraphFormat _format;
    }
}
