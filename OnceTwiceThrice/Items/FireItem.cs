using System.Drawing;

namespace OnceTwiceThrice
{
	public class FireItem : ItemBase, IItems
	{
		public Image Picture { get; }
		public FireItem(int x, int y) : base(x, y)
		{
			Picture = Helpful.GetImageByName("Fire");
		}

		public bool CanStep(MovableBase mob) => false;
		public bool CanStop(MovableBase mob) => true;
	}
}