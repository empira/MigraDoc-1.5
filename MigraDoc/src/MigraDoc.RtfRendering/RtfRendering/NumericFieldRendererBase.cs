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

using System.Diagnostics;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Fields;
using MigraDoc.RtfRendering.Resources;

namespace MigraDoc.RtfRendering
{
    /// <summary>
    /// Base Class to render numeric fields.
    /// </summary>
    internal abstract class NumericFieldRendererBase : FieldRenderer
    {
        internal NumericFieldRendererBase(DocumentObject domObj, RtfDocumentRenderer docRenderer)
            : base(domObj, docRenderer)
        {
            _numericField = domObj as NumericFieldBase;
        }

        /// <summary>
        /// Translates the number format to RTF.
        /// </summary>
        protected void TranslateFormat()
        {
            switch (_numericField.Format)
            {
                case "":
                    break;

                case "ROMAN":
                    _rtfWriter.WriteText(@" \*ROMAN");
                    break;

                case "roman":
                    _rtfWriter.WriteText(@" \*roman");
                    break;

                case "ALPHABETIC":
                    _rtfWriter.WriteText(@" \*ALPHABETIC");
                    break;

                case "alphabetic":
                    _rtfWriter.WriteText(@" \*alphabetic");
                    break;

                default:
                    Debug.WriteLine(Messages2.InvalidNumericFieldFormat(_numericField.Format), "warning");
                    break;
            }
        }
        private readonly NumericFieldBase _numericField;
    }
}
