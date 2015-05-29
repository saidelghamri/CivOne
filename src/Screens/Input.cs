// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class Input : BaseScreen
	{
		public event EventHandler Accept;
		public event EventHandler Cancel;
		
		private string _text;
		private int _fontId;
		private byte _textColour, _cursorColour;
		private int _x, _y, _width, _height;
		private int _maxLength;
		private int _cursorPosition = -1;
		private bool _update = true;
		
		public string Text
		{
			get
			{
				return _text;
			}
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			_canvas.FillRectangle(0, 0, 0, 320, 200);
			int fontHeight = Resources.Instance.GetFontHeight(_fontId);
			int cursorPosition = _cursorPosition;
			if (cursorPosition < 0) cursorPosition = 0;
			
			int xx = _x;
			int yy = _y + (int)Math.Ceiling((float)(_height - fontHeight) / 2);
			
			if (gameTick % 4 < 2)
			{
				for (int i = 0; i <= _text.Length; i++)
				{
					int letterWidth = (_text.Length <= i) ? 7 : Resources.Instance.GetLetterSize(_fontId, _text[i]).Width;
					
					if (i == cursorPosition)
					{
						_canvas.FillRectangle(_cursorColour, xx, _y, letterWidth + 1, _height);
						break;
					}
					
					xx += letterWidth + 1;
				}
			}
			if (_text.Length > 0)
				_canvas.DrawText(_text, _fontId, _textColour, _x, yy);
			
			_update = false;
			return true;
		}
		
		public override bool KeyDown(KeyEventArgs args)
		{
			StringBuilder sb;
			switch (args.KeyCode)
			{
				case Keys.Left:
					if (_cursorPosition > 0) _cursorPosition--;
					else _cursorPosition = 0;
					_update = true;
					return true;
				case Keys.Right:
					if (_cursorPosition < 0) _cursorPosition = 0;
					if (_cursorPosition < _text.Length) _cursorPosition++;
					else _cursorPosition = _text.Length;
					_update = true;
					return true;
				case Keys.Escape:
					if (Cancel != null)
						Cancel(this, null);
					break;
				case Keys.Enter:
					if (Accept != null)
						Accept(this, null);
					break;
				case Keys.Delete:
					//TODO: Handle delete
					break;
				case Keys.Back:
					if (_cursorPosition <= 0) return false;
					
					sb = new StringBuilder(_text);
					sb.Remove(--_cursorPosition, 1);
					_text = sb.ToString();
					
					_update = true;
					return true;
				default:
					char c = (char)(args.KeyCode);
					if (!args.Shift) c = Char.ToLower(c);
					if (args.KeyData == Keys.OemPeriod) c = '.';
					if (args.KeyData == Keys.Oemcomma) c = ',';
					if (args.Shift && (c >= '0' && c <= '9')) c -= (char)16;
					if (!Resources.Instance.ValidCharacter(_fontId, c)) return false;
					
					if (_cursorPosition == -1)
					{
						_text = string.Empty;
						_cursorPosition = 0;
					}
					
					sb = new StringBuilder(_text);
					if (_text.Length > _cursorPosition)
					{
						sb[_cursorPosition] = c;
					}
					else
					{
						sb.Append(c);
					}
					_text = sb.ToString();
					_cursorPosition++;
					while (_cursorPosition >= _maxLength) _cursorPosition--;
					
					_update = true;
					return true;
			}
			return false;
		}
		
		public void Close()
		{
			Destroy();
		}
		
		public Input(Color[] colours, string text, int fontId, byte textColour, byte cursorColour, int x, int y, int width, int height, int maxLength)
		{
			_canvas = new Picture(320, 200, colours);
			_text = text;
			_fontId = fontId;
			_textColour = textColour;
			_cursorColour = cursorColour;
			_x = x;
			_y = y;
			_width = width;
			_height = height;
			_maxLength = maxLength;
		}
		
		public Input(Color[] colours, int fontId, byte textColour, byte cursorColour, int x, int y, int width, int height, int maxLength)
		{
			_canvas = new Picture(320, 200, colours);
			_text = "";
			_fontId = fontId;
			_textColour = textColour;
			_cursorColour = cursorColour;
			_x = x;
			_y = y;
			_width = width;
			_height = height;
			_maxLength = maxLength;
		}
	}
}