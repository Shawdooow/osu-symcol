﻿using System.Collections.Generic;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Vitaru.Mods;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Game.Overlays.Settings;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using System.Linq;
using osu.Game.Rulesets.Vitaru.Settings;
using osu.Game.Rulesets.Vitaru.Edit;
using osu.Game.Rulesets.Edit;
using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Vitaru.Beatmaps;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Vitaru.UI;
using osu.Game.Rulesets.Difficulty;
using osu.Framework.Audio;

namespace osu.Game.Rulesets.Vitaru
{
    public sealed class VitaruRuleset : Ruleset
    {
        public const string RULESET_VERSION = "0.8.4";

        public override int? LegacyID => 4; 

        public override string Description => "vitaru!";

        public override string ShortName => "vitaru";

        public override IEnumerable<int> AvailableVariants
        {
            get
            {
                for (int i = 0; i <= 3; i++)
                    yield return (int)ControlScheme.Vitaru + i;
            }
        }

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0)
        {
            switch (getControlType(variant))
            {
                case ControlScheme.Vitaru:
                    return new[]
                    {
                        new KeyBinding(InputKey.W, VitaruAction.Up),
                        new KeyBinding(InputKey.S, VitaruAction.Down),
                        new KeyBinding(InputKey.A, VitaruAction.Left),
                        new KeyBinding(InputKey.D, VitaruAction.Right),
                        new KeyBinding(InputKey.Shift, VitaruAction.Slow),
                        new KeyBinding(InputKey.MouseLeft, VitaruAction.Shoot),
                    };
                case ControlScheme.Dodge:
                    return new[]
                    {
                        new KeyBinding(InputKey.W, VitaruAction.Up),
                        new KeyBinding(InputKey.S, VitaruAction.Down),
                        new KeyBinding(InputKey.A, VitaruAction.Left),
                        new KeyBinding(InputKey.D, VitaruAction.Right),
                        new KeyBinding(InputKey.Shift, VitaruAction.Slow),
                    };
                case ControlScheme.Touhosu:
                    return new[]
                    {
                        new KeyBinding(InputKey.W, VitaruAction.Up),
                        new KeyBinding(InputKey.S, VitaruAction.Down),
                        new KeyBinding(InputKey.A, VitaruAction.Left),
                        new KeyBinding(InputKey.D, VitaruAction.Right),
                        new KeyBinding(InputKey.Shift, VitaruAction.Slow),
                        new KeyBinding(InputKey.MouseLeft, VitaruAction.Shoot),
                        new KeyBinding(InputKey.MouseRight, VitaruAction.Spell),
                    };
                case ControlScheme.Sakuya:
                    return new[]
                    {
                        new KeyBinding(InputKey.W, VitaruAction.Up),
                        new KeyBinding(InputKey.S, VitaruAction.Down),
                        new KeyBinding(InputKey.A, VitaruAction.Left),
                        new KeyBinding(InputKey.D, VitaruAction.Right),
                        new KeyBinding(InputKey.Shift, VitaruAction.Slow),
                        new KeyBinding(InputKey.MouseLeft, VitaruAction.Shoot),
                        new KeyBinding(InputKey.MouseRight, VitaruAction.Spell),
                        new KeyBinding(InputKey.E, VitaruAction.Increase),
                        new KeyBinding(InputKey.Q, VitaruAction.Decrease),
                    };
                case ControlScheme.Ryukoy:
                    return new[]
                    {
                        new KeyBinding(InputKey.W, VitaruAction.Up),
                        new KeyBinding(InputKey.S, VitaruAction.Down),
                        new KeyBinding(InputKey.A, VitaruAction.Left),
                        new KeyBinding(InputKey.D, VitaruAction.Right),
                        new KeyBinding(InputKey.Shift, VitaruAction.Slow),
                        new KeyBinding(InputKey.MouseLeft, VitaruAction.Shoot),
                        new KeyBinding(InputKey.MouseRight, VitaruAction.Spell),
                        new KeyBinding(InputKey.F, VitaruAction.Pull),
                    };
            }

            return new KeyBinding[0];
        }

        public override string GetVariantName(int variant)
        {
            switch (getControlType(variant))
            {
                default:
                    return "null";
                case ControlScheme.Vitaru:
                    return "Vitaru";
                case ControlScheme.Dodge:
                    return "Dodge";
                case ControlScheme.Touhosu:
                    return "Touhosu";
                case ControlScheme.Sakuya:
                    return "Sakuya";
                case ControlScheme.Ryukoy:
                    return "Ryukoy";
            }
        }

        private ControlScheme getControlType(int variant)
        {
            return (ControlScheme)Enum.GetValues(typeof(ControlScheme)).Cast<int>().OrderByDescending(i => i).First(v => variant >= v);
        }

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.DifficultyReduction:
                    return new Mod[]
                    {
                        new VitaruModEasy(),
                        new VitaruModNoFail(),
                        new MultiMod(new VitaruModHalfTime(), new VitaruModDaycore())
                    };
                case ModType.DifficultyIncrease:
                    return new Mod[]
                    {
                        new VitaruModHardRock(),
                        new MultiMod(new VitaruModDoubleTime(), new VitaruModNightcore())
                    };
                default: return new Mod[] { };
            }
        }

        public override RulesetContainer CreateRulesetContainerWith(WorkingBeatmap beatmap) => new VitaruRulesetContainer(this, beatmap);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new VitaruBeatmapConverter(beatmap);

        public override IBeatmapProcessor CreateBeatmapProcessor(IBeatmap beatmap) => new VitaruBeatmapProcessor(beatmap);

        public override DifficultyCalculator CreateDifficultyCalculator(WorkingBeatmap beatmap) => new VitaruDifficultyCalculator(this, beatmap);

        public override RulesetSettingsSubsection CreateSettings() => new VitaruSettings(this);

        public override Drawable CreateIcon() => new Sprite { Texture = VitaruTextures.Get("icon") };

        public override HitObjectComposer CreateHitObjectComposer() => new VitaruHitObjectComposer(this);

        public static ResourceStore<byte[]> VitaruResources;
        public static TextureStore VitaruTextures;
        public static AudioManager VitaruAudio;

        public VitaruRuleset(RulesetInfo rulesetInfo = null) : base(rulesetInfo)
        {
            if (VitaruResources == null)
            {
                VitaruResources = new ResourceStore<byte[]>();
                VitaruResources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore("osu.Game.Rulesets.Vitaru.dll"), "Assets"));
                VitaruResources.AddStore(new DllResourceStore("osu.Game.Rulesets.Vitaru.dll"));
                VitaruTextures = new TextureStore(new RawTextureLoaderStore(new NamespacedResourceStore<byte[]>(VitaruResources, @"Textures")));
                VitaruTextures.AddStore(new RawTextureLoaderStore(new OnlineStore()));

                var tracks = new ResourceStore<byte[]>(VitaruResources);
                tracks.AddStore(new NamespacedResourceStore<byte[]>(VitaruResources, @"Tracks"));
                tracks.AddStore(new OnlineStore());

                var samples = new ResourceStore<byte[]>(VitaruResources);
                samples.AddStore(new NamespacedResourceStore<byte[]>(VitaruResources, @"Samples"));
                samples.AddStore(new OnlineStore());

                VitaruAudio = new AudioManager(tracks, samples);
            }
        }
    }

    public enum Gamemodes
    {
        Vitaru,
        Gravaru,
        Dodge,
        Touhosu,
    }

    public enum ControlScheme
    {
        Vitaru,
        Dodge,

        Touhosu,

        Sakuya,
        Ryukoy
    }
}
