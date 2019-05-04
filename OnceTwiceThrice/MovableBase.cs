﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml.Linq;

namespace OnceTwiceThrice
{
	public class Animation
	{
		public bool IsMoving { get; set; }
		public Keys Direction { get; set; }
		public Animation()
		{
			IsMoving = false;
			Direction = Keys.None;
		}
	}

	public interface IMovable
	{
		Image Image { get; }
		KeyMap KeyMap { get; set; }
		void MakeMove(Keys key);
		void MakeAnimation();
		int X { get; }
		int Y { get; }
		double DX { get; }
		double DY { get; }
	}

	public class MovableBase : IMovable
	{
		public Image Image {
			get
			{
				switch (lastDirection)
				{
					case Keys.Up: return goUp[0];
					case Keys.Down: return goDown[0];
					case Keys.Right: return goRight[0];
					case Keys.Left: return goLeft[0];
				}

				return goDown[0];
			}
		}

		private Keys lastDirection;
		
		private List<Image> goUp;
		private List<Image> goDown;
		private List<Image> goRight;
		private List<Image> goLeft;
		
	

		public GameModel Model;

		public event Action OnStop; //Вызывается при завершени анимации шага
		public event Action<Keys> OnCantMove; //Вызывается при невозможности двигаться в заданном направлении
		
		public KeyMap KeyMap { get; set; }

		public int X { get; private set; }
		public int Y { get; private set; }

		public double DX { get; private set; }
		public double DY { get; private set; }

		public Animation CurrentAnimation;

		public virtual double Speed
		{
			get { return 0.05; }
		}

		public MovableBase(GameModel model, string ImageFile, int X, int Y)
		{
			this.Model = model;
			KeyMap = new KeyMap();
			
			goUp = new List<Image>();
			goDown = new List<Image>();
			goRight = new List<Image>();
			goLeft = new List<Image>();
			
			goUp.Add(Helpful.GetImageByName(ImageFile + "Up"));
			goDown.Add(Helpful.GetImageByName(ImageFile + "Down"));
			goRight.Add(Helpful.GetImageByName(ImageFile + "Right"));
			goLeft.Add(Helpful.GetImageByName(ImageFile + "Left"));
			lastDirection = Keys.Down;
			
			this.X = X;
			this.Y = Y;
			CurrentAnimation = new Animation();

			OnStop += () =>
			{
				if (KeyMap[CurrentAnimation.Direction] &&
				    Model.IsInsideMap(X, Y, CurrentAnimation.Direction))
				{
					MakeMove(CurrentAnimation.Direction);
					return;
				}

				var nextDirection = KeyMap.GetAnyOnDirection();
				if (nextDirection != Keys.None)
					MakeMove(nextDirection);
			};
		}
		public void MakeMove(Keys key)
		{
			lastDirection = key;
			if (CurrentAnimation.IsMoving)
				return;

			//if (Model.IsInsideMap(X, Y, key))
			if (AllowToMove(key))
			{
				DX = DY = 0;
//				switch (key)
//				{
//					case Keys.Up: Y--; DY = 1; break;
//					case Keys.Down: Y++; DY = -1; break;
//					case Keys.Left: X--; DX = 1; break;
//					case Keys.Right: X++; DX = -1; break;
//				}
				CurrentAnimation.IsMoving = true;
				CurrentAnimation.Direction = key;
			}
			else
			{
				if (OnCantMove != null) OnCantMove(key);
			}
		}

		public void MakeAnimation()
		{
			if (!CurrentAnimation.IsMoving)
				return;
			switch (CurrentAnimation.Direction)
			{
				case Keys.Up: DY += -Speed; break;
				case Keys.Down: DY += Speed; break;
				case Keys.Left: DX += -Speed; break;
				case Keys.Right: DX += Speed; break;
			}
			if (1 - Math.Abs(DX) < 0.01 || 1 - Math.Abs(DY) < 0.01)
			{
				if (1 - Math.Abs(DX) < -0.01 || 1 - Math.Abs(DY) < -0.01)
					throw new Exception("DX или DY > 1");
				X = (int)Math.Round(X + DX, 0);
				Y = (int)Math.Round(Y + DY, 0);
				DX = DY = 0;
				CurrentAnimation.IsMoving = false;
				if (OnStop != null)
					OnStop();
				
			}
		}

		//public virtual bool IsHero() => false;

		public bool AllowToMove(Keys key)
		{
			int newX = 0;
			int newY = 0;
			Helpful.XYPlusKeys(X, Y, key, ref newX, ref newY);
			if (!Model.IsInsideMap(newX, newY))
				return false;
			if (Model.ItemsMap[newX, newY].Count > 0)
				return CanMoveOn(Model.ItemsMap[newX, newY].Peek());
			return CanMoveOn(Model.BackMap[newX, newY]);
		}

		public bool CanMoveOn(IItems item) => item.CanStep(this) || CanStep(item);
		public bool CanMoveOn(IBackground background) => background.CanStep(this) || CanStep(background);

		public virtual bool CanStep(IItems item) => false;
		public virtual bool CanStep(IBackground background) => false;
	}
}
