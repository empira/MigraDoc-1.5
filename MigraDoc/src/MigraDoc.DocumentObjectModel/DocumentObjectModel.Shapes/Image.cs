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

using System;
using System.IO;
using MigraDoc.DocumentObjectModel.Internals;

namespace MigraDoc.DocumentObjectModel.Shapes
{
    /// <summary>
    /// Represents an image in the document or paragraph.
    /// </summary>
    public class Image : Shape
    {
        /// <summary>
        /// Initializes a new instance of the Image class.
        /// </summary>
        public Image()
        { }

        /// <summary>
        /// Initializes a new instance of the Image class with the specified parent.
        /// </summary>
        internal Image(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Initializes a new instance of the Image class from the specified (file) name.
        /// </summary>
        public Image(string name)
            : this()
        {
            Name = name;
        }

        //#region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Image Clone()
        {
            return (Image)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Image image = (Image)base.DeepCopy();
            if (image._pictureFormat != null)
            {
                image._pictureFormat = image._pictureFormat.Clone();
                image._pictureFormat._parent = image;
            }
            return image;
        }
        //#endregion

        //#region Properties
        /// <summary>
        /// Gets or sets the name of the image.
        /// </summary>
        public string Name
        {
            get { return _name.Value; }
            set { _name.Value = value; }
        }
        [DV]
        internal NString _name = NString.NullValue;

        /// <summary>
        /// Gets or sets the ScaleWidth of the image.
        /// If the Width is set to, the resulting image width is ScaleWidth * Width.
        /// </summary>
        public double ScaleWidth
        {
            get { return _scaleWidth.Value; }
            set { _scaleWidth.Value = value; }
        }
        [DV]
        internal NDouble _scaleWidth = NDouble.NullValue;

        /// <summary>
        /// Gets or sets the ScaleHeight of the image.
        /// If the Height is set too, the resulting image height is ScaleHeight * Height.
        /// </summary>
        public double ScaleHeight
        {
            get { return _scaleHeight.Value; }
            set { _scaleHeight.Value = value; }
        }
        [DV]
        internal NDouble _scaleHeight = NDouble.NullValue;

        /// <summary>
        /// Gets or sets whether the AspectRatio of the image is kept unchanged.
        /// If both Width and Height are set, this property is ignored.
        /// </summary>
        public bool LockAspectRatio
        {
            get { return _lockAspectRatio.Value; }
            set { _lockAspectRatio.Value = value; }
        }
        [DV]
        internal NBool _lockAspectRatio = NBool.NullValue;

        /// <summary>
        /// Gets or sets the PictureFormat for the image
        /// </summary>
        public PictureFormat PictureFormat
        {
            get { return _pictureFormat ?? (_pictureFormat = new PictureFormat(this)); }
            set
            {
                SetParent(value);
                _pictureFormat = value;
            }
        }
        [DV]
        internal PictureFormat _pictureFormat;

        /// <summary>
        /// Gets or sets a user defined resolution for the image in dots per inch.
        /// </summary>
        public double Resolution
        {
            get { return _resolution.Value; }
            set { _resolution.Value = value; }
        }
        [DV]
        internal NDouble _resolution = NDouble.NullValue;
        //#endregion

        #region Internal
        /// <summary>
        /// Converts Image into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine("\\image(\"" + _name.Value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\")");

            int pos = serializer.BeginAttributes();

            base.Serialize(serializer);
            if (!_scaleWidth.IsNull)
                serializer.WriteSimpleAttribute("ScaleWidth", ScaleWidth);
            if (!_scaleHeight.IsNull)
                serializer.WriteSimpleAttribute("ScaleHeight", ScaleHeight);
            if (!_lockAspectRatio.IsNull)
                serializer.WriteSimpleAttribute("LockAspectRatio", LockAspectRatio);
            if (!_resolution.IsNull)
                serializer.WriteSimpleAttribute("Resolution", Resolution);
            if (!IsNull("PictureFormat"))
                _pictureFormat.Serialize(serializer);

            serializer.EndAttributes(pos);
        }

        /// <summary>
        /// Gets the concrete image path, taking into account the DOM document's DdlFile and
        /// ImagePath properties as well as the given working directory (which can be null).
        /// </summary>
        public string GetFilePath(string workingDir)
        {
            if (Name.StartsWith("base64:")) // The file is stored in the string here, so we don't have to add a path.
                return Name;

            string filePath;

#if !NETFX_CORE
            if (!String.IsNullOrEmpty(workingDir))
                filePath = workingDir;
            else
                filePath = Directory.GetCurrentDirectory() + "\\";
#else
            throw new NotImplementedException();
            //if (!String.IsNullOrEmpty(workingDir))
            //    filePath = workingDir;
            //else
            //    filePath = Directory.GetCurrentDirectory() + "\\";
#endif

            if (!Document.IsNull("ImagePath"))
            {
                string foundfile = ImageHelper.GetImageName(filePath, Name, Document.ImagePath);
                if (foundfile != null)
                    filePath = foundfile;
                else
                    filePath = Path.Combine(filePath, Name);
            }
            else
                filePath = Path.Combine(filePath, Name);

            return filePath;
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Image))); }
        }
        static Meta _meta;
        #endregion
    }
}
