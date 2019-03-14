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

using MigraDoc.DocumentObjectModel.Internals;

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// Represents text in a paragraph.
    /// </summary>
    public class Text : DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the Text class.
        /// </summary>
        public Text()
        { }

        /// <summary>
        /// Initializes a new instance of the Text class with the specified parent.
        /// </summary>
        internal Text(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Initializes a new instance of the Text class with a string as paragraph content.
        /// </summary>
        public Text(string content)
            : this()
        {
            Content = content;
        }

        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Text Clone()
        {
            return (Text)DeepCopy();
        }

        /// <summary>
        /// Gets or sets the text content.
        /// </summary>
        public string Content
        {
            get { return _content.Value; }
            set { _content.Value = value; }
        }
        [DV]
        internal NString _content = NString.NullValue;

        /// <summary>
        /// Converts Text into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            string text = DdlEncoder.StringToText(_content.Value);
            // To make DDL more readable write soft hypens as keywords.
            text = text.Replace(new string((char)173, 1), "\\-");
            serializer.Write(text);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Text))); }
        }
        static Meta _meta;
    }
}
