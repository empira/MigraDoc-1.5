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
using System.Diagnostics;
using System.Reflection;
using System.IO;
using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.RtfRendering.Resources;

namespace MigraDoc.RtfRendering
{
    /// <summary>
    ///   Render an image to RTF.
    /// </summary>
    internal class ImageRenderer : ShapeRenderer
    {
        internal ImageRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer)
            : base(domObj, docRenderer)
        {
            _image = domObj as Image;
            _filePath = _image.GetFilePath(_docRenderer.WorkingDirectory);
            _isInline = DocumentRelations.HasParentOfType(_image, typeof(Paragraph)) ||
                       RenderInParagraph();

            CalculateImageDimensions();
        }

        /// <summary>
        ///   Renders an image to RTF.
        /// </summary>
        internal override void Render()
        {
            bool renderInParagraph = RenderInParagraph();
            DocumentElements elms = DocumentRelations.GetParent(_image) as DocumentElements;
            if (elms != null && !renderInParagraph &&
                !(DocumentRelations.GetParent(elms) is Section || DocumentRelations.GetParent(elms) is HeaderFooter))
            {
                Debug.WriteLine(Messages2.ImageFreelyPlacedInWrongContext(_image.Name), "warning");
                return;
            }
            if (renderInParagraph)
                StartDummyParagraph();

            if (!_isInline)
                StartShapeArea();

            RenderImage();
            if (!_isInline)
                EndShapeArea();

            if (renderInParagraph)
                EndDummyParagraph();
        }

        /// <summary>
        ///   Renders image specific attributes and the image byte series to RTF.
        /// </summary>
        private void RenderImage()
        {
            StartImageDescription();
            RenderImageAttributes();
            RenderByteSeries();
            EndImageDescription();
        }

        private void StartImageDescription()
        {
            if (_isInline)
            {
                _rtfWriter.StartContent();
                _rtfWriter.WriteControlWithStar("shppict");
                _rtfWriter.StartContent();
                _rtfWriter.WriteControl("pict");
            }
            else
            {
                RenderNameValuePair("shapeType", "75"); // 75 entspr. Bildrahmen.
                StartNameValuePair("pib");
                _rtfWriter.StartContent();
                _rtfWriter.WriteControl("pict");
            }
        }

        private void EndImageDescription()
        {
            if (_isInline)
            {
                _rtfWriter.EndContent();
                _rtfWriter.EndContent();
            }
            else
            {
                _rtfWriter.EndContent();
                EndNameValuePair();
            }
        }

        private void RenderImageAttributes()
        {
            if (_isInline)
            {
                _rtfWriter.StartContent();
                _rtfWriter.WriteControlWithStar("picprop");
                RenderNameValuePair("shapeType", "75");
                RenderFillFormat();
                //REM: LineFormat is not completely supported in word.
                RenderLineFormat();
                _rtfWriter.EndContent();
            }
            RenderDimensionSettings();
            RenderCropping();
            RenderSourceType();
        }

        private void RenderSourceType()
        {
            string extension = Path.GetExtension(_filePath);
            if (extension == null)
            {
                _imageFile = null;
                Debug.WriteLine("No Image type given.", "warning");
                return;
            }
            switch (extension.ToLower())
            {
                case ".jpeg":
                case ".jpg":
                    _rtfWriter.WriteControl("jpegblip");
                    break;

                case ".png":
                    _rtfWriter.WriteControl("pngblip");
                    break;

                case ".gif":
                    _rtfWriter.WriteControl("pngblip");
                    break;

                case ".pdf":
                    // Show a PDF logo in RTF document
                    _imageFile =
                      Assembly.GetExecutingAssembly().GetManifestResourceStream("MigraDoc.RtfRendering.Resources.PDF.png");
                    _rtfWriter.WriteControl("pngblip");
                    break;

                default:
                    Debug.WriteLine(Messages2.ImageTypeNotSupported(_image.Name), "warning");
                    _imageFile = null;
                    break;
            }
        }

        /// <summary>
        ///   Renders scaling, width and height for the image.
        /// </summary>
        private void RenderDimensionSettings()
        {
            float scaleX = (GetShapeWidth() / _originalWidth);
            float scaleY = (GetShapeHeight() / _originalHeight);
            _rtfWriter.WriteControl("picscalex", (int)(scaleX * 100));
            _rtfWriter.WriteControl("picscaley", (int)(scaleY * 100));

            RenderUnit("pichgoal", GetShapeHeight() / scaleY);
            RenderUnit("picwgoal", GetShapeWidth() / scaleX);

            //A bit obscure, but necessary for Word 2000:
            _rtfWriter.WriteControl("pich", (int)(_originalHeight.Millimeter * 100));
            _rtfWriter.WriteControl("picw", (int)(_originalWidth.Millimeter * 100));
        }

