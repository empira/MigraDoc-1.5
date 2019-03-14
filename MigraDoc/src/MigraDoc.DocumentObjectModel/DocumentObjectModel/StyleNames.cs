#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Stefan Lange
//   Klaus Potzesny
//   David Stephensen
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

#pragma warning disable 1591

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// Enumerates the predefined style names.
    /// </summary>
    public class StyleNames
    {
        public const string DefaultParagraphFont = "DefaultParagraphFont";
        public const string Normal = "Normal";
        public const string Heading1 = "Heading1";
        public const string Heading2 = "Heading2";
        public const string Heading3 = "Heading3";
        public const string Heading4 = "Heading4";
        public const string Heading5 = "Heading5";
        public const string Heading6 = "Heading6";
        public const string Heading7 = "Heading7";
        public const string Heading8 = "Heading8";
        public const string Heading9 = "Heading9";
        // TODO Verify if "List" exists with Word.
        public const string List = "List";
        public const string Footnote = "Footnote";
        public const string Header = "Header";
        public const string Footer = "Footer";
        public const string Hyperlink = "Hyperlink";
        public const string InvalidStyleName = "InvalidStyleName";
    }
}
