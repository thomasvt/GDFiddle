using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace GDFiddle.UI.Text
{
    // how to render: http://www.angelcode.com/products/bmfont/doc/render_text.html

    public class Font
    {
        public string TextureFilename { get; }
        internal readonly int LineHeight;
        internal readonly int Base;
        internal readonly Dictionary<int, Glyph> Glyphs;
        private readonly Dictionary<uint, int> _kernings; // kerning-distance by First+Second charcode combined into uint.

        internal Font(string textureFilename, int lineHeight, int @base, List<Glyph> glyphs, List<Kerning> kernings)
        {
            TextureFilename = textureFilename;
            LineHeight = lineHeight;
            Base = @base;
            Glyphs = glyphs.ToDictionary(g => g.Code);
            _kernings = kernings.ToDictionary(k => (uint)(k.First << 16) | k.Second, k => k.Amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetKerningOffset(ushort first, ushort second)
        {
            var mask = (uint) (first << 16) | second;
            return _kernings.TryGetValue(mask, out var distance) 
                ? distance 
                : 0;
        }

        public static Font FromBMFontFile(string pngFile, string fntFile)
        {
            if (!File.Exists(pngFile))
                throw new FileNotFoundException($"Font file '{pngFile}' not found.");
            if (!File.Exists(fntFile))
                throw new FileNotFoundException($"Font file should have an accompanying fnt-file '{fntFile}'. Did you use BMFont to generate?");

            if (new XmlSerializer(typeof(font)).Deserialize(File.OpenRead(fntFile)) is not font fontDefinition)
                throw new Exception($"File '{fntFile}' is not valid. Did you use BMFont to generate?");

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

        public IEnumerable<GlyphInfo> GetTextGlyphs(int x, int y, string text)
        {
            // how to render text:  http://www.angelcode.com/products/bmfont/doc/render_text.html

            var previousCharCode = (ushort)0;
            for (var i = 0; i < text.Length; i++)
            {
                var code = (ushort)char.ConvertToUtf32(text, i);
                if (!Glyphs.TryGetValue(code, out var glyph))
                {
                    if (!Glyphs.TryGetValue('?', out glyph))
                        continue;
                }

                var min = new Vector2(x + glyph.XOffset, y + glyph.YOffset);
                var max = min + new Vector2(glyph.Width, glyph.Heigth);
                yield return new GlyphInfo(min, max, glyph.UVMin, glyph.UVMax);

                x += glyph.XAdvance + GetKerningOffset(previousCharCode, code);

                previousCharCode = code;
            }
        }

        public Vector2 Measure(string text)
        {
            var width = 0;
            var previousCharCode = (ushort)0;
            for (var i = 0; i < text.Length; i++)
            {
                var code = (ushort)char.ConvertToUtf32(text, i);
                if (!Glyphs.TryGetValue(code, out var glyph))
                {
                    if (!Glyphs.TryGetValue('?', out glyph))
                        continue;
                }

                width += glyph.XAdvance + GetKerningOffset(previousCharCode, code);

                previousCharCode = code;
            }

            return new Vector2(width, LineHeight);
        }
    }
}
