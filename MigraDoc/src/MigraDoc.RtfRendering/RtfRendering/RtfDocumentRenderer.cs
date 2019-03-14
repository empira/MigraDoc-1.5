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
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Internals;
using MigraDoc.DocumentObjectModel.Visitors;

namespace MigraDoc.RtfRendering
{
    /// <summary>
    /// Class to render a MigraDoc document to RTF format.
    /// </summary>
    public class RtfDocumentRenderer : RendererBase
    {
        /// <summary>
        /// Initializes a new instance of the DocumentRenderer class.
        /// </summary>
        public RtfDocumentRenderer()
        { }

        /// <summary>
        /// This function is declared only for technical reasons!
        /// </summary>
        internal override void Render()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Renders a MigraDoc document to the specified file.
        /// </summary>
        public void Render(Document doc, string file, string workingDirectory)
        {
            StreamWriter strmWrtr = null;
            try
            {
                _document = doc;
                _docObject = doc;
                _workingDirectory = workingDirectory;
                string path = file;
                if (workingDirectory != null)
                    path = Path.Combine(workingDirectory, file);

                strmWrtr = new StreamWriter(path, false, System.Text.Encoding.Default);
                _rtfWriter = new RtfWriter(strmWrtr);
                WriteDocument();
            }
            finally
            {
                if (strmWrtr != null)
                {
                    strmWrtr.Flush();
                    strmWrtr.Close();
                }
            }
        }

        /// <summary>
        /// Renders a MigraDoc document to the specified stream.
        /// </summary>
        public void Render(Document document, Stream stream, string workingDirectory)
        {
            Render(document, stream, true, workingDirectory);
        }

        /// <summary>
        /// Renders a MigraDoc document to the specified stream.
        /// </summary>
        public void Render(Document document, Stream stream, bool closeStream, string workingDirectory)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (document.UseCmykColor)
                throw new InvalidOperationException("Cannot create RTF document with CMYK colors.");

            StreamWriter strmWrtr = null;
            try
            {
                strmWrtr = new StreamWriter(stream, System.Text.Encoding.Default);
                _document = document;
                _docObject = document;
                _workingDirectory = workingDirectory;
                _rtfWriter = new RtfWriter(strmWrtr);
                WriteDocument();
            }
            finally
            {
                if (strmWrtr != null)
                {
                    strmWrtr.Flush();
                    if (stream != null)
                    {
                        if (closeStream)
                            strmWrtr.Close();
                        else
                            stream.Position = 0; // Reset the stream position if the stream is kept open.
                    }
                }
            }
        }

        /// <summary>
        /// Renders a MigraDoc to Rtf and returns the result as string.
        /// </summary>
        public string RenderToString(Document document, string workingDirectory)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (document.UseCmykColor)
                throw new InvalidOperationException("Cannot create RTF document with CMYK colors.");

