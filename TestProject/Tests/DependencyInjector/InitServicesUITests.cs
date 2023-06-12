﻿using MainCore.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splat;
using System;
using System.Collections.Generic;
using TestProject.Tests.Mock;
using WPFUI;

namespace TestProject.Tests.DependencyInjector
{
    [TestClass]
    public class InitServicesUITests
    {
        [DataTestMethod, Timeout(10000)]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void TestInit(VersionEnums version, Type type)
        {
            AppBoostrapper.Init(version);
            var result = Locator.Current.GetService(type);
            Assert.IsNotNull(result);
        }

        private static IEnumerable<object[]> GetTestData()
        {
            return ServiceData.GetUIService();
        }
    }
}