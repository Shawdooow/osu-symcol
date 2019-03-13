﻿using System.Threading.Tasks;
using osu.Core.Containers.Shawdooow;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets;
using osu.Mods.Online.Base;
using osu.Mods.Online.Multi.Packets.Match;
using osu.Mods.Online.Multi.Rulesets;
using osu.Mods.Online.Multi.Settings;
using osuTK;
using osuTK.Graphics;
using Symcol.Base.Graphics.Containers;
using Symcol.Networking.Packets;

namespace osu.Mods.Online.Multi.Screens.Pieces
{
    public class MatchTools : MultiplayerContainer
    {
        public readonly Bindable<MatchScreenMode> Mode = new Bindable<MatchScreenMode>() { Default = MatchScreenMode.MapDetails };

        public readonly Bindable<MatchGamemode> GameMode = new Bindable<MatchGamemode>() { Default = MatchGamemode.HeadToHead };

        public readonly Bindable<MatchObjective> Objective = new Bindable<MatchObjective>() { Default = MatchObjective.Score };

        public readonly Bindable<bool> LiveSpectator = new Bindable<bool>() { Default = false };

        public readonly Bindable<bool> Powerups = new Bindable<bool>() { Default = false };

        public readonly OsuTabControl<MatchScreenMode> TabControl;

        public readonly SymcolContainer SelectedContent;

        private readonly MapDetails mapDetails = new MapDetails();

        public BeatmapInfo SelectedBeatmap { get; private set; }

        public RulesetInfo SelectedRuleset { get; private set; }

        private int? selectedBeatmapSetID;

        private RulesetStore rulesets;

        private Bindable<RulesetInfo> ruleset;

        public BeatmapManager Beatmaps { get; private set; }

        private Bindable<WorkingBeatmap> beatmap = new Bindable<WorkingBeatmap>();

        private bool searching;

