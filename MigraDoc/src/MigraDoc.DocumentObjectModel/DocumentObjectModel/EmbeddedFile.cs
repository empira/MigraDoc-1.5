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
    /// EmbeddedFile is used for PDF Documents that shall be embedded in another PDF Document. EmbeddedFile is only supported in PDF.
    /// </summary>
    public class EmbeddedFile : DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the EmbeddedFile class.
        /// </summary>
        public EmbeddedFile()
        { }

        /// <summary>
        /// Initializes a new instance of the EmbeddedFile class.
        /// </summary>
        /// <param name="name">The name used to refer and to entitle the embedded file.</param>
        /// <param name="path">The path of the file to embed.</param>
        public EmbeddedFile(string name, string path)
        {
            Name = name;
            Path = path;
        }

        #region Methods
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name used to refer and to entitle the embedded file.
        /// </summary>
        public string Name
        {
            get { return _name.Value; }
            set { _name.Value = value; }
        }
        [DV]
        internal NString _name = NString.NullValue;

        /// <summary>
        /// Gets or sets the path to the file to embed.
        /// </summary>
        public string Path
        {
            get { return _path.Value; }
            set { _path.Value = value; }
        }
        [DV]
        internal NString _path = NString.NullValue;
        #endregion

        #region Internal
        /// <summary>
        /// Converts Section into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine("\\EmbeddedFile");
            serializer.BeginAttributes();

            if (!_name.IsNull && _name.Value != "")
                serializer.WriteSimpleAttribute("Name", Name);

            if (!_name.IsNull && _name.Value != "")
                serializer.WriteSimpleAttribute("Path", Path);

            serializer.EndAttributes();
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta => _meta ?? (_meta = new Meta(typeof(EmbeddedFile)));

        static Meta _meta;
        #endregion
    }
}
