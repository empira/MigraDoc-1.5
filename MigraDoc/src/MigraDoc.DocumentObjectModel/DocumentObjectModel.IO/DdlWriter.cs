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

using System.IO;
using System.Text;

namespace MigraDoc.DocumentObjectModel.IO
{
    /// <summary>
    /// Represents the MigraDoc DDL writer.
    /// </summary>
    public class DdlWriter
    {
        /// <summary>
        /// Initializes a new instance of the DdlWriter class with the specified Stream.
        /// </summary>
        public DdlWriter(Stream stream)
        {
            _writer = new StreamWriter(stream);
            _serializer = new Serializer(_writer);
        }

#if !NETFX_CORE
        /// <summary>
        /// Initializes a new instance of the DdlWriter class with the specified filename.
        /// </summary>
        public DdlWriter(string filename)
        {
            _writer = new StreamWriter(filename, false, Encoding.UTF8);
            _serializer = new Serializer(_writer);
        }
#endif

        /// <summary>
        /// Initializes a new instance of the DdlWriter class with the specified TextWriter.
        /// </summary>
        public DdlWriter(TextWriter writer)
        {
            _serializer = new Serializer(writer);
        }

        /// <summary>
        /// Closes the underlying serializer and text writer.
        /// </summary>
        public void Close()
        {
            _serializer = null;

            if (_writer != null)
            {
                _writer.Close();
                _writer = null;
            }
        }

        /// <summary>
        /// Flushes the underlying TextWriter.
        /// </summary>
        public void Flush()
        {
            _serializer.Flush();
        }

        /// <summary>
        /// Gets or sets the indentation for the DDL file.
        /// </summary>
        public int Indent
        {
            get { return _serializer.Indent; }
            set { _serializer.Indent = value; }
        }

        /// <summary>
        /// Gets or sets the initial indentation for the DDL file.
        /// </summary>
        public int InitialIndent
        {
            get { return _serializer.InitialIndent; }
            set { _serializer.InitialIndent = value; }
        }

        /// <summary>
        /// Writes the specified DocumentObject to DDL.
        /// </summary>
        public void WriteDocument(DocumentObject documentObject)
        {
            documentObject.Serialize(_serializer);
            _serializer.Flush();
        }

        /// <summary>
        /// Writes the specified DocumentObjectCollection to DDL.
        /// </summary>
        public void WriteDocument(DocumentObjectCollection documentObjectContainer)
        {
            documentObjectContainer.Serialize(_serializer);
            _serializer.Flush();
        }

        /// <summary>
        /// Writes a DocumentObject type object to string.
        /// </summary>
        public static string WriteToString(DocumentObject docObject)
        {
            return WriteToString(docObject, 2, 0);
        }

        /// <summary>
        /// Writes a DocumentObject type object to string. Indent a new block by indent characters.
        /// </summary>
        public static string WriteToString(DocumentObject docObject, int indent)
        {
            return WriteToString(docObject, indent, 0);
        }

        /// <summary>
        /// Writes a DocumentObject type object to string. Indent a new block by indent + initialIndent characters.
        /// </summary>
        public static string WriteToString(DocumentObject docObject, int indent, int initialIndent)
        {
            StringBuilder strBuilder = new StringBuilder();
            StringWriter writer = null;
            DdlWriter wrt = null;
            try
            {
                writer = new StringWriter(strBuilder);

                wrt = new DdlWriter(writer);
                wrt.Indent = indent;
                wrt.InitialIndent = initialIndent;

                wrt.WriteDocument(docObject);
                wrt.Close();
            }
            finally
            {
                if (wrt != null)
                    wrt.Close();
                if (writer != null)
                {
#if !NETFX_CORE
                    writer.Close();
#else
                    writer.Dispose();
#endif
                }
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// Writes a DocumentObjectCollection type object to string.
        /// </summary>
        public static string WriteToString(DocumentObjectCollection docObjectContainer)
        {
            return WriteToString(docObjectContainer, 2, 0);
        }

        /// <summary>
        /// Writes a DocumentObjectCollection type object to string. Indent a new block by _indent characters.
        /// </summary>
        public static string WriteToString(DocumentObjectCollection docObjectContainer, int indent)
        {
            return WriteToString(docObjectContainer, indent, 0);
        }

        /// <summary>
        /// Writes a DocumentObjectCollection type object to string. Indent a new block by
        /// indent + initialIndent characters.
        /// </summary>
        public static string WriteToString(DocumentObjectCollection docObjectContainer, int indent, int initialIndent)
        {
            StringBuilder strBuilder = new StringBuilder();
            StringWriter writer = null;
            DdlWriter wrt = null;
            try
            {
                writer = new StringWriter(strBuilder);

                wrt = new DdlWriter(writer);
                wrt.Indent = indent;
                wrt.InitialIndent = initialIndent;

                wrt.WriteDocument(docObjectContainer);
                wrt.Close();
            }
            finally
            {
                if (wrt != null)
                    wrt.Close();
                if (writer != null)
                {
#if !NETFX_CORE
                    writer.Close();
#else
                    writer.Dispose();
#endif
                }
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// Writes a document object to a DDL file.
        /// </summary>
        public static void WriteToFile(DocumentObject docObject, string filename)
        {
            WriteToFile(docObject, filename, 2, 0);
        }

        /// <summary>
        /// Writes a document object to a DDL file. Indent a new block by the specified number of characters.
        /// </summary>
        public static void WriteToFile(DocumentObject docObject, string filename, int indent)
        {
            WriteToFile(docObject, filename, indent, 0);
        }

        /// <summary>
        /// Writes a DocumentObject type object to a DDL file. Indent a new block by indent + initialIndent characters.
        /// </summary>
        public static void WriteToFile(DocumentObject docObject, string filename, int indent, int initialIndent)
        {
            DdlWriter wrt = null;
            try
            {
                wrt = new DdlWriter(filename);
                wrt.Indent = indent;
                wrt.InitialIndent = initialIndent;

                wrt.WriteDocument(docObject);
            }
            finally
            {
                if (wrt != null)
                    wrt.Close();
            }
        }

        /// <summary>
        /// Writes a DocumentObjectCollection type object to a DDL file.
        /// </summary>
        public static void WriteToFile(DocumentObjectCollection docObjectContainer, string filename)
        {
            WriteToFile(docObjectContainer, filename, 2, 0);
        }

        /// <summary>
        /// Writes a DocumentObjectCollection type object to a DDL file. Indent a new block by
        /// indent + initialIndent characters.
        /// </summary>
        public static void WriteToFile(DocumentObjectCollection docObjectContainer, string filename, int indent)
        {
            WriteToFile(docObjectContainer, filename, indent, 0);
        }

        /// <summary>
        /// Writes a DocumentObjectCollection type object to a DDL file. Indent a new block by
        /// indent + initialIndent characters.
        /// </summary>
        public static void WriteToFile(DocumentObjectCollection docObjectContainer, string filename, int indent, int initialIndent)
        {
            DdlWriter wrt = null;
            try
            {
                wrt = new DdlWriter(filename);
                wrt.Indent = indent;
                wrt.InitialIndent = initialIndent;

                wrt.WriteDocument(docObjectContainer);
            }
            finally
            {
                if (wrt != null)
                    wrt.Close();
            }
        }

        StreamWriter _writer;
        Serializer _serializer;
    }
}
