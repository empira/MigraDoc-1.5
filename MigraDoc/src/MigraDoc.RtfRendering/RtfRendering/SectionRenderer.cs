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
    /// Class to render a Section to RTF.
    /// </summary>
    internal class SectionRenderer : RendererBase
    {
        internal SectionRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer)
            : base(domObj, docRenderer)
        {
            _section = domObj as Section;
        }

        /// <summary>
        /// Renders a section to RTF
        /// </summary>
        internal override void Render()
        {
            _useEffectiveValue = true;

            Sections secs = DocumentRelations.GetParent(_section) as Sections;
            if (_section != secs.First)
            {
                _rtfWriter.WriteControl("pard");
                _rtfWriter.WriteControl("sect");
            }
            _rtfWriter.WriteControl("sectd");

            //Rendering some footnote attributes:
            _docRenderer.RenderSectionProperties();

            object pageStp = _section.PageSetup;
            if (pageStp != null)
                RendererFactory.CreateRenderer((PageSetup)pageStp, _docRenderer).Render();

            object hdrs = GetValueAsIntended("Headers");
            if (hdrs != null)
            {
                HeadersFootersRenderer hfr = new HeadersFootersRenderer(hdrs as HeadersFooters, _docRenderer);
                // PageSetup has to be set here, because HeaderFooter could be from a different section than PageSetup.
                hfr.PageSetup = (PageSetup)pageStp;
                hfr.Render();
            }

            object ftrs = GetValueAsIntended("Footers");
            if (ftrs != null)
            {
                HeadersFootersRenderer hfr = new HeadersFootersRenderer(ftrs as HeadersFooters, _docRenderer);
                hfr.PageSetup = (PageSetup)pageStp;
                hfr.Render();
            }

            if (!_section.IsNull("Elements"))
            {
                foreach (DocumentObject docObj in _section.Elements)
                {
                    RendererBase rndrr = RendererFactory.CreateRenderer(docObj, _docRenderer);
                    if (rndrr != null)
                        rndrr.Render();
                }
            }
        }

        readonly Section _section;
    }
}
