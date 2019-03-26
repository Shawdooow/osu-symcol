﻿#region usings

using osu.Game.Rulesets.Vitaru.ChapterSets.Vitaru.HitObjects.DrawableHitObjects;
using osu.Game.Rulesets.Vitaru.Ruleset.Containers.Playfields;
using osu.Game.Rulesets.Vitaru.Ruleset.HitObjects;
using osu.Game.Rulesets.Vitaru.Ruleset.HitObjects.Drawables;

#endregion

namespace osu.Game.Rulesets.Vitaru.ChapterSets.Touhosu.HitObjects.DrawableHitObjects
{
    public class DrawableTouhosuCluster : DrawableVitaruCluster
    {
        public DrawableTouhosuCluster(TouhosuCluster cluster)
            : base(cluster)
        {
        }

        public DrawableTouhosuCluster(TouhosuCluster cluster, VitaruPlayfield playfield)
            : base(cluster, playfield)
        {
        }
    }
}
