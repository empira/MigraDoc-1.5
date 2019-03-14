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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using MigraDoc.DocumentObjectModel.Internals;

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// The Color class represents an ARGB color value.
    /// </summary>
    [DebuggerDisplay("(A={A}, R={R}, G={G}, B={B} C={C}, M={M}, Y={Y}, K={K})")]
    public struct Color : INullableValue
    {
        /// <summary>
        /// Initializes a new instance of the Color class.
        /// </summary>
        public Color(uint argb)
        {
            _isCmyk = false;
            _argb = argb;
            _a = _c = _m = _y = _k = 0f; // Compiler enforces this line of code
            InitCmykFromRgb();
        }

        /// <summary>
        /// Initializes a new instance of the Color class.
        /// </summary>
        public Color(byte r, byte g, byte b)
        {
            _isCmyk = false;
            _argb = 0xFF000000 | ((uint)r << 16) | ((uint)g << 8) | b;
            _a = _c = _m = _y = _k = 0f; // Compiler enforces this line of code
            InitCmykFromRgb();
        }

        /// <summary>
        /// Initializes a new instance of the Color class.
        /// </summary>
        public Color(byte a, byte r, byte g, byte b)
        {
            _isCmyk = false;
            _argb = ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
            _a = _c = _m = _y = _k = 0f; // Compiler enforces this line of code
            InitCmykFromRgb();
        }

        /// <summary>
        /// Initializes a new instance of the Color class with a CMYK color.
        /// All values must be in a range between 0 to 100 percent.
        /// </summary>
        public Color(double alpha, double cyan, double magenta, double yellow, double black)
        {
            _isCmyk = true;
            _a = (float)(alpha > 100 ? 100 : (alpha < 0 ? 0 : alpha));
            _c = (float)(cyan > 100 ? 100 : (cyan < 0 ? 0 : cyan));
            _m = (float)(magenta > 100 ? 100 : (magenta < 0 ? 0 : magenta));
            _y = (float)(yellow > 100 ? 100 : (yellow < 0 ? 0 : yellow));
            _k = (float)(black > 100 ? 100 : (black < 0 ? 0 : black));
            _argb = 0; // Compiler enforces this line of code
            InitRgbFromCmyk();
        }

        /// <summary>
        /// Initializes a new instance of the Color class with a CMYK color.
        /// All values must be in a range between 0 to 100 percent.
        /// </summary>
        public Color(double cyan, double magenta, double yellow, double black)
            : this(100, cyan, magenta, yellow, black)
        { }

        void InitCmykFromRgb()
        {
            // Similar formula as in PDFsharp
            _isCmyk = false;
            int c = 255 - (int)R;
            int m = 255 - (int)G;
            int y = 255 - (int)B;
            int k = Math.Min(c, Math.Min(m, y));
            if (k == 255)
                _c = _m = _y = 0;
            else
            {
                float black = 255f - k;
                _c = 100f * (c - k) / black;
                _m = 100f * (m - k) / black;
                _y = 100f * (y - k) / black;
            }
            _k = 100f * k / 255f;
            _a = A / 2.55f;
        }

        void InitRgbFromCmyk()
        {
            // Similar formula as in PDFsharp
            _isCmyk = true;
            float black = _k * 2.55f + 0.5f;
            float factor = (255f - black) / 100f;
            byte a = (byte)(_a * 2.55 + 0.5);
            byte r = (byte)(255 - Math.Min(255f, _c * factor + black));
            byte g = (byte)(255 - Math.Min(255f, _m * factor + black));
            byte b = (byte)(255 - Math.Min(255f, _y * factor + black));
            _argb = ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a CMYK color.
        /// </summary>
        public bool IsCmyk
        {
            get { return _isCmyk; }
        }

        /// <summary>
        /// Determines whether this color is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return this == Empty; }
        }

        /// <summary>
        /// Returns the value.
        /// </summary>
        object INullableValue.GetValue()
        {
            return this;
        }

        /// <summary>
        /// Sets the given value.
        /// </summary>
        void INullableValue.SetValue(object value)
        {
            if (value is uint)
                _argb = (uint)value;
            else
                this = Parse(value.ToString());
        }

        /// <summary>
        /// Resets this instance, i.e. IsNull() will return true afterwards.
        /// </summary>
        void INullableValue.SetNull()
        {
            this = Empty;
        }

        /// <summary>
        /// Determines whether this instance is null (not set).
        /// </summary>
        bool INullableValue.IsNull
        {
            get { return this == Empty; }
        }

        /// <summary>
        /// Determines whether this instance is null (not set).
        /// </summary>
        internal bool IsNull
        {
            get { return this == Empty; }
        }

        /// <summary>
        /// Gets or sets the ARGB value.
        /// </summary>
        public uint Argb
        {
            get { return _argb; }
            set
            {
                if (_isCmyk)
                    throw new InvalidOperationException("Cannot change a CMYK color.");
                _argb = value;
                InitCmykFromRgb();
            }
        }

        /// <summary>
        /// Gets or sets the RGB value.
        /// </summary>
        public uint RGB
        {
            get { return _argb; }
            set
            {
                if (_isCmyk)
                    throw new InvalidOperationException("Cannot change a CMYK color.");
                _argb = value;
                InitCmykFromRgb();
            }
        }

        /// <summary>
        /// Calls base class Equals.
        /// </summary>
        public override bool Equals(Object obj)
        {
            if (obj is Color)
            {
                Color color = (Color)obj;
                if (_isCmyk ^ color._isCmyk)
                    return false;
                if (_isCmyk)
                    return _a == color._a && _c == color._c && _m == color._m && _y == color._y && _k == color._k;
                return _argb == color._argb;
            }
            return false;
        }

        /// <summary>
        /// Gets the ARGB value that this Color instance represents.
        /// </summary>
        public override int GetHashCode()
        {
            return (int)_argb ^ _a.GetHashCode() ^ _c.GetHashCode() ^ _m.GetHashCode() ^ _y.GetHashCode() ^ _k.GetHashCode();
        }

        /// <summary>
        /// Compares two color objects. True if both argb values are equal, false otherwise.
        /// </summary>
        public static bool operator ==(Color color1, Color color2)
        {
            if (color1._isCmyk ^ color2._isCmyk)
                return false;
            if (color1._isCmyk)
                return color1._a == color2._a && color1._c == color2._c && color1._m == color2._m && color1._y == color2._y && color1._k == color2._k;
            return color1._argb == color2._argb;
        }

        /// <summary>
        /// Compares two color objects. True if both argb values are not equal, false otherwise.
        /// </summary>
        public static bool operator !=(Color color1, Color color2)
        {
            return !(color1 == color2);
        }

        /// <summary>
        /// Parses the string and returns a color object.
        /// Throws ArgumentException if color is invalid.
        /// Supports four different formats for hex colors.
        /// Format 1: uses prefix "0x", followed by as many hex digits as needed. Important: do not forget the opacity, so use 7 or 8 digits.
        /// Format 2: uses prefix "#", followed by exactly 8 digits including opacity.
        /// Format 3: uses prefix "#", followed by exactly 6 digits; opacity will be 0xff.
        /// Format 4: uses prefix "#", followed by exactly 3 digits; opacity will be 0xff; "#ccc" will be treated as "#ffcccccc", "#d24" will be treated as "#ffdd2244".
        /// </summary>
        /// <param name="color">integer, hex or color name.</param>
        public static Color Parse(string color)
        {
            if (color == null)
                throw new ArgumentNullException("color");
            if (color == "")
                throw new ArgumentException("color");

            try
            {
                uint clr;
                // Must use Enum.Parse because Enum.IsDefined is case sensitive
                try
                {
                    object obj = Enum.Parse(typeof(ColorName), color, true);
                    clr = (uint)obj;
                    return new Color(clr);
                }
                catch
                {
                    // Ignore exception because it's not a ColorName.
                }

                NumberStyles numberStyle = NumberStyles.Integer;
                string number = color.ToLower();
                if (number.StartsWith("0x"))
                {
                    numberStyle = NumberStyles.HexNumber;
                    number = color.Substring(2);
                }
                else if (number.StartsWith("#"))
                {
                    numberStyle = NumberStyles.HexNumber;
                    switch (color.Length)
                    {
                        case 9:
                            number = color.Substring(1);
                            break;
                        case 7:
                            number = "ff" + color.Substring(1);
                            break;
                        case 4:
                            number = "ff" + color.Substring(1,1) + color.Substring(1, 1) + 
                                     color.Substring(2, 1) + color.Substring(2, 1) + 
                                     color.Substring(3, 1) + color.Substring(3, 1);
                            break;
                        default:
                            throw new ArgumentException(DomSR.InvalidColorString(color), "color");
                    }
                }
                clr = uint.Parse(number, numberStyle);
                return new Color(clr);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException(DomSR.InvalidColorString(color), ex);
            }
        }

        /// <summary>
        /// Gets the alpha (transparency) part of the RGB Color.
        /// The values is in the range between 0 to 255.
        /// </summary>
        public uint A
        {
            get { return (_argb & 0xFF000000) >> 24; }
        }

        /// <summary>
        /// Gets the red part of the Color.
        /// The values is in the range between 0 to 255.
        /// </summary>
        public uint R
        {
            get { return (_argb & 0xFF0000) >> 16; }
        }

        /// <summary>
        /// Gets the green part of the Color.
        /// The values is in the range between 0 to 255.
        /// </summary>
        public uint G
        {
            get { return (_argb & 0x00FF00) >> 8; }
        }

        /// <summary>
        /// Gets the blue part of the Color.
        /// The values is in the range between 0 to 255.
        /// </summary>
        public uint B
        {
            get { return _argb & 0x0000FF; }
        }

        /// <summary>
        /// Gets the alpha (transparency) part of the CMYK Color.
        /// The values is in the range between 0 (transparent) to 100 (opaque) percent.
        /// </summary>
        public double Alpha
        {
            get { return _a; }
        }

        /// <summary>
        /// Gets the cyan part of the Color.
        /// The values is in the range between 0 to 100 percent.
        /// </summary>
        public double C
        {
            get { return _c; }
        }

        /// <summary>
        /// Gets the magenta part of the Color.
        /// The values is in the range between 0 to 100 percent.
        /// </summary>
        public double M
        {
            get { return _m; }
        }

        /// <summary>
        /// Gets the yellow part of the Color.
        /// The values is in the range between 0 to 100 percent.
        /// </summary>
        public double Y
        {
            get { return _y; }
        }

        /// <summary>
        /// Gets the key (black) part of the Color.
        /// The values is in the range between 0 to 100 percent.
        /// </summary>
        public double K
        {
            get { return _k; }
        }

        /// <summary>
        /// Gets a non transparent color brightened in terms of transparency if any is given(A &lt; 255),
        /// otherwise this instance itself.
        /// </summary>
        public Color GetMixedTransparencyColor()
        {
            int alpha = (int)A;
            if (alpha == 0xFF)
                return this;

            int red = (int)R;
            int green = (int)G;
            int blue = (int)B;

            double whiteFactor = 1 - alpha / 255.0;

            red = (int)(red + (255 - red) * whiteFactor);
            green = (int)(green + (255 - green) * whiteFactor);
            blue = (int)(blue + (255 - blue) * whiteFactor);
            return new Color((uint)(0xFF << 24 | (red << 16) | (green << 8) | blue));
        }

        /// <summary>
        /// Writes the Color object in its hexadecimal value.
        /// </summary>
        public override string ToString()
        {
            if (_stdColors == null)
            {
#if !SILVERLIGHT
                Array colorNames = Enum.GetNames(typeof(ColorName));
                Array colorValues = Enum.GetValues(typeof(ColorName));
                int count = colorNames.GetLength(0);
                _stdColors = new Dictionary<uint, string>(count);
                for (int index = 0; index < count; index++)
                {
                    string c = (string)colorNames.GetValue(index);
                    uint d = (uint)colorValues.GetValue(index);
                    // Some color are double named...
                    // Aqua == Cyan
                    // Fuchsia == Magenta
                    Style key = new Style();
                    if (!_stdColors.ContainsKey(d))
                        _stdColors.Add(d, c);
                }
#else
        _stdColors = new Dictionary<uint, string>();
        _stdColors.Add(0xFFF0F8FF, "AliceBlue");
        _stdColors.Add(0xFFFAEBD7, "AntiqueWhite");
        _stdColors.Add(0xFF00FFFF, "Aqua");
        _stdColors.Add(0xFF7FFFD4, "Aquamarine");
        _stdColors.Add(0xFFF0FFFF, "Azure");
        _stdColors.Add(0xFFF5F5DC, "Beige");
        _stdColors.Add(0xFFFFE4C4, "Bisque");
        _stdColors.Add(0xFF000000, "Black");
        _stdColors.Add(0xFFFFEBCD, "BlanchedAlmond");
        _stdColors.Add(0xFF0000FF, "Blue");
        _stdColors.Add(0xFF8A2BE2, "BlueViolet");
        _stdColors.Add(0xFFA52A2A, "Brown");
        _stdColors.Add(0xFFDEB887, "BurlyWood");
        _stdColors.Add(0xFF5F9EA0, "CadetBlue");
        _stdColors.Add(0xFF7FFF00, "Chartreuse");
        _stdColors.Add(0xFFD2691E, "Chocolate");
        _stdColors.Add(0xFFFF7F50, "Coral");
        _stdColors.Add(0xFF6495ED, "CornflowerBlue");
        _stdColors.Add(0xFFFFF8DC, "Cornsilk");
        _stdColors.Add(0xFFDC143C, "Crimson");
        //_stdColors.Add(0xFF00FFFF, "Cyan"); already added as "Aqua"
        _stdColors.Add(0xFF00008B, "DarkBlue");
        _stdColors.Add(0xFF008B8B, "DarkCyan");
        _stdColors.Add(0xFFB8860B, "DarkGoldenrod");
        _stdColors.Add(0xFFA9A9A9, "DarkGray");
        _stdColors.Add(0xFF006400, "DarkGreen");
        _stdColors.Add(0xFFBDB76B, "DarkKhaki");
        _stdColors.Add(0xFF8B008B, "DarkMagenta");
        _stdColors.Add(0xFF556B2F, "DarkOliveGreen");
        _stdColors.Add(0xFFFF8C00, "DarkOrange");
        _stdColors.Add(0xFF9932CC, "DarkOrchid");
        _stdColors.Add(0xFF8B0000, "DarkRed");
        _stdColors.Add(0xFFE9967A, "DarkSalmon");
        _stdColors.Add(0xFF8FBC8B, "DarkSeaGreen");
        _stdColors.Add(0xFF483D8B, "DarkSlateBlue");
        _stdColors.Add(0xFF2F4F4F, "DarkSlateGray");
        _stdColors.Add(0xFF00CED1, "DarkTurquoise");
        _stdColors.Add(0xFF9400D3, "DarkViolet");
        _stdColors.Add(0xFFFF1493, "DeepPink");
        _stdColors.Add(0xFF00BFFF, "DeepSkyBlue");
        _stdColors.Add(0xFF696969, "DimGray");
        _stdColors.Add(0xFF1E90FF, "DodgerBlue");
        _stdColors.Add(0xFFB22222, "Firebrick");
        _stdColors.Add(0xFFFFFAF0, "FloralWhite");
        _stdColors.Add(0xFF228B22, "ForestGreen");
        _stdColors.Add(0xFFFF00FF, "Fuchsia");
        _stdColors.Add(0xFFDCDCDC, "Gainsboro");
        _stdColors.Add(0xFFF8F8FF, "GhostWhite");
        _stdColors.Add(0xFFFFD700, "Gold");
        _stdColors.Add(0xFFDAA520, "Goldenrod");
        _stdColors.Add(0xFF808080, "Gray");
        _stdColors.Add(0xFF008000, "Green");
        _stdColors.Add(0xFFADFF2F, "GreenYellow");
        _stdColors.Add(0xFFF0FFF0, "Honeydew");
        _stdColors.Add(0xFFFF69B4, "HotPink");
        _stdColors.Add(0xFFCD5C5C, "IndianRed");
        _stdColors.Add(0xFF4B0082, "Indigo");
        _stdColors.Add(0xFFFFFFF0, "Ivory");
        _stdColors.Add(0xFFF0E68C, "Khaki");
        _stdColors.Add(0xFFE6E6FA, "Lavender");
        _stdColors.Add(0xFFFFF0F5, "LavenderBlush");
        _stdColors.Add(0xFF7CFC00, "LawnGreen");
        _stdColors.Add(0xFFFFFACD, "LemonChiffon");
        _stdColors.Add(0xFFADD8E6, "LightBlue");
        _stdColors.Add(0xFFF08080, "LightCoral");
        _stdColors.Add(0xFFE0FFFF, "LightCyan");
        _stdColors.Add(0xFFFAFAD2, "LightGoldenrodYellow");
        _stdColors.Add(0xFFD3D3D3, "LightGray");
        _stdColors.Add(0xFF90EE90, "LightGreen");
        _stdColors.Add(0xFFFFB6C1, "LightPink");
        _stdColors.Add(0xFFFFA07A, "LightSalmon");
        _stdColors.Add(0xFF20B2AA, "LightSeaGreen");
        _stdColors.Add(0xFF87CEFA, "LightSkyBlue");
        _stdColors.Add(0xFF778899, "LightSlateGray");
        _stdColors.Add(0xFFB0C4DE, "LightSteelBlue");
        _stdColors.Add(0xFFFFFFE0, "LightYellow");
        _stdColors.Add(0xFF00FF00, "Lime");
        _stdColors.Add(0xFF32CD32, "LimeGreen");
        _stdColors.Add(0xFFFAF0E6, "Linen");
        // _stdColors.Add(0xFFFF00FF, "Magenta"); already added as "Fuchsia"
        _stdColors.Add(0xFF800000, "Maroon");
        _stdColors.Add(0xFF66CDAA, "MediumAquamarine");
        _stdColors.Add(0xFF0000CD, "MediumBlue");
        _stdColors.Add(0xFFBA55D3, "MediumOrchid");
        _stdColors.Add(0xFF9370DB, "MediumPurple");
        _stdColors.Add(0xFF3CB371, "MediumSeaGreen");
        _stdColors.Add(0xFF7B68EE, "MediumSlateBlue");
        _stdColors.Add(0xFF00FA9A, "MediumSpringGreen");
        _stdColors.Add(0xFF48D1CC, "MediumTurquoise");
        _stdColors.Add(0xFFC71585, "MediumVioletRed");
        _stdColors.Add(0xFF191970, "MidnightBlue");
        _stdColors.Add(0xFFF5FFFA, "MintCream");
        _stdColors.Add(0xFFFFE4E1, "MistyRose");
        _stdColors.Add(0xFFFFE4B5, "Moccasin");
        _stdColors.Add(0xFFFFDEAD, "NavajoWhite");
        _stdColors.Add(0xFF000080, "Navy");
        _stdColors.Add(0xFFFDF5E6, "OldLace");
        _stdColors.Add(0xFF808000, "Olive");
        _stdColors.Add(0xFF6B8E23, "OliveDrab");
        _stdColors.Add(0xFFFFA500, "Orange");
        _stdColors.Add(0xFFFF4500, "OrangeRed");
        _stdColors.Add(0xFFDA70D6, "Orchid");
        _stdColors.Add(0xFFEEE8AA, "PaleGoldenrod");
        _stdColors.Add(0xFF98FB98, "PaleGreen");
        _stdColors.Add(0xFFAFEEEE, "PaleTurquoise");
        _stdColors.Add(0xFFDB7093, "PaleVioletRed");
        _stdColors.Add(0xFFFFEFD5, "PapayaWhip");
        _stdColors.Add(0xFFFFDAB9, "PeachPuff");
        _stdColors.Add(0xFFCD853F, "Peru");
        _stdColors.Add(0xFFFFC0CB, "Pink");
        _stdColors.Add(0xFFDDA0DD, "Plum");
        _stdColors.Add(0xFFB0E0E6, "PowderBlue");
        _stdColors.Add(0xFF800080, "Purple");
        _stdColors.Add(0xFFFF0000, "Red");
        _stdColors.Add(0xFFBC8F8F, "RosyBrown");
        _stdColors.Add(0xFF4169E1, "RoyalBlue");
        _stdColors.Add(0xFF8B4513, "SaddleBrown");
        _stdColors.Add(0xFFFA8072, "Salmon");
        _stdColors.Add(0xFFF4A460, "SandyBrown");
        _stdColors.Add(0xFF2E8B57, "SeaGreen");
        _stdColors.Add(0xFFFFF5EE, "SeaShell");
        _stdColors.Add(0xFFA0522D, "Sienna");
        _stdColors.Add(0xFFC0C0C0, "Silver");
        _stdColors.Add(0xFF87CEEB, "SkyBlue");
        _stdColors.Add(0xFF6A5ACD, "SlateBlue");
        _stdColors.Add(0xFF708090, "SlateGray");
        _stdColors.Add(0xFFFFFAFA, "Snow");
        _stdColors.Add(0xFF00FF7F, "SpringGreen");
        _stdColors.Add(0xFF4682B4, "SteelBlue");
        _stdColors.Add(0xFFD2B48C, "Tan");
        _stdColors.Add(0xFF008080, "Teal");
        _stdColors.Add(0xFFD8BFD8, "Thistle");
        _stdColors.Add(0xFFFF6347, "Tomato");
        _stdColors.Add(0x00FFFFFF, "Transparent");
        _stdColors.Add(0xFF40E0D0, "Turquoise");
        _stdColors.Add(0xFFEE82EE, "Violet");
        _stdColors.Add(0xFFF5DEB3, "Wheat");
        _stdColors.Add(0xFFFFFFFF, "White");
        _stdColors.Add(0xFFF5F5F5, "WhiteSmoke");
        _stdColors.Add(0xFFFFFF00, "Yellow");
        _stdColors.Add(0xFF9ACD32, "YellowGreen");
#endif
            }
            if (_isCmyk)
            {
                string s;
                if (Alpha == 100.0)
                    s = String.Format(CultureInfo.InvariantCulture, "CMYK({0:0.##},{1:0.##},{2:0.##},{3:0.##})", C, M, Y, K);
                else
                    s = String.Format(CultureInfo.InvariantCulture, "CMYK({0:0.##},{1:0.##},{2:0.##},{3:0.##},{4:0.##})", Alpha, C, M, Y, K);
                return s;
            }
            else
            {
                if (_stdColors.ContainsKey(_argb))
                    return _stdColors[_argb];
                if ((_argb & 0xFF000000) == 0xFF000000)
                    return "RGB(" +
                           ((_argb & 0xFF0000) >> 16) + "," +
                           ((_argb & 0x00FF00) >> 8) + "," +
                           (_argb & 0x0000FF) + ")";
                return "0x" + _argb.ToString("X");
            }
        }
        static Dictionary<uint, string> _stdColors;

        /// <summary>
        /// Creates an RGB color from an existing color with a new alpha (transparency) value.
        /// </summary>
        public static Color FromRgbColor(byte a, Color color)
        {
            return new Color(a, (byte)color.R, (byte)color.G, (byte)color.B);
        }

        /// <summary>
        /// Creates an RGB color from red, green, and blue.
        /// </summary>
        public static Color FromRgb(byte r, byte g, byte b)
        {
            return new Color(255, r, g, b);
        }

        /// <summary>
        /// Creates an RGB color from alpha, red, green, and blue.
        /// </summary>
        public static Color FromArgb(byte alpha, byte r, byte g, byte b)
        {
            return new Color(alpha, r, g, b);
        }

        /// <summary>
        /// Creates a Color structure from the specified CMYK values.
        /// All values must be in a range between 0 to 100 percent.
        /// </summary>
        public static Color FromCmyk(double cyan, double magenta, double yellow, double black)
        {
            return new Color(cyan, magenta, yellow, black);
        }

        /// <summary>
        /// Creates a Color structure from the specified CMYK values.
        /// All values must be in a range between 0 to 100 percent.
        /// </summary>
        public static Color FromCmyk(double alpha, double cyan, double magenta, double yellow, double black)
        {
            return new Color(alpha, cyan, magenta, yellow, black);
        }

        /// <summary>
        /// Creates a CMYK color from an existing color with a new alpha (transparency) value.
        /// </summary>
        public static Color FromCmykColor(double alpha, Color color)
        {
            return new Color(alpha, color.C, color.M, color.Y, color.K);
        }

        uint _argb; // ARGB
        bool _isCmyk;
        private float _a; // \
        private float _c; // |
        private float _m; // |--- alpha + CMYK
        private float _y; // |
        private float _k; // /

        /// <summary>
        /// Represents a null color.
        /// </summary>
        public static readonly Color Empty = new Color(0);
    }
}