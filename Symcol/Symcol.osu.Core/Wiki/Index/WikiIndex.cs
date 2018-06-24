﻿using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using OpenTK;
using OpenTK.Graphics;
using Symcol.Core.Graphics.Containers;
using Symcol.osu.Core.Wiki.OverlayPieces;

namespace Symcol.osu.Core.Wiki.Index
{
    public class WikiIndex : SymcolContainer
    {
        public readonly Bindable<WikiSet> CurrentWikiSet = new Bindable<WikiSet>();

        private readonly OsuScrollContainer scrollContainer;

        private FillFlowContainer<WikiClickableOsuSpriteText> selectableWikis;

        public WikiIndex()
        {
            Anchor = Anchor.BottomLeft;
            Origin = Anchor.BottomLeft;

            RelativeSizeAxes = Axes.Y;
            Size = new Vector2(180, 0.88f);

            Children = new Drawable[]
            {
                new OsuSpriteText
                {
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,

                    Colour = Color4.White,
                    TextSize = 28,
                    Text = "Index"
                },
                scrollContainer = new OsuScrollContainer
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.9f
                }
            };

            CurrentWikiSet.ValueChanged += value =>
            {
                WikiSetStore.ReloadWikiSets();
                ReloadOptions();
            };
            CurrentWikiSet.TriggerChange();
        }

        public void ReloadOptions()
        {
            if (selectableWikis != null)
            {
                scrollContainer.Remove(selectableWikis);
                selectableWikis.Dispose();
            }

            scrollContainer.Add(selectableWikis = new FillFlowContainer<WikiClickableOsuSpriteText>
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y
            });

            foreach (WikiSet set in WikiSetStore.LoadedWikiSets)
            {
                WikiClickableOsuSpriteText button = new WikiClickableOsuSpriteText
                {
                    Text = set.Name,
                    TextSize = 18,
                };
                selectableWikis.Add(button);
                button.Action = () => { CurrentWikiSet.Value = set; };
            }
        }
    }
}
