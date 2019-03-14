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
using System.Reflection;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering.Resources;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Provides the functionality to convert a MigraDoc document into PDF.
    /// </summary>
    public class PdfDocumentRenderer
    {
        /// <summary>
        /// Initializes a new instance of the PdfDocumentRenderer class.
        /// </summary>
        public PdfDocumentRenderer()
        {
            //_unicode = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDocumentRenderer"/> class.
        /// </summary>
        /// <param name="unicode">If true Unicode encoding is used for all text. If false, WinAnsi encoding is used.</param>
        public PdfDocumentRenderer(bool unicode)
        {
            _unicode = unicode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDocumentRenderer" /> class.
        /// </summary>
        /// <param name="unicode">If true Unicode encoding is used for all text. If false, WinAnsi encoding is used.</param>
        /// <param name="fontEmbedding">Obsolete parameter.</param>
        [Obsolete("Must not specify an embedding option anymore.")]
        public PdfDocumentRenderer(bool unicode, PdfFontEmbedding fontEmbedding)
        {
            _unicode = unicode;
        }

        /// <summary>
        /// Gets a value indicating whether the text is rendered as Unicode.
        /// </summary>
        public bool Unicode
        {
            get { return _unicode; }
        }
        readonly bool _unicode;

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }
        string _language = String.Empty;

        /// <summary>
        /// Set the MigraDoc document to be rendered by this printer.
        /// </summary>
        public Document Document
        {
            set
            {
                _document = null;
                value.BindToRenderer(this);
                _document = value;
            }
        }
        Document _document;

        /// <summary>
        /// Gets or sets a document renderer.
        /// </summary>
        /// <remarks>
        /// A document renderer is automatically created and prepared
        /// when printing before this property was set.
        /// </remarks>
        public DocumentRenderer DocumentRenderer
        {
            get
            {
                if (_documentRenderer == null)
                    PrepareDocumentRenderer();
                return _documentRenderer;
            }
            set { _documentRenderer = value; }
        }
        DocumentRenderer _documentRenderer;

        void PrepareDocumentRenderer()
        {
            PrepareDocumentRenderer(false);
        }

        void PrepareDocumentRenderer(bool prepareCompletely)
        {
            if (_document == null)
#if !NETFX_CORE
                throw new InvalidOperationException(Messages2.PropertyNotSetBefore("DocumentRenderer", MethodBase.GetCurrentMethod().Name));
#else
                throw new InvalidOperationException(Messages2.PropertyNotSetBefore("DocumentRenderer", "PrepareDocumentRenderer"));
#endif

            if (_documentRenderer == null)
            {
                _documentRenderer = new DocumentRenderer(_document);
                _documentRenderer.WorkingDirectory = _workingDirectory;
            }
            if (prepareCompletely && _documentRenderer.FormattedDocument == null)
            {
                _documentRenderer.PrepareDocument();
            }
        }

        /// <summary>
        /// Renders the document into a PdfDocument containing all pages of the document.
        /// </summary>
        public void RenderDocument()
        {
#if true
            PrepareRenderPages();
#else
            if (this.documentRenderer == null)
                PrepareDocumentRenderer();

            if (this.pdfDocument == null)
            {
                this.pdfDocument = new PdfDocument();
                this.pdfDocument.Info.Creator = VersionInfo.Creator;
            }

            WriteDocumentInformation();
#endif
            RenderPages(1, _documentRenderer.FormattedDocument.PageCount);
        }

        /// <summary>
        /// Renders the document into a PdfDocument containing all pages of the document.
        /// </summary>
        public void PrepareRenderPages()
        {
            //if (this.documentRenderer == null)
            PrepareDocumentRenderer(true);

            if (_pdfDocument == null)
            {
                _pdfDocument = CreatePdfDocument();
                if (_document.UseCmykColor)
                    _pdfDocument.Options.ColorMode = PdfColorMode.Cmyk;
            }

            // Add embedded files, that are defined in MigraDoc _document to PDFsharp _pdfDocument.
            foreach (EmbeddedFile embeddedFile in _document.EmbeddedFiles)
            {
                _pdfDocument.AddEmbeddedFile(embeddedFile.Name, embeddedFile.Path);
            }

            WriteDocumentInformation();
            //RenderPages(1, this.documentRenderer.FormattedDocument.PageCount);
        }

        /// <summary>
        /// Gets the count of pages.
        /// </summary>
        public int PageCount
        {
            get { return _documentRenderer.FormattedDocument.PageCount; }
        }

        /// <summary>
        /// Saves the PdfDocument to the specified path. If a file already exists, it will be overwritten.
        /// </summary>
        public void Save(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path == "")
                throw new ArgumentException("PDF file Path must not be empty");

            if (_workingDirectory != null)
                path = Path.Combine(_workingDirectory, path);

            _pdfDocument.Save(path);
        }

        /// <summary>
        /// Saves the PDF document to the specified stream.
        /// </summary>
        public void Save(Stream stream, bool closeStream)
        {
            _pdfDocument.Save(stream, closeStream);
        }

        /// <summary>
        /// Renders the specified page range.
        /// </summary>
        /// <param name="startPage">The first page to print.</param>
        /// <param name="endPage">The last page to print</param>
        public void RenderPages(int startPage, int endPage)
        {
            if (startPage < 1)
                throw new ArgumentOutOfRangeException("startPage");

            if (endPage > _documentRenderer.FormattedDocument.PageCount)
                throw new ArgumentOutOfRangeException("endPage");

            if (_documentRenderer == null)
                PrepareDocumentRenderer();

            if (_pdfDocument == null)
                _pdfDocument = CreatePdfDocument();

            _documentRenderer._printDate = DateTime.Now;
            for (int pageNr = startPage; pageNr <= endPage; ++pageNr)
            {
                PdfPage pdfPage = _pdfDocument.AddPage();
                PageInfo pageInfo = _documentRenderer.FormattedDocument.GetPageInfo(pageNr);
                pdfPage.Width = pageInfo.Width;
                pdfPage.Height = pageInfo.Height;
                pdfPage.Orientation = pageInfo.Orientation;

                using (XGraphics gfx = XGraphics.FromPdfPage(pdfPage))
                {
                    gfx.MUH = _unicode ? PdfFontEncoding.Unicode : PdfFontEncoding.WinAnsi;
                    _documentRenderer.RenderPage(gfx, pageNr);
                }
            }
        }

        /// <summary>
        /// Gets or sets a working directory for the printing process.
        /// </summary>
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set { _workingDirectory = value; }
        }
        string _workingDirectory;

        /// <summary>
        /// Gets or sets the PDF document to render on.
        /// </summary>
        /// <remarks>A PDF document in memory is automatically created when printing before this property was set.</remarks>
        public PdfDocument PdfDocument
        {
            get { return _pdfDocument; }
            set { _pdfDocument = value; }
        }
        PdfDocument _pdfDocument;

        /// <summary>
        /// Writes document information like author and subject to the PDF document.
        /// </summary>
        public void WriteDocumentInformation()
        {
            if (!_document.IsNull("Info"))
            {
                DocumentInfo docInfo = _document.Info;
                PdfDocumentInformation pdfInfo = _pdfDocument.Info;

                if (!docInfo.IsNull("Author"))
                    pdfInfo.Author = docInfo.Author;

                if (!docInfo.IsNull("Keywords"))
                    pdfInfo.Keywords = docInfo.Keywords;

                if (!docInfo.IsNull("Subject"))
                    pdfInfo.Subject = docInfo.Subject;

                if (!docInfo.IsNull("Title"))
                    pdfInfo.Title = docInfo.Title;
            }
        }

        /// <summary>
        /// Creates a new PDF document.
        /// </summary>
        PdfDocument CreatePdfDocument()
        {
            PdfDocument document = new PdfDocument();
            document.Info.Creator = VersionInfo.Creator;
            if (!String.IsNullOrEmpty(_language))
                document.Language = _language;
            return document;
        }
    }
}
