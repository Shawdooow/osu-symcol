﻿using System.Collections.Generic;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Vitaru.Objects;
using osu.Game.Rulesets.Vitaru.Objects.Drawables;
using osu.Game.Rulesets.Vitaru.Scoring;
using OpenTK;
using osu.Game.Rulesets.Vitaru.UI.Cursor;
using osu.Framework.Graphics.Cursor;
using osu.Game.Rulesets.Vitaru.Settings;
using osu.Framework.Configuration;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Rulesets.Vitaru.Debug;
// ReSharper disable PossibleLossOfFraction

namespace osu.Game.Rulesets.Vitaru.UI
{
    public class VitaruRulesetContainer : RulesetContainer<VitaruHitObject>
    {
        private readonly DebugStat<int> ranked;

        private readonly Bindable<bool> rankedFilter = VitaruSettings.VitaruConfigManager.GetBindable<bool>(VitaruSetting.RankedFilter);

        public VitaruRulesetContainer(Ruleset ruleset, WorkingBeatmap beatmap)
            : base(ruleset, beatmap)
        {
            //TODO: make this a function if it works
            if (DebugToolkit.GeneralDebugItems.Count > 0)
                DebugToolkit.GeneralDebugItems = new List<Container>();
            if (DebugToolkit.MachineLearningDebugItems.Count > 0)
                DebugToolkit.MachineLearningDebugItems = new List<Container>();

            DebugToolkit.GeneralDebugItems.Add(ranked = new DebugStat<int>(new Bindable<int>()) { Text = "Ranked" });
            VitaruPlayfield = new VitaruPlayfield((VitaruInputManager)KeyBindingInputManager);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (VitaruPlayfield.OnJudgement == null && !rankedFilter)
            {
                OsuColour osu = new OsuColour();
                ranked.Text = "Unranked (Bad Code)";
                ranked.SpriteText.Colour = osu.Yellow;
            }
            else if (rankedFilter)
                ranked.Text = "Ranked (Ranked Filter Disabled)";

            VitaruInputManager vitaruInputManager = (VitaruInputManager)KeyBindingInputManager;
            vitaruInputManager.DebugToolkit?.UpdateItems();
        }

        protected override void Update()
        {
            base.Update();

            if (Clock.ElapsedFrameTime > 1000)
                ranked.Bindable.Value += 1000;
            else if (Clock.ElapsedFrameTime > 1000 / 10)
                ranked.Bindable.Value += 100;
            else if (Clock.ElapsedFrameTime > 1000 / 30)
                ranked.Bindable.Value += 10;
            else if (Clock.ElapsedFrameTime > 1000 / 45)
                ranked.Bindable.Value += 5;
            else if (Clock.ElapsedFrameTime > 1000 / 60)
                ranked.Bindable.Value++;

            if (ranked.Bindable.Value >= 1000 && VitaruPlayfield.OnJudgement != null && rankedFilter)
            {
                OsuColour osu = new OsuColour();
                VitaruPlayfield.OnJudgement = null;
                ranked.SpriteText.Colour = osu.Yellow;
                ranked.Text = "Unranked (Bad PC)";
            }
        }

        protected override CursorContainer CreateCursor() => new GameplayCursor();

        public override ScoreProcessor CreateScoreProcessor() => new VitaruScoreProcessor(this);

        protected override Playfield CreatePlayfield() => VitaruPlayfield;

        public VitaruPlayfield VitaruPlayfield { get; }

        public override int Variant => (int)variant();

        private readonly Bindable<string> character = VitaruSettings.VitaruConfigManager.GetBindable<string>(VitaruSetting.Character);
        private readonly Bindable<Gamemodes> gamemode = VitaruSettings.VitaruConfigManager.GetBindable<Gamemodes>(VitaruSetting.Gamemode);

        private ControlScheme variant()
        {
            if (gamemode == Gamemodes.Vitaru)
                return ControlScheme.Vitaru;
            else if (gamemode == Gamemodes.Dodge)
                return ControlScheme.Dodge;
            else
            {
                switch (character.Value)
                {
                    default:
                        return ControlScheme.Touhosu;
                    case "SakuyaIzayoi":
                        return ControlScheme.Sakuya;
                    case "RyukoyHakurei":
                        return ControlScheme.Ryukoy;
                }
            }
        }

        public override PassThroughInputManager CreateInputManager() => new VitaruInputManager(Ruleset.RulesetInfo, Variant);

        protected override DrawableHitObject<VitaruHitObject> GetVisualRepresentation(VitaruHitObject h)
        {
            if (h is Bullet bullet)
                return new DrawableBullet(bullet, VitaruPlayfield);
            if (h is Laser laser)
                return new DrawableLaser(laser, VitaruPlayfield);
            if (h is Pattern pattern)
                return new DrawablePattern(pattern, VitaruPlayfield);
            return null;
        }

        //protected override FramedReplayInputHandler CreateReplayInputHandler(Replay replay) => new VitaruReplayInputHandler(replay);

        protected override Vector2 GetAspectAdjustedSize()
        {
            var aspectSize = new Vector2(DrawSize.Y * 10f / 16f, DrawSize.Y);

            if (gamemode == Gamemodes.Touhosu)
                aspectSize = new Vector2(DrawSize.Y * 20f / 16f, DrawSize.Y);
            else if (gamemode == Gamemodes.Dodge)
                aspectSize = new Vector2(DrawSize.Y * 4f / 3f, DrawSize.Y);

            return new Vector2(aspectSize.X / DrawSize.X, aspectSize.Y / DrawSize.Y);
        }

        protected override Vector2 PlayfieldArea => new Vector2(0.8f);
    }
}
