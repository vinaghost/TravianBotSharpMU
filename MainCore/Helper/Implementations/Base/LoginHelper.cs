using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public sealed class LoginHelper : ILoginHelper
    {
        private readonly IChromeManager _chromeManager;

        private readonly IGeneralHelper _generalHelper;
        private readonly IUpdateHelper _updateHelper;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly ISystemPageParser _systemPageParser;
        private readonly IOptionParser _optionParser;

        public LoginHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory, ISystemPageParser systemPageParser, IUpdateHelper updateHelper, IOptionParser optionParser)
        {
            _chromeManager = chromeManager;
            _generalHelper = generalHelper;
            _contextFactory = contextFactory;
            _systemPageParser = systemPageParser;
            _updateHelper = updateHelper;
            _optionParser = optionParser;
        }

        public Result Execute(int accountId)
        {
            var result = AcceptCookie(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = Login(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            _updateHelper.Update(accountId);

            result = DisableContextualHelp(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        private Result AcceptCookie(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            if (html.DocumentNode.Descendants("a").Any(x => x.HasClass("cmpboxbtn") && x.HasClass("cmpboxbtnyes")))
            {
                var result = _generalHelper.Click(accountId, By.ClassName("cmpboxbtnyes"), false);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        private Result Login(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            var usernameNode = _systemPageParser.GetUsernameNode(html);

            var passwordNode = _systemPageParser.GetPasswordNode(html);

            var buttonNode = _systemPageParser.GetLoginButton(html);
            if (buttonNode is null)
            {
                return Result.Fail(new Skip("Login button not found."));
            }

            if (usernameNode is null)
            {
                return Result.Fail(new Retry("Cannot find username box"));
            }

            if (passwordNode is null)
            {
                return Result.Fail(new Retry("Cannot find password box"));
            }

            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(accountId);
            var access = context.Accesses.Where(x => x.AccountId == accountId).OrderByDescending(x => x.LastUsed).FirstOrDefault();

            var result = _generalHelper.Input(accountId, By.XPath(usernameNode.XPath), account.Username);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Input(accountId, By.XPath(passwordNode.XPath), access.Password);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Click(accountId, By.XPath(buttonNode.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private Result DisableContextualHelp(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var accountInfo = context.AccountsInfo.Find(accountId);
            if (accountInfo.IsContextualHelpDisabled) return Result.Ok();

            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var optionButton = _optionParser.GetOptionsButton(html);
            if (optionButton is null)
            {
                return Result.Fail(new Retry("Cannot find option button"));
            }

            var result = _generalHelper.Click(accountId, By.XPath(optionButton.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            html = chromeBrowser.GetHtml();

            var disableContextualHelpButton = _optionParser.GetContextualHelpCheckBox(html);
            if (disableContextualHelpButton is null)
            {
                return Result.Fail(new Retry("Cannot find disable contextual help button"));
            }

            var chrome = chromeBrowser.GetChrome();
            var buttonElement = chrome.FindElement(By.XPath(disableContextualHelpButton.XPath));
            if (!buttonElement.Selected)
            {
                buttonElement.Click();
            }

            var saveButton = _optionParser.GetSaveButton(html);
            if (saveButton is null)
            {
                return Result.Fail(new Retry("Cannot find save button"));
            }
            result = _generalHelper.Click(accountId, By.XPath(saveButton.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            accountInfo.IsContextualHelpDisabled = true;
            context.Update(accountInfo);
            context.SaveChanges();

            return Result.Ok();
        }
    }
}