        private void CalculateImageDimensions()
        {
            XImage bip = null;
            try
            {
                _imageFile = File.OpenRead(_filePath);
                //System.Drawing.Bitmap bip2 = new System.Drawing.Bitmap(imageFile);
                bip = XImage.FromFile(_filePath);

                float horzResolution;
                float vertResolution;
                string ext = Path.GetExtension(_filePath).ToLower();
                float origHorzRes = (float)bip.HorizontalResolution;
                float origVertRes = (float)bip.VerticalResolution;

                _originalHeight = bip.PixelHeight * 72 / origVertRes;
                _originalWidth = bip.PixelWidth * 72 / origHorzRes;

                if (_image.IsNull("Resolution"))
                {
                    horzResolution = (ext == ".gif") ? 72 : (float)bip.HorizontalResolution;
                    vertResolution = (ext == ".gif") ? 72 : (float)bip.VerticalResolution;
                }
                else
                {
                    horzResolution = (float)GetValueAsIntended("Resolution");
                    vertResolution = horzResolution;
                }

                Unit origHeight = bip.Size.Height * 72 / vertResolution;
                Unit origWidth = bip.Size.Width * 72 / horzResolution;

                _imageHeight = origHeight;
                _imageWidth = origWidth;

                bool scaleWidthIsNull = _image.IsNull("ScaleWidth");
                bool scaleHeightIsNull = _image.IsNull("ScaleHeight");
                float sclHeight = scaleHeightIsNull ? 1 : (float)GetValueAsIntended("ScaleHeight");
                _scaleHeight = sclHeight;
                float sclWidth = scaleWidthIsNull ? 1 : (float)GetValueAsIntended("ScaleWidth");
                _scaleWidth = sclWidth;

                bool doLockAspectRatio = _image.IsNull("LockAspectRatio") || _image.LockAspectRatio;

                if (doLockAspectRatio && (scaleHeightIsNull || scaleWidthIsNull))
                {
                    if (!_image.IsNull("Width") && _image.IsNull("Height"))
                    {
                        _imageWidth = _image.Width;
                        _imageHeight = origHeight * _imageWidth / origWidth;
                    }
                    else if (!_image.IsNull("Height") && _image.IsNull("Width"))
                    {
                        _imageHeight = _image.Height;
                        _imageWidth = origWidth * _imageHeight / origHeight;
                    }
                    else if (!_image.IsNull("Height") && !_image.IsNull("Width"))
                    {
                        _imageWidth = _image.Width;
                        _imageHeight = _image.Height;
                    }
                    if (scaleWidthIsNull && !scaleHeightIsNull)
                        _scaleWidth = _scaleHeight;
                    else if (scaleHeightIsNull && !scaleWidthIsNull)
                        _scaleHeight = _scaleWidth;
                }
                else
                {
                    if (!_image.IsNull("Width"))
                        _imageWidth = _image.Width;
                    if (!_image.IsNull("Height"))
                        _imageHeight = _image.Height;
                }
                return;
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine(Messages2.ImageNotFound(_image.Name), "warning");
            }
            catch (Exception exc)
            {
                Debug.WriteLine(Messages2.ImageNotReadable(_image.Name, exc.Message), "warning");
            }
            finally
            {
                if (bip != null)
                    bip.Dispose();
            }

            //Setting defaults in case an error occurred.
            _imageFile = null;
            _imageHeight = (Unit)GetValueOrDefault("Height", Unit.FromInch(1));
            _imageWidth = (Unit)GetValueOrDefault("Width", Unit.FromInch(1));
            _scaleHeight = (double)GetValueOrDefault("ScaleHeight", 1.0);
            _scaleWidth = (double)GetValueOrDefault("ScaleWidth", 1.0);
        }

        /// <summary>
        ///   Renders the image file as byte series.
        /// </summary>
        private void RenderByteSeries()
        {
            if (_imageFile != null)
            {
                _imageFile.Seek(0, SeekOrigin.Begin);
                int byteVal;
                while ((byteVal = _imageFile.ReadByte()) != -1)
                {
                    string strVal = byteVal.ToString("x");
                    if (strVal.Length == 1)
                        _rtfWriter.WriteText("0");
                    _rtfWriter.WriteText(strVal);
                }
                _imageFile.Close();
            }
        }

        protected override Unit GetShapeHeight()
        {
            return _imageHeight * _scaleHeight;
        }


        protected override Unit GetShapeWidth()
        {
            return _imageWidth * _scaleWidth;
        }

        /// <summary>
        ///   Renders the image cropping at all edges.
        /// </summary>
        private void RenderCropping()
        {
            Translate("PictureFormat.CropLeft", "piccropl");
            Translate("PictureFormat.CropRight", "piccropr");
            Translate("PictureFormat.CropTop", "piccropt");
            Translate("PictureFormat.CropBottom", "piccropb");
        }

        private readonly string _filePath;
        private readonly Image _image;
        private readonly bool _isInline;
        //FileStream imageFile;
        private Stream _imageFile;
        private Unit _imageWidth;
        private Unit _imageHeight;

        private Unit _originalHeight;
        private Unit _originalWidth;

        //this is the user defined scaling, not the stuff to render as scalex, scaley
        private double _scaleHeight;
        private double _scaleWidth;
    }
}