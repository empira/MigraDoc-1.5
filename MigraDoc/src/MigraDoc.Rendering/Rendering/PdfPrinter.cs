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

#if DELETED
namespace MigraDoc.Rendering
{
    /// <summary>
    /// Provides the functionality to convert MigraDoc documents into PDF.
    /// </summary>
    [Obsolete("Use class PdfDocumentRenderer.")]  // DELETE: 8/06
    public class PdfPrinter
    {
        /// <summary>
        /// Initializes a new instance of the PdfPrinter class.
        /// </summary>
        public PdfPrinter()
        { }

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
            get { return _documentRenderer; }
            set { _documentRenderer = value; }
        }
        DocumentRenderer _documentRenderer;

        void PrepareDocumentRenderer()
        {
            if (_document == null)
                throw new InvalidOperationException(Messages2.PropertyNotSetBefore("DocumentRenderer", MethodInfo.GetCurrentMethod().Name));

            _documentRenderer = new DocumentRenderer(_document);
            _documentRenderer.WorkingDirectory = this.workingDirectory;
            _documentRenderer.PrepareDocument();
        }

        /// <summary>
        /// Prints a PDF document containing all pages of the document.
        /// </summary>
        public void PrintDocument()
        {
            if (_documentRenderer == null)
                PrepareDocumentRenderer();

            if (_pdfDocument == null)
            {
                _pdfDocument = new PdfDocument();
                _pdfDocument.Info.Creator = VersionInfo.Creator;
            }

            WriteDocumentInformation();
            PrintPages(1, _documentRenderer.FormattedDocument.PageCount);
        }

        /// <summary>
        /// Saves the PDF document to the specified path. If a file already exists, it will be overwritten.
        /// </summary>
        public void Save(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path == "")
                throw new ArgumentException("PDF file Path must not be empty");

            if (this.workingDirectory != null)
                Path.Combine(this.workingDirectory, path);

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
        /// Prints the specified page range.
        /// </summary>
        /// <param name="startPage">The first page to print.</param>
        /// <param name="endPage">The last page to print</param>
        public void PrintPages(int startPage, int endPage)
        {
            if (startPage < 1)
                throw new ArgumentOutOfRangeException("startPage");

            if (endPage > _documentRenderer.FormattedDocument.PageCount)
                throw new ArgumentOutOfRangeException("endPage");

            if (_documentRenderer == null)
                PrepareDocumentRenderer();

            if (_pdfDocument == null)
            {
                _pdfDocument = new PdfDocument();
                _pdfDocument.Info.Creator = VersionInfo.Creator;
            }

            _documentRenderer._printDate = DateTime.Now;
            for (int pageNr = startPage; pageNr <= endPage; ++pageNr)
            {
                PdfPage pdfPage = _pdfDocument.AddPage();
                PageInfo pageInfo = _documentRenderer.FormattedDocument.GetPageInfo(pageNr);
                pdfPage.Width = pageInfo.Width;
                pdfPage.Height = pageInfo.Height;
                pdfPage.Orientation = pageInfo.Orientation;
                _documentRenderer.RenderPage(XGraphics.FromPdfPage(pdfPage), pageNr);
            }
        }

        /// <summary>
        /// Gets or sets a working directory for the printing process.
        /// </summary>
        public string WorkingDirectory
        {
            get { return this.workingDirectory; }
            set { this.workingDirectory = value; }
        }
        string workingDirectory;

        /// <summary>
        /// Gets or sets the PDF document to render on.
        /// </summary>
        /// <remarks>A PDF document in memory is automatically created when printing before this property was set.</remarks>
        public PdfDocument PdfDocument
        {
            get { return _pdfDocument; }
            set { _pdfDocument = value; }
        }

        /// <summary>
        /// Writes document information like author and subject to the PDF document.
        /// </summary>
        private void WriteDocumentInformation()
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
        PdfDocument _pdfDocument;
    }
}
#endif
