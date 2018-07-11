﻿using osu.Framework.Graphics;
using OpenTK;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.Containers;
using osu.Framework.Audio.Track;
using osu.Game.Beatmaps.ControlPoints;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.MathUtils;
using osu.Game.Rulesets.Vitaru.Settings;
using osu.Framework.Extensions.Color4Extensions;

namespace osu.Game.Rulesets.Vitaru.Objects.Drawables.Pieces
{
    public class BulletPiece : BeatSyncedContainer
    {
        public override bool HandleMouseInput => false;
        public override bool HandleKeyboardInput => false;

        private readonly GraphicsOptions graphics = VitaruSettings.VitaruConfigManager.GetBindable<GraphicsOptions>(VitaruSetting.BulletVisuals);

        private readonly Sprite bulletKiai;
        private Sprite dean;
        private readonly CircularContainer circle;
        public readonly Box Box;

        public static bool ExclusiveTestingHax;

        private readonly float randomRotationValue = 1;
        private readonly bool randomRotateDirection;

        private readonly DrawableBullet drawableBullet;

        public BulletPiece(DrawableBullet drawableBullet)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            this.drawableBullet = drawableBullet;

            randomRotationValue = (float)RNG.Next(10, 15) / 10;
            randomRotateDirection = RNG.NextBool();

            Size = new Vector2((float)drawableBullet.Bullet.Diameter + 12);

            if (graphics != GraphicsOptions.HighPerformance && graphics != GraphicsOptions.StandardV2)
                Child = bulletKiai = new Sprite
                {
                    Scale = new Vector2(2),
                    Alpha = 0,
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Colour = drawableBullet.AccentColour,
                    Texture = VitaruRuleset.VitaruTextures.Get("bulletKiai"),
                };

            Add(circle = new CircularContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 1,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(graphics == GraphicsOptions.StandardV2 ? 1 : 1.05f),
                BorderColour = drawableBullet.AccentColour,
                BorderThickness = graphics == GraphicsOptions.StandardV2 ? 0 : 6,
                Masking = true,

                Child = Box = new Box
                {
                    RelativeSizeAxes = Axes.Both
                }
            });

            if (drawableBullet.Bullet.Shape == Shape.Triangle)
            {
                Add(new EquilateralTriangle
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                });
                Box.Alpha = 0;
            }

            if (graphics != GraphicsOptions.HighPerformance)
                circle.EdgeEffect = new EdgeEffectParameters
                {
                    Hollow = drawableBullet.Bullet.Shape == Shape.Circle ? true : false,
                    Radius = (float)drawableBullet.Bullet.Diameter,
                    Type = EdgeEffectType.Shadow,
                    Colour = drawableBullet.AccentColour.Opacity(graphics == GraphicsOptions.StandardV2 ? 0.5f : 0.2f)
                };
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, TrackAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);

            if (graphics != GraphicsOptions.HighPerformance && graphics != GraphicsOptions.StandardV2)
            {
                if (effectPoint.KiaiMode && bulletKiai.Alpha == 0)
                    bulletKiai.FadeInFromZero(timingPoint.BeatLength / 4);
                if (!effectPoint.KiaiMode && bulletKiai.Alpha == 1)
                    bulletKiai.FadeOutFromOne(timingPoint.BeatLength);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (graphics != GraphicsOptions.HighPerformance && graphics != GraphicsOptions.StandardV2 && bulletKiai.Alpha > 0)
            {
                if (randomRotateDirection)
                    bulletKiai.Rotation += (float)(Clock.ElapsedFrameTime / 1000 * 90) * randomRotationValue;
                else
                    bulletKiai.Rotation += (float)(-Clock.ElapsedFrameTime / 1000 * 90) * randomRotationValue;
            }

            if (ExclusiveTestingHax && circle.Alpha == 1)
            {
                circle.Alpha = 0;
                Add(dean = new Sprite
                {
                    RelativeSizeAxes = Axes.Both,
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Colour = drawableBullet.AccentColour.Lighten(0.4f),
                    Texture = VitaruRuleset.VitaruTextures.Get("Dean"),
                });
            }
            else if (!ExclusiveTestingHax && circle.Alpha == 0)
            {
                circle.Alpha = 1;
                Remove(dean);
                dean.Dispose();
            }

            if (ExclusiveTestingHax)
                dean.Rotation += (float)Clock.ElapsedFrameTime / 2;
        }
    }
}
