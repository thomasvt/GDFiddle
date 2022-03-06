using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace GDFiddle.UI
{
    // how to render: http://www.angelcode.com/products/bmfont/doc/render_text.html

    public class Font
    {
        public string Filename { get; }
        internal readonly int LineHeight;
        internal readonly int Base;
        internal readonly Dictionary<int, Glyph> Glyphs;
        private readonly Dictionary<uint, int> _kernings; // kerning-distance by First+Second charcode combined into uint.

        internal Font(string filename, int lineHeight, int @base, List<Glyph> glyphs, List<Kerning> kernings)
        {
            Filename = filename;
            LineHeight = lineHeight;
            Base = @base;
            Glyphs = glyphs.ToDictionary(g => g.Code);
            _kernings = kernings.ToDictionary(k => (uint)(k.First << 16) | k.Second, k => k.Amount);
        }

        internal int GetKerningDistance(ushort first, ushort second)
        {
            var mask = (uint) (first << 16) | second;
            return _kernings.TryGetValue(mask, out var distance) 
                ? distance 
                : 0;
        }

        internal readonly struct Glyph
        {
            public readonly int Code;
            public readonly Vector2 UVMin;
            public readonly Vector2 UVMax;
            public readonly int Width;
            public readonly int Heigth;
            public readonly int XOffset;
            public readonly int YOffset;
            public readonly int XAdvance;

            public Glyph(int code, Vector2 uvMin, Vector2 uvMax, int width, int heigth, int xOffset, int yOffset, int xAdvance)
            {
                Code = code;
                UVMin = uvMin;
                UVMax = uvMax;
                Width = width;
                Heigth = heigth;
                XOffset = xOffset;
                YOffset = yOffset;
                XAdvance = xAdvance;
            }
        }

        internal readonly struct Kerning
        {
            public readonly ushort First;
            public readonly ushort Second;
            public readonly int Amount;

            public Kerning(ushort first, ushort second, int amount)
            {
                First = first;
                Second = second;
                Amount = amount;
            }
        }

        public static Font FromBMFontFile(string pngFile)
        {
            var xmlFile = Path.GetFileNameWithoutExtension(pngFile) + ".fnt";
            if (!File.Exists(pngFile))
                throw new FileNotFoundException($"Font file '{pngFile}' not found.");
            if (!File.Exists(xmlFile))
                throw new FileNotFoundException($"Font file should have an accompanying xml-file '{xmlFile}'. Did you use BMFont to generate?");

            if (new XmlSerializer(typeof(font)).Deserialize(File.OpenRead(xmlFile)) is not font fontDefinition)
                throw new Exception($"File '{xmlFile}' is not valid. Did you use BMFont to generate?");

            var common = fontDefinition.Items.OfType<fontCommon>().First();
            var @base = int.Parse(common.@base);
            var atlasWidth = int.Parse(common.scaleW);
            var atlasHeight = int.Parse(common.scaleH);
            var lineHeight = int.Parse(common.lineHeight);

            var glyphs = ParseGlyphs(fontDefinition, atlasWidth, atlasHeight);
            var kernings = ParseKernings(fontDefinition);

            return new Font(pngFile, lineHeight, @base, glyphs, kernings);
        }

        private static List<Kerning> ParseKernings(font fontDefinition)
        {
            var kerningsIn = fontDefinition.Items.OfType<fontKernings>().Single();
            var kerningCount = int.Parse(kerningsIn.count);
            var kernings = new List<Kerning>(kerningCount);
            foreach (var kerning in kerningsIn.kerning)
            {
                kernings.Add(new Kerning(
                    ushort.Parse(kerning.first),
                    ushort.Parse(kerning.second),
                    int.Parse(kerning.amount)
                ));
            }

            return kernings;
        }

        private static List<Glyph> ParseGlyphs(font fontDefinition, int atlasWidth, int atlasHeight)
        {
            var fontChars = fontDefinition.Items.OfType<fontChars>().Single();
            var charCount = int.Parse(fontChars.count);
            var glyphs = new List<Glyph>(charCount);
            foreach (var @char in fontChars.@char)
            {
                var x = (float)int.Parse(@char.x);
                var y = (float)int.Parse(@char.y);
                var w = int.Parse(@char.width);
                var h = int.Parse(@char.height);

                glyphs.Add(new Glyph(
                    int.Parse(@char.id),
                    new Vector2(x/atlasWidth, y/atlasHeight),
                    new Vector2((x+w) / atlasWidth, (y+h) / atlasHeight),
                    w,
                    h,
                    int.Parse(@char.xoffset),
                    int.Parse(@char.yoffset),
                    int.Parse(@char.xadvance)
                ));
            }

            return glyphs;
        }
    }
}
