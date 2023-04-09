﻿using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class UpgradeBuildingHelper : Base.UpgradeBuildingHelper
    {
        public UpgradeBuildingHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager, IChromeManager chromeManager, ISystemPageParser systemPageParser, IBuildingsHelper buildingsHelper, INavigateHelper navigateHelper, ILogManager logManager) : base(contextFactory, planManager, chromeManager, systemPageParser, buildingsHelper, navigateHelper, logManager)
        {
        }
    }
}