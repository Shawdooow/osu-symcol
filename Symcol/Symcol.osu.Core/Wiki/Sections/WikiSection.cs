﻿using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using OpenTK;
using Symcol.osu.Core.Containers.Text;
using Symcol.osu.Core.Wiki.Sections.SectionPieces;

namespace Symcol.osu.Core.Wiki.Sections
{
    public abstract class WikiSection : FillFlowContainer
    {
        public abstract string Title { get; }

        public readonly ClickableOsuSpriteText SectionHeaderText;

        public virtual WikiSubSection[] SubSections => null;

        private readonly FillFlowContainer content;

        protected override Container<Drawable> Content => content;

        protected WikiSection()
        {
            OsuColour osu = new OsuColour();
            Direction = FillDirection.Vertical;
            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;
            InternalChildren = new Drawable[]
            {
                SectionHeaderText = new ClickableOsuSpriteText
                {
                    Colour = osu.Yellow,
                    Text = Title,
                    TextSize = 32,
                    Font = @"Exo2.0-Bold",
                    Margin = new MarginPadding
                    {
                        Horizontal = WikiOverlay.CONTENT_X_MARGIN,
                        Vertical = 12
                    }
                },
                content = new FillFlowContainer
                {
                    Direction = FillDirection.Vertical,
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Padding = new MarginPadding
                    {
                        Horizontal = WikiOverlay.CONTENT_X_MARGIN,
                        Bottom = 20
                    }
                },
                new Box
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 1,
                    Colour = OsuColour.Gray(34),
                    EdgeSmoothness = Vector2.One
                }
            };
        }
    }
}
