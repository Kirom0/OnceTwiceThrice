﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnceTwiceThrice
{
	public abstract class SpellBase
	{
		public Image Picture { get; private set; }
        public int X { get; }
        public int Y { get; }
		public readonly GameModel Model;
        public readonly IHero Hero;
        public event Action OnDestroy;
		public virtual int SlidesCount { get => 10; }
        protected int Interval;
        protected int StartTime;
		public List<Image> Slides;
		public List<Image> CastSlides;
		protected void Destroy()
        {
            OnDestroy?.Invoke();
        }
		private int SlideCounter;

		public SpellBase(IHero hero, int X, int Y, string imageFile)
		{
            Interval = 100;
			this.Model = hero.Model;
            this.Hero = hero;
			this.X = X;
			this.Y = Y;
			

			SlideCounter = 0;
			Slides = new List<Image>();
			CastSlides = new List<Image>(2);
			for (int i = 0; i < SlidesCount; i++)
				Slides.Add(Useful.GetImageByName(imageFile + "Spell/" + i));
			for (int i = 0; i < 2; i++)
				CastSlides.Add(Useful.GetImageByName(imageFile + Useful.DirectionName(Hero.GazeDirection) + "/Spell/" + i));

			Picture = Slides[SlideCounter++];
            StartTime = Model.TickCount;
            Hero.LockKeyMap();

            OnDestroy += () => Model.Spells.Remove(this as ISpell);

            Model.OnTick += onTick;
			Hero.Image = CastSlides[0];
		}

		private void ChangeSlide()
		{
			if(SlideCounter < SlidesCount)
				Picture = Slides[SlideCounter++];
			//if (SlideCounter == 8)
			//	Hero.UnlockKeyMap();
		}

        private void onTick()
        {
			if ((Model.TickCount - StartTime) == 10)
				Hero.Image = CastSlides[1];
			if ((Model.TickCount - StartTime) == 30)
				Hero.Image = CastSlides[0];
			if ((Model.TickCount - StartTime) == 40)
				Hero.Image = CastSlides[1];
			if ((Model.TickCount - StartTime) % 10 == 0)
				ChangeSlide();
			if ((Model.TickCount - StartTime) == 60)
			{
				Hero.UnlockKeyMap();
				Hero.UpdateImage();
			}
			if (Model.TickCount - StartTime > Interval)
            {
                Destroy();
				
				Model.OnTick -= onTick;
            }
        }
	}
}
