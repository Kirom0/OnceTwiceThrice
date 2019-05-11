using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OnceTwiceThrice
{
	public static class Useful
	{
		public static MovableBase GetNextHero(IEnumerator<MovableBase> enumerator)
		{
			if (enumerator.MoveNext())
				return enumerator.Current;
			enumerator.Reset();
			return GetNextHero(enumerator);
		}
		
		public static bool KeyIsMove(Keys key)
		{
			return
				key == Keys.Up ||
				key == Keys.Down ||
				key == Keys.Left ||
				key == Keys.Right;
		}

		public static Image GetImageByName(string name)
		{
			return Image.FromFile("../../images/" + name + ".png");
		}

		public static void XyPlusKeys(int x, int y, Keys key, ref int newX, ref int newY)
		{
			var dx = 0;
			var dy = 0;
			switch (key)
			{
				case Keys.Up: dy = -1; break;
				case Keys.Down: dy = 1; break;
				case Keys.Left: dx = -1; break;
				case Keys.Right: dx = 1; break;
				default: throw new ArgumentException();
			}
			newX = x + dx;
			newY = y + dy;
		}

		public static string DirectionName(Keys key)
		{
			switch (key)
			{
				case Keys.Up: return "Up";
				case Keys.Down: return "Down";
				case Keys.Left: return "Left";
				case Keys.Right: return "Right";
				default: throw new ArgumentException();
			}
		}

		public static string[] CutArray(string[] array, int begin, int end)
		{
			var result = new string[end - begin + 1];
			for (var i = begin; i <= end; i++)
				result[i - begin] = array[i];
			return result;
		}
	}
}