            _document = document;
            _docObject = document;
            _workingDirectory = workingDirectory;
            StringWriter writer = null;
            try
            {
                writer = new StringWriter();
                _rtfWriter = new RtfWriter(writer);
                WriteDocument();
                writer.Flush();
                return writer.GetStringBuilder().ToString();
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Renders a MigraDoc document with help of the internal RtfWriter.
        /// </summary>
        private void WriteDocument()
        {
            if (Document.EmbeddedFiles.Count > 0)
                throw new InvalidOperationException("Embedded files are not supported in RTF documents.");

            RtfFlattenVisitor flattener = new RtfFlattenVisitor();
            flattener.Visit(_document);
            Prepare();
            _rtfWriter.StartContent();
            RenderHeader();
            RenderDocumentArea();
            _rtfWriter.EndContent();
        }

        /// <summary>
        /// Prepares this renderer by collecting Information for font and color table.
        /// </summary>
        private void Prepare()
        {
            _fontList.Clear();
            //Fonts 
            _fontList.Add("Symbol");
            _fontList.Add("Wingdings");
            _fontList.Add("Courier New");

            _colorList.Clear();
            _colorList.Add(Colors.Black);//!!necessary for borders!!
            _listList.Clear();
            ListInfoRenderer.Clear();
            ListInfoOverrideRenderer.Clear();
            CollectTables(_document);
        }

        /// <summary>
        /// Renders the RTF Header.
        /// </summary>
        private void RenderHeader()
        {
            _rtfWriter.WriteControl("rtf", 1);
            _rtfWriter.WriteControl("ansi");
            _rtfWriter.WriteControl("ansicpg", 1252);
            _rtfWriter.WriteControl("deff", 0);//default font

            //Document properties can occur before and between the header tables.

            RenderFontTable();
            RenderColorTable();
            RenderStyles();
            //Lists are not yet implemented.
            RenderListTable();
        }

        /// <summary>
        /// Fills the font, color and (later!) list hashtables so they can be rendered and used by other renderers.
        /// </summary>
        private void CollectTables(DocumentObject dom)
        {
            ValueDescriptorCollection vds = Meta.GetMeta(dom).ValueDescriptors;
            int count = vds.Count;
            for (int idx = 0; idx < count; idx++)
            {
                ValueDescriptor vd = vds[idx];
                if (!vd.IsRefOnly && !vd.IsNull(dom))
                {
                    if (vd.ValueType == typeof(Color))
                    {
                        Color clr = (Color)vd.GetValue(dom, GV.ReadWrite);
                        clr = clr.GetMixedTransparencyColor();
                        if (!_colorList.Contains(clr))
                            _colorList.Add(clr);
                    }
                    else if (vd.ValueType == typeof(Font))
                    {
                        Font fnt = vd.GetValue(dom, GV.ReadWrite) as Font; //ReadOnly
                        if (!fnt.IsNull("Name") && !_fontList.Contains(fnt.Name))
                            _fontList.Add(fnt.Name);
                    }
                    else if (vd.ValueType == typeof(ListInfo))
                    {
                        ListInfo lst = vd.GetValue(dom, GV.ReadWrite) as ListInfo; //ReadOnly
                        if (!_listList.Contains(lst))
                            _listList.Add(lst);
                    }
                    if (typeof(DocumentObject).IsAssignableFrom(vd.ValueType))
                    {
                        CollectTables(vd.GetValue(dom, GV.ReadWrite) as DocumentObject); //ReadOnly
                        if (typeof(DocumentObjectCollection).IsAssignableFrom(vd.ValueType))
                        {
                            DocumentObjectCollection coll = vd.GetValue(dom, GV.ReadWrite) as DocumentObjectCollection; //ReadOnly
                            if (coll != null)
                            {
                                foreach (DocumentObject obj in coll)
                                {
                                    // SeriesCollection may contain null values.
                                    if (obj != null)
                                        CollectTables(obj);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Renders the font hashtable within the RTF header.
        /// </summary>
        private void RenderFontTable()
        {
            if (_fontList.Count == 0)
                return;

            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("fonttbl");
            for (int idx = 0; idx < _fontList.Count; ++idx)
            {
                _rtfWriter.StartContent();
                string name = (string)_fontList[idx];
                _rtfWriter.WriteControl("f", idx);
                System.Drawing.Font font = new System.Drawing.Font(name, 12); //any size
                _rtfWriter.WriteControl("fcharset", (int)font.GdiCharSet);
                _rtfWriter.WriteText(name);
                _rtfWriter.WriteSeparator();
                _rtfWriter.EndContent();
            }
            _rtfWriter.EndContent();
        }

        /// <summary>
        /// Renders the color hashtable within the RTF header.
        /// </summary>
        private void RenderColorTable()
        {
            if (_colorList.Count == 0)
                return;

            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("colortbl");
            //this would indicate index 0 as auto color:
            //this.rtfWriter.WriteSeparator();
            //left away cause there is no auto color in MigraDoc.
            foreach (object obj in _colorList)
            {
                Color color = (Color)obj;
                _rtfWriter.WriteControl("red", (int)color.R);
                _rtfWriter.WriteControl("green", (int)color.G);
                _rtfWriter.WriteControl("blue", (int)color.B);
                _rtfWriter.WriteSeparator();
            }
            _rtfWriter.EndContent();
        }

        /// <summary>
        /// Gets the font table index for the specified font name.
        /// </summary>
        internal int GetFontIndex(string fontName)
        {
            if (_fontList.Contains(fontName))
                return (int)_fontList.IndexOf(fontName);

            //development purpose exception
            throw new ArgumentException("Font does not exist in this document's font table.", "fontName");
        }

        /// <summary>
        /// Gets the color table index for the specified color.
        /// </summary>
        internal int GetColorIndex(Color color)
        {
            Color clr = color.GetMixedTransparencyColor();
            int idx = (int)_colorList.IndexOf(clr);
            //development purpose exception
            if (idx < 0)
                throw new ArgumentException("Color does not exist in this document's color table.", "color");
            return idx;
        }

        /// <summary>
        /// Gets the style index for the specified color.
        /// </summary>
        internal int GetStyleIndex(string styleName)
        {
            return _document.Styles.GetIndex(styleName);
        }

        /// <summary>
        /// Renders styles as part of the RTF header.
        /// </summary>
        private void RenderStyles()
        {
            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("stylesheet");
            foreach (Style style in _document.Styles)
            {
                RendererFactory.CreateRenderer(style, this).Render();
            }
            _rtfWriter.EndContent();
        }

        /// <summary>
        /// Renders the list hashtable within the RTF header.
        /// </summary>
        private void RenderListTable()
        {
            if (_listList.Count == 0)
                return;

            _rtfWriter.StartContent();
            _rtfWriter.WriteControlWithStar("listtable");
            foreach (object obj in _listList)
            {
                ListInfo lst = (ListInfo)obj;
                ListInfoRenderer lir = new ListInfoRenderer(lst, this);
                lir.Render();
            }
            _rtfWriter.EndContent();

            _rtfWriter.StartContent();
            _rtfWriter.WriteControlWithStar("listoverridetable");
            foreach (object obj in _listList)
            {
                ListInfo lst = (ListInfo)obj;
                ListInfoOverrideRenderer lir =
                    new ListInfoOverrideRenderer(lst, this);
                lir.Render();
            }
            _rtfWriter.EndContent();
        }

        /// <summary>
        /// Renders the RTF document area, which is all except the header.
        /// </summary>
        private void RenderDocumentArea()
        {
            RenderInfo();
            RenderDocumentFormat();
            RenderGlobalPorperties();
            foreach (Section sect in _document.Sections)
            {
                RendererFactory.CreateRenderer(sect, this).Render();
            }
        }

        /// <summary>
        /// Renders global document properties, such as mirror margins and unicode treatment.
        /// Note that a section specific margin mirroring does not work in Word.
        /// </summary>
        private void RenderGlobalPorperties()
        {
            _rtfWriter.WriteControl("viewkind", 4);
            _rtfWriter.WriteControl("uc", 1);

            //Em4-Space doesn't work without this:
            _rtfWriter.WriteControl("lnbrkrule");

            //Footnotes only, no endnotes:
            _rtfWriter.WriteControl("fet", 0);

            //Enables title pages as (FirstpageHeader):
            _rtfWriter.WriteControl("facingp");

            //Space between paragraphs as maximum between space after and space before:
            _rtfWriter.WriteControl("htmautsp");

            //Word cannot realize the mirror margins property for single sections,
            //although rtf control words exist for this purpose. 
            //Thus, the mirror margins property is set globally if it's true for the first section.
            Section sec = _document.Sections.First as Section;
            if (sec != null)
            {
                if (!sec.PageSetup.IsNull("MirrorMargins") && sec.PageSetup.MirrorMargins)
                    _rtfWriter.WriteControl("margmirror");
            }
        }

        /// <summary>
        /// Renders the document format such as standard tab stops and footnote settings.
        /// </summary>
        private void RenderDocumentFormat()
        {
            Translate("DefaultTabStop", "deftab");
            Translate("FootnoteNumberingRule", "ftn");
            Translate("FootnoteLocation", "ftn", RtfUnit.Undefined, "bj", false);
            Translate("FootnoteNumberStyle", "ftnn");
            Translate("FootnoteStartingNumber", "ftnstart");
        }

        /// <summary>
        /// Renders footnote properties for a section. (not part of the rtf specification, but necessary for Word)
        /// </summary>
        internal void RenderSectionProperties()
        {
            Translate("FootnoteNumberingRule", "sftn");
            Translate("FootnoteLocation", "sftn", RtfUnit.Undefined, "bj", false);
            Translate("FootnoteNumberStyle", "sftnn");
            Translate("FootnoteStartingNumber", "sftnstart");
        }

        /// <summary>
        /// Renders the document information of title, author etc..
        /// </summary>
        private void RenderInfo()
        {
            if (_document.IsNull("Info"))
                return;

            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("f", 2); // Second font is Courier New. See Prepare().
            _rtfWriter.WriteControl("info");
            DocumentInfo info = _document.Info;
            if (!info.IsNull("Title"))
            {
                _rtfWriter.StartContent();
                _rtfWriter.WriteControl("title");
                _rtfWriter.WriteText(info.Title);
                _rtfWriter.EndContent();
            }
            if (!info.IsNull("Subject"))
            {
                _rtfWriter.StartContent();
                _rtfWriter.WriteControl("subject");
                _rtfWriter.WriteText(info.Subject);
                _rtfWriter.EndContent();
            }
            if (!info.IsNull("Author"))
            {
                _rtfWriter.StartContent();
                _rtfWriter.WriteControl("author");
                _rtfWriter.WriteText(info.Author);
                _rtfWriter.EndContent();
            }
            if (!info.IsNull("Keywords"))
            {
                _rtfWriter.StartContent();
                _rtfWriter.WriteControl("keywords");
                _rtfWriter.WriteText(info.Keywords);
                _rtfWriter.EndContent();
            }
            _rtfWriter.EndContent();
        }

        /// <summary>
        /// Gets the MigraDoc document that is currently rendered.
        /// </summary>
        internal Document Document
        {
            get { return _document; }
        }
        private Document _document;

        /// <summary>
        /// Gets the RtfWriter the document is rendered with.
        /// </summary>
        internal RtfWriter RtfWriter
        {
            get { return _rtfWriter; }
        }

        internal string WorkingDirectory
        {
            get { return _workingDirectory; }
        }
        string _workingDirectory;

        private readonly List<Color> _colorList = new List<Color>();
        private readonly List<string> _fontList = new List<string>();
        private readonly List<ListInfo> _listList = new List<ListInfo>();
    }
}