        public MatchTools(OsuNetworkingHandler osuNetworkingHandler) : base(osuNetworkingHandler)
        {
            Masking = true;
            CornerRadius = 16;
            Anchor = Anchor.TopRight;
            Origin = Anchor.TopRight;
            RelativeSizeAxes = Axes.Both;
            Width = 0.49f;
            Height = 0.45f;
            Position = new Vector2(-10, 10);

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black.Opacity(0.8f)
                },
                TabControl = new OsuTabControl<MatchScreenMode>
                {
                    Position = new Vector2(72, 0),
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.08f,
                    Width = 0.8f
                },
                SelectedContent = new SymcolContainer
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.92f
                }
            };
            TabControl.Current.Value = MatchScreenMode.MapDetails;

            Mode.ValueChanged += value =>
            {
                if (SelectedContent.Children.Count == 1)
                    SelectedContent.Remove(SelectedContent.Child);

                switch (value)
                {
                    case MatchScreenMode.MapDetails:
                        SelectedContent.Child = mapDetails;

                        if (SelectedBeatmap != null)
                            mapDetails.SetMap(Beatmaps.GetWorkingBeatmap(SelectedBeatmap));
                        else if (selectedBeatmapSetID != 0)
                            mapDetails.SetMap(selectedBeatmapSetID);
                        else
                            mapDetails.SetMap(searching, selectedBeatmapSetID);
                        break;
                    case MatchScreenMode.MatchSettings:
                        SelectedContent.Child = new Container
                        {
                            RelativeSizeAxes = Axes.Both,

                            Children = new Drawable[]
                            {
                                new MultiplayerDropdownEnumOption<MatchObjective>(OsuNetworkingHandler, Objective, "Match Objective", 4),
                                new MultiplayerDropdownEnumOption<MatchGamemode>(OsuNetworkingHandler, GameMode, "Match Gamemode", 3),
                                new MultiplayerToggleOption(OsuNetworkingHandler, Powerups, "Combo Powerups", 2),
                                new MultiplayerToggleOption(OsuNetworkingHandler, LiveSpectator, "Live Spectator", 1),
                            }
                        };
                        break;
                    case MatchScreenMode.RulesetSettings:
                        if (SelectedRuleset.CreateInstance() is IRulesetMulti multiRuleset)
                            SelectedContent.Child = multiRuleset.RulesetSettings(OsuNetworkingHandler);
                        break;
                    case MatchScreenMode.SoundBoard:
                        SelectedContent.Child = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = new HitSoundBoard
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                ButtonSize = 80
                            }
                        };
                        break;
                }
            };
            Mode.BindTo(TabControl.Current);
            Mode.TriggerChange();
        }

        [BackgroundDependencyLoader]
        private void load(RulesetStore rulesets, Bindable<RulesetInfo> ruleset, Bindable<WorkingBeatmap> beatmap, BeatmapManager beatmaps)
        {
            this.ruleset = ruleset;
            this.rulesets = rulesets;
            this.beatmap.BindTo(beatmap);
            Beatmaps = beatmaps;
        }

        protected override void OnPacketRecieve(PacketInfo info)
        {
            if (info.Packet is SetMapPacket mapPacket)
                Task.Factory.StartNew(() =>
                {
                    searching = true;

                    //Tell the user we are searching!
                    MapChange(mapPacket.Map.OnlineBeatmapSetID, mapPacket.Map.RulesetShortname);

                    foreach (BeatmapSetInfo beatmapSet in Beatmaps.GetAllUsableBeatmapSets())
                        if (mapPacket.Map.OnlineBeatmapID != -1 && beatmapSet.OnlineBeatmapSetID == mapPacket.Map.OnlineBeatmapSetID)
                        {
                            foreach (BeatmapInfo b in beatmapSet.Beatmaps)
                                if (b.OnlineBeatmapID == mapPacket.Map.OnlineBeatmapID)
                                {
                                    ruleset.Value = rulesets.GetRuleset(mapPacket.Map.RulesetShortname);
                                    beatmap.Value = Beatmaps.GetWorkingBeatmap(b, beatmap);
                                    beatmap.Value.Track.Start();

                                    MapChange(b);
                                    goto complete;
                                }

                            break;
                        }
                        //try to fallback for old maps
                        else if (mapPacket.Map.BeatmapTitle == beatmapSet.Metadata.Title && mapPacket.Map.BeatmapMapper == beatmapSet.Metadata.Author.Username)
                        {
                            foreach (BeatmapInfo b in beatmapSet.Beatmaps)
                                if (mapPacket.Map.BeatmapDifficulty == b.Version)
                                {
                                    ruleset.Value = rulesets.GetRuleset(mapPacket.Map.RulesetShortname);
                                    beatmap.Value = Beatmaps.GetWorkingBeatmap(b, beatmap);
                                    beatmap.Value.Track.Start();

                                    MapChange(b);
                                    goto complete;
                                }

                            break;
                        }

                    //Tell the user they are missing this / we couldn't find it
                    mapDetails.SetMap(selectedBeatmapSetID);

                    complete:;
                    searching = false;

                }, TaskCreationOptions.LongRunning);
        }

        public void MapChange(BeatmapInfo info)
        {
            if (info == null)
            {
                MapChange(-1, "osu");
                return;
            }

            SelectedBeatmap = info;

            WorkingBeatmap working = Beatmaps.GetWorkingBeatmap(info);

            try
            {
                selectedBeatmapSetID = working.BeatmapSetInfo.OnlineBeatmapSetID;
            }
            catch
            {
                selectedBeatmapSetID = -1;
            }

            if (Mode.Value == MatchScreenMode.MapDetails)
                mapDetails.SetMap(working);
        }

        public void MapChange(int? onlineBeatmapSetID, string rulesetShortname)
        {
            SelectedRuleset = rulesets.GetRuleset(rulesetShortname);
            SelectedBeatmap = null;
            selectedBeatmapSetID = onlineBeatmapSetID;

            if (Mode.Value == MatchScreenMode.MapDetails)
            {
                if (selectedBeatmapSetID != 0 && selectedBeatmapSetID != -1)
                    mapDetails.SetMap(true, selectedBeatmapSetID);
                else
                    mapDetails.SetMap(false, selectedBeatmapSetID);
            }
        }
    }

    public enum MatchGamemode
    {
        [System.ComponentModel.Description("Head to Head")]
        HeadToHead,
        [System.ComponentModel.Description("Knockout")]
        Knockout,
        [System.ComponentModel.Description("Team Versus")]
        TeamVS,
        [System.ComponentModel.Description("TAG4")]
        TAG4,
        [System.ComponentModel.Description("Team TAG4")]
        TeamTAG4,
        [System.ComponentModel.Description("Tourny Mode")]
        Tournement,
    }

    public enum MatchObjective
    {
        [System.ComponentModel.Description("Score")]
        Score,
        [System.ComponentModel.Description("Accuracy")]
        Accuracy,
        [System.ComponentModel.Description("Combo")]
        Combo,
    }

    public enum MatchScreenMode
    {
        [System.ComponentModel.Description("Sound Board")]
        SoundBoard,
        [System.ComponentModel.Description("Ruleset Settings")]
        RulesetSettings,
        [System.ComponentModel.Description("Match Settings")]
        MatchSettings,
        [System.ComponentModel.Description("Map Details")]
        MapDetails
    }
}
