using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Parsers.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject.Tests.Parser.Option
{
    [TestClass]
    public class OptionParserTest
    {
        private static readonly List<IOptionParser> _instance = new(){
            new MainCore.Parsers.Implementations.TravianOfficial.OptionParser(),
            new MainCore.Parsers.Implementations.TTWars.OptionParser()
        };

        private static List<string> _version;

        private readonly HtmlDocument _doc = new();

        private readonly string _path = Path.Combine("Tests", "Parser", "Option", "HtmlFiles");

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _version = Enum.GetNames(typeof(VersionEnums)).ToList();

            Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        public void GetOptionsButtonTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var node = _instance[index].GetOptionsButton(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        public void GetContextualHelpCheckBoxTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var node = _instance[index].GetContextualHelpCheckBox(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        public void GetSaveButtonTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var node = _instance[index].GetSaveButton(_doc);
            Assert.IsNotNull(node);
        }
    }
}