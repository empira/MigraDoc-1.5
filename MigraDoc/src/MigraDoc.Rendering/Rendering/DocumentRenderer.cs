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
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel.Visitors;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering.Resources;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Provides methods to render the document or single parts of it to a XGraphics object.
    /// </summary>
    /// <remarks>
    /// One prepared instance of this class can serve to render several output formats.
    /// </remarks>
    public class DocumentRenderer
    {
        /// <summary>
        /// Initializes a new instance of the DocumentRenderer class.
        /// </summary>
        /// <param name="document">The migradoc document to render.</param>
        public DocumentRenderer(Document document)
        {
            _document = document;
        }

        /// <summary>
        /// Prepares this instance for rendering.
        /// </summary>
        public void PrepareDocument()
        {
            PdfFlattenVisitor visitor = new PdfFlattenVisitor();
            visitor.Visit(_document);
            _previousListNumbers = new Dictionary<ListType, int>(3);
            _previousListNumbers[ListType.NumberList1] = 0;
            _previousListNumbers[ListType.NumberList2] = 0;
            _previousListNumbers[ListType.NumberList3] = 0;
            _formattedDocument = new FormattedDocument(_document, this);
            //REM: Size should not be necessary in this case.
#if true
            XGraphics gfx = XGraphics.CreateMeasureContext(new XSize(2000, 2000), XGraphicsUnit.Point, XPageDirection.Downwards);
#else
#if GDI
      XGraphics gfx = XGraphics.FromGraphics(Graphics.FromHwnd(IntPtr.Zero), new XSize(2000, 2000));
#endif
#if WPF
      XGraphics gfx = XGraphics.FromDrawingContext(null, new XSize(2000, 2000), XGraphicsUnit.Point);
#endif
#endif
            //      _previousListNumber = int.MinValue;
            //gfx.MUH = _unicode;
            //gfx.MFEH = _fontEmbedding;

            _previousListInfo = null;
            _formattedDocument.Format(gfx);
        }

        /// <summary>
        /// Occurs while the document is being prepared (can be used to show a progress bar).
        /// </summary>
        public event PrepareDocumentProgressEventHandler PrepareDocumentProgress;

        /// <summary>
        /// Allows applications to display a progress indicator while PrepareDocument() is being executed.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maximum"></param>
        internal virtual void OnPrepareDocumentProgress(int value, int maximum)
        {
            if (PrepareDocumentProgress != null)
            {
                // Invokes the delegates. 
                PrepareDocumentProgressEventArgs e = new PrepareDocumentProgressEventArgs(value, maximum);
                PrepareDocumentProgress(this, e);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance supports PrepareDocumentProgress.
        /// </summary>
        public bool HasPrepareDocumentProgress
        {
            get { return PrepareDocumentProgress != null; }
        }

        /// <summary>
        /// Gets the formatted document of this instance.
        /// </summary>
        public FormattedDocument FormattedDocument
        {
            get { return _formattedDocument; }
        }
        FormattedDocument _formattedDocument;

        /// <summary>
        /// Renders a MigraDoc document to the specified graphics object.
        /// </summary>
        public void RenderPage(XGraphics gfx, int page)
        {
            RenderPage(gfx, page, PageRenderOptions.All);
        }

        /// <summary>
        /// Renders a MigraDoc document to the specified graphics object.
        /// </summary>
        public void RenderPage(XGraphics gfx, int page, PageRenderOptions options)
        {
            if (_formattedDocument.IsEmptyPage(page))
                return;

            FieldInfos fieldInfos = _formattedDocument.GetFieldInfos(page);

            fieldInfos.Date = _printDate != DateTime.MinValue ? _printDate : DateTime.Now;

            if ((options & PageRenderOptions.RenderHeader) == PageRenderOptions.RenderHeader)
                RenderHeader(gfx, page);
            if ((options & PageRenderOptions.RenderFooter) == PageRenderOptions.RenderFooter)
                RenderFooter(gfx, page);

            if ((options & PageRenderOptions.RenderContent) == PageRenderOptions.RenderContent)
            {
                RenderInfo[] renderInfos = _formattedDocument.GetRenderInfos(page);
                //foreach (RenderInfo renderInfo in renderInfos)
                int count = renderInfos.Length;
                for (int idx = 0; idx < count; idx++)
                {
                    RenderInfo renderInfo = renderInfos[idx];
                    Renderer renderer = Renderer.Create(gfx, this, renderInfo, fieldInfos);
                    renderer.Render();
                }
            }
        }

        /// <summary>
        /// Gets the document objects that get rendered on the specified page.
        /// </summary>
        public DocumentObject[] GetDocumentObjectsFromPage(int page)
        {
            RenderInfo[] renderInfos = _formattedDocument.GetRenderInfos(page);
            int count = renderInfos != null ? renderInfos.Length : 0;
            DocumentObject[] documentObjects = new DocumentObject[count];
            for (int idx = 0; idx < count; idx++)
                documentObjects[idx] = renderInfos[idx].DocumentObject;
            return documentObjects;
        }

        /// <summary>
        /// Gets the render information for document objects that get rendered on the specified page.
        /// </summary>
        public RenderInfo[] GetRenderInfoFromPage(int page)
        {
            return _formattedDocument.GetRenderInfos(page);
        }

        /// <summary>
        /// Renders a single object to the specified graphics object at the given point.
        /// </summary>
        /// <param name="graphics">The graphics object to render on.</param>
        /// <param name="xPosition">The left position of the rendered object.</param>
        /// <param name="yPosition">The top position of the rendered object.</param>
        /// <param name="width">The width.</param>
        /// <param name="documentObject">The document object to render. Can be paragraph, table, or shape.</param>
        /// <remarks>This function is still in an experimental state.</remarks>
        public void RenderObject(XGraphics graphics, XUnit xPosition, XUnit yPosition, XUnit width, DocumentObject documentObject)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            if (documentObject == null)
                throw new ArgumentNullException("documentObject");

            if (!(documentObject is Shape) && !(documentObject is Table) &&
                !(documentObject is Paragraph))
                throw new ArgumentException(Messages2.ObjectNotRenderable, "documentObject");

            Renderer renderer = Renderer.Create(graphics, this, documentObject, null);
            renderer.Format(new Rectangle(xPosition, yPosition, width, double.MaxValue), null);

            RenderInfo renderInfo = renderer.RenderInfo;
            renderInfo.LayoutInfo.ContentArea.X = xPosition;
            renderInfo.LayoutInfo.ContentArea.Y = yPosition;

            renderer = Renderer.Create(graphics, this, renderer.RenderInfo, null);
            renderer.Render();
        }

        /// <summary>
        /// Gets or sets the working directory for rendering.
        /// </summary>
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set { _workingDirectory = value; }
        }
        string _workingDirectory;

        private void RenderHeader(XGraphics graphics, int page)
        {
            FormattedHeaderFooter formattedHeader = _formattedDocument.GetFormattedHeader(page);
            if (formattedHeader == null)
                return;

            Rectangle headerArea = _formattedDocument.GetHeaderArea(page);
            RenderInfo[] renderInfos = formattedHeader.GetRenderInfos();
            FieldInfos fieldInfos = _formattedDocument.GetFieldInfos(page);
            foreach (RenderInfo renderInfo in renderInfos)
            {
                Renderer renderer = Renderer.Create(graphics, this, renderInfo, fieldInfos);
                renderer.Render();
            }
        }

        private void RenderFooter(XGraphics graphics, int page)
        {
            FormattedHeaderFooter formattedFooter = _formattedDocument.GetFormattedFooter(page);
            if (formattedFooter == null)
                return;

            Rectangle footerArea = _formattedDocument.GetFooterArea(page);
            RenderInfo[] renderInfos = formattedFooter.GetRenderInfos();
#if true
#if true
            // The footer is bottom-aligned and grows with its contents. topY specifies the Y position where the footer begins.
            XUnit topY = footerArea.Y + footerArea.Height - RenderInfo.GetTotalHeight(renderInfos);
            // Hack: The purpose of "topY" is unclear, but two paragraphs in the footer will use the same topY and will be rendered at the same position.
            // offsetY specifies the offset (amount of movement) for all footer items. It's the difference between topY and the position calculated for the first item.
            XUnit offsetY = 0;
            bool notFirst = false;

            FieldInfos fieldInfos = _formattedDocument.GetFieldInfos(page);
            foreach (RenderInfo renderInfo in renderInfos)
            {
                Renderer renderer = Renderer.Create(graphics, this, renderInfo, fieldInfos);
                if (!notFirst)
                {
                    offsetY = renderer.RenderInfo.LayoutInfo.ContentArea.Y - topY;
                    notFirst = true;
                }
                XUnit savedY = renderer.RenderInfo.LayoutInfo.ContentArea.Y;
                // Apply offsetY only to items that do not have an absolute position.
                if (renderer.RenderInfo.LayoutInfo.Floating != Floating.None)
                    renderer.RenderInfo.LayoutInfo.ContentArea.Y -= offsetY;
                renderer.Render();
                renderer.RenderInfo.LayoutInfo.ContentArea.Y = savedY;
            }
#else
            // TODO Document the purpose of "topY".
            XUnit topY = footerArea.Y + footerArea.Height - RenderInfo.GetTotalHeight(renderInfos);
            // Hack: The purpose of "topY" is unclear, but two paragraphs in the footer will use the same topY and will be rendered at the same position.
            XUnit offsetY = 0;

            FieldInfos fieldInfos = _formattedDocument.GetFieldInfos(page);
            foreach (RenderInfo renderInfo in renderInfos)
            {
                Renderer renderer = Renderer.Create(graphics, this, renderInfo, fieldInfos);
                XUnit savedY = renderer.RenderInfo.LayoutInfo.ContentArea.Y;
                renderer.RenderInfo.LayoutInfo.ContentArea.Y = topY + offsetY;
                renderer.Render();
                renderer.RenderInfo.LayoutInfo.ContentArea.Y = savedY;
                offsetY += renderer.RenderInfo.LayoutInfo.ContentArea.Height;
            }
#endif
#else
            XUnit topY = footerArea.Y + footerArea.Height - RenderInfo.GetTotalHeight(renderInfos);

            FieldInfos fieldInfos = _formattedDocument.GetFieldInfos(page);
            foreach (RenderInfo renderInfo in renderInfos)
            {
                Renderer renderer = Renderer.Create(graphics, this, renderInfo, fieldInfos);
                XUnit savedY = renderer.RenderInfo.LayoutInfo.ContentArea.Y;
                renderer.RenderInfo.LayoutInfo.ContentArea.Y = topY;
                renderer.Render();
                renderer.RenderInfo.LayoutInfo.ContentArea.Y = savedY;
            }
#endif
        }

        internal void AddOutline(int level, string title, PdfPage destinationPage, XPoint position)
        {
            if (level < 1 || destinationPage == null)
                return;

            PdfDocument document = destinationPage.Owner;

            if (document == null)
                return;

            PdfOutlineCollection outlines = document.Outlines;
            while (--level > 0)
            {
                int count = outlines.Count;
                if (count == 0)
                {
                    // You cannot add empty bookmarks to PDF. So we use blank here.
                    var outline = AddOutline(outlines, " ", destinationPage, position);
                    outlines = outline.Outlines;
                }
                else
                    outlines = outlines[count - 1].Outlines;
            }
            AddOutline(outlines, title, destinationPage, position);
        }

        private PdfOutline AddOutline(PdfOutlineCollection outlines, string title, PdfPage destinationPage, XPoint position)
        {
            var outline = outlines.Add(title, destinationPage, true);
            outline.Left = position.X;
            outline.Top = position.Y;
            return outline;
        }

        internal int NextListNumber(ListInfo listInfo)
        {
            ListType listType = listInfo.ListType;
            bool isNumberList = listType == ListType.NumberList1 ||
              listType == ListType.NumberList2 ||
              listType == ListType.NumberList3;

            int listNumber = int.MinValue;
            if (listInfo == _previousListInfo)
            {
                if (isNumberList)
                    return _previousListNumbers[listType];
                return listNumber;
            }

            //bool listTypeChanged = _previousListInfo == null || _previousListInfo.ListType != listType;

            if (isNumberList)
            {
                listNumber = 1;
                if (/*!listTypeChanged &&*/ (listInfo._continuePreviousList.IsNull || listInfo.ContinuePreviousList))
                    listNumber = _previousListNumbers[listType] + 1;

                _previousListNumbers[listType] = listNumber;
            }

            _previousListInfo = listInfo;
            return listNumber;
        }
        ListInfo _previousListInfo;
        Dictionary<ListType, int> _previousListNumbers;
        private readonly Document _document;
        internal DateTime _printDate = DateTime.MinValue;

        /// <summary>
        /// Arguments for the PrepareDocumentProgressEvent which is called while a document is being prepared (you can use this to display a progress bar).
        /// </summary>
        public class PrepareDocumentProgressEventArgs : EventArgs
        {
            /// <summary>
            /// Indicates the current step reached in document preparation.
            /// </summary>
            public int Value;
            /// <summary>
            /// Indicates the final step in document preparation. The quitient of Value and Maximum can be used to calculate a percentage (e. g. for use in a progress bar).
            /// </summary>
            public int Maximum;

            /// <summary>
            /// Initializes a new instance of the <see cref="PrepareDocumentProgressEventArgs"/> class.
            /// </summary>
            /// <param name="value">The current step in document preparation.</param>
            /// <param name="maximum">The latest step in document preparation.</param>
            public PrepareDocumentProgressEventArgs(int value, int maximum)
            {
                Value = value;
                Maximum = maximum;
            }
        }

        /// <summary>
        /// The event handler that is being called for the PrepareDocumentProgressEvent event.
        /// </summary>
        public delegate void PrepareDocumentProgressEventHandler(object sender, PrepareDocumentProgressEventArgs e);

        internal int ProgressMaximum;
        internal int ProgressCompleted;
    }
}
