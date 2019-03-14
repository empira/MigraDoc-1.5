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
using MigraDoc.DocumentObjectModel.Fields;

namespace MigraDoc.RtfRendering
{
    /// <summary>
    /// Renders an information field to RTF.
    /// </summary>
    internal class InfoFieldRenderer : FieldRenderer
    {
        internal InfoFieldRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer)
            : base(domObj, docRenderer)
        {
            _infoField = domObj as InfoField;
        }

        /// <summary>
        /// Renders an InfoField to RTF.
        /// </summary>
        internal override void Render()
        {
            StartField();
            _rtfWriter.WriteText("INFO ");
            switch (_infoField.Name.ToLower())
            {
                case "author":
                    _rtfWriter.WriteText("Author");
                    break;

                case "title":
                    _rtfWriter.WriteText("Title");
                    break;

                case "keywords":
                    _rtfWriter.WriteText("Keywords");
                    break;

                case "subject":
                    _rtfWriter.WriteText("Subject");
                    break;
            }
            EndField();
        }

        /// <summary>
        /// Gets the requested document info if available.
        /// </summary>
        protected override string GetFieldResult()
        {
            Document doc = _infoField.Document;
            if (!doc.IsNull("Info." + _infoField.Name))
                return doc.Info.GetValue(_infoField.Name) as string;

            return "";
        }

        readonly InfoField _infoField;
    }
}
