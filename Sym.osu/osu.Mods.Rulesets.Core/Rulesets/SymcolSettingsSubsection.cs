﻿#region usings

using osu.Core;
using osu.Core.Config;
using osu.Framework.Allocation;
using osu.Framework.Platform;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets;

#endregion

namespace osu.Mods.Rulesets.Core.Rulesets
{
    public abstract class SymcolSettingsSubsection : RulesetSettingsSubsection
    {
        protected SymcolSettingsSubsection(Ruleset ruleset)
            : base(ruleset)
        {
        }

        [BackgroundDependencyLoader]
        private void load(Storage storage)
        {
            if (SymManager.SymConfigManager == null)
                SymManager.SymConfigManager = new SymConfigManager(storage);
        }
    }
}
