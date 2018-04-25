﻿using eden.Game.GamePieces;
using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Configuration;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Vitaru.Settings;
using osu.Game.Rulesets.Vitaru.UI;
using Symcol.Rulesets.Core.Rulesets;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Vitaru
{
    public class VitaruInputManager : SymcolInputManager<VitaruAction>
    {
        private Bindable<bool> debugUI = VitaruSettings.VitaruConfigManager.GetBindable<bool>(VitaruSetting.DebugOverlay);
        private Bindable<bool> comboFire = VitaruSettings.VitaruConfigManager.GetBindable<bool>(VitaruSetting.ComboFire);
        private Bindable<GraphicsPresets> graphics = VitaruSettings.VitaruConfigManager.GetBindable<GraphicsPresets>(VitaruSetting.GraphicsPresets);

        protected override bool VectorVideo => VitaruSettings.VitaruConfigManager.GetBindable<bool>(VitaruSetting.VectorVideos);

        protected Container<Drawable> BlurContainer = new Container<Drawable> { RelativeSizeAxes = Axes.Both, Masking = false, AlwaysPresent = true, Name = "BlurContainer" };

        public readonly Box Shade;

        public VitaruInputManager(RulesetInfo ruleset, int variant) : base(ruleset, variant, SimultaneousBindingMode.Unique)
        {
            if (graphics.Value == GraphicsPresets.Standard)
                Add(Shade = new Box { RelativeSizeAxes = Axes.Both, Alpha = 0, Colour = Color4.Orange });

            if (debugUI)
                Add(new DebugValueUI { Anchor = Anchor.CentreLeft, Origin = Anchor.CentreLeft, Position = new Vector2(10, 0) });

            if (false)//comboFire)
                Add(new ComboFire());

            Add(BlurContainer.WithEffect(new GlowEffect
            {
                Strength = 1f,
                BlurSigma = new Vector2(8),
                Colour = Color4.Cyan.Opacity(0.5f)
            }));
        }

        bool blured;
        public void ToggleBlur()
        {
            List<Drawable> drawables = new List<Drawable>();

            if (!blured)
            {
                foreach (Drawable drawable in Children)
                    if (drawable != BlurContainer.Parent)
                        drawables.Add(drawable);

                foreach (Drawable drawable in drawables)
                {
                    Remove(drawable);
                    BlurContainer.Add(drawable);
                }
            }
            else
            {
                foreach (Drawable drawable in BlurContainer.Children)
                    drawables.Add(drawable);

                foreach (Drawable drawable in drawables)
                {
                    BlurContainer.Remove(drawable);
                    Add(drawable);
                }
            }

            blured = !blured;
        }
    }

    public enum VitaruAction
    {
        None = -1,

        //Movement
        Left = 0,
        Right,
        Up,
        Down,

        //Self-explaitory
        Shoot,
        Spell,

        //Slows the player + reveals hitbox
        Slow,

        //Sakuya
        Increase,
        Decrease,

        //Kokoro
        RightShoot,
        LeftShoot,

        //Nue
        Spell2,
        Spell3,
        Spell4
    }
}
