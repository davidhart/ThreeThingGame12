using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Imaging;

namespace TTG
{
    class BitmapFont
    {
        Texture2D _texture;

        int _charactersX;
        int _charactersY;
        int _spacing;
        int _characterWidth;
        int _characterHeight;

        public BitmapFont(string fontName, int charactersX, int charactersY)
        {
            _charactersX = charactersX;
            _charactersY = charactersY;
            _spacing = 0;

			_texture = new Texture2D(fontName, false);
			_texture.SetFilter(TextureFilterMode.Nearest);
            _characterWidth = _texture.Width / _charactersX;
            _characterHeight = _texture.Height / _charactersY;
        }

        public void SetSpacing(int spacing)
        {
            _spacing = spacing;
        }

        public Vector2 GetCharSize()
        {
            return new Vector2(_characterWidth, _characterHeight);
        }

        public void DrawText(SpriteBatch batch, string text, Vector2 position, float scale = 1)
        {
            DrawText(batch, text, position, new Rgba(255, 255, 255, 255), scale);
        }

        private void GetSourceRectFromIndex(int index, ref ImageRect r)
        {
            r = new ImageRect((index % _charactersX) * _characterWidth, 
                              (index / _charactersX) * _characterHeight, 
                              _characterWidth, 
                              _characterHeight);
        }

        private bool GetCharacterSourceRect(char character, ref ImageRect r, int characterOffset)
        {
            int charIndex = -1;

            if (character == 'x')
                charIndex = 36;
            if (character >= 'A' && character <= 'Z')
                charIndex = character - 'A';
            else if (character >= '0' && character <= '9')
                charIndex = character - '0' + 26;

            if (charIndex < 0)
                return false;

            GetSourceRectFromIndex(charIndex + characterOffset, ref r);

            return true;
        }

        public void DrawText(SpriteBatch batch, string text, Vector2 position, Rgba color, float scale = 1, int characterOffset = 0)
        {
            ImageRect sourceRect = new ImageRect();
            ImageRect destRect = new ImageRect(0, 0, (int)(_characterWidth * scale), (int)(_characterHeight * scale));
            bool draw = false;

            for (int i = 0; i < text.Length; ++i)
            {
                if (draw = GetCharacterSourceRect(text[i], ref sourceRect, characterOffset))
                {
                    destRect.X = (int)position.X;
                    destRect.Y = (int)position.Y;
                    batch.Draw(_texture, destRect, sourceRect, color);
                }

                if (draw || text[i] == ' ')
                {
                    position.X += (_characterWidth + _spacing) * scale;
                }
            }
        }
    }
}
