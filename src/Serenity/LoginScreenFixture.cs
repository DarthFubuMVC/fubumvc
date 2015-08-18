using System;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Dates;
using FubuCore.Reflection;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Endpoints;
using OpenQA.Selenium;
using Serenity.Fixtures;
using StoryTeller;

namespace Serenity
{
    public class LoginScreenFixture : ScreenFixture
    {
        public static By LoginSubmitButton = By.Id("login-submit");
        public static By LoginMessageText = By.Id("login-message");

        public LoginScreenFixture()
        {
            Title = "Login Screen";
        }

        [FormatAs("Recycle the browser")]
        public void RecycleTheBrowser()
        {
            Browser.Recycle();
        }

        [FormatAs("Go to the login screen")]
        public void OpenLoginScreen()
        {
            Navigation.NavigateTo(new LoginRequest());
        }

        [FormatAs("Logout")]
        public void Logout()
        {
            Navigation.NavigateTo(new LogoutRequest());
        }

        [FormatAs("Login as {user}/{password}")]
        public void Login(string user, string password)
        {
            // TODO -- need to fix this in Serenity
            Wait.Until(() => Driver.FindElements(By.Name("UserName")).Any());

            SetData(By.Name("UserName"), user);
            SetData(By.Name("Password"), password);

            Driver.FindElement(LoginSubmitButton).Click();
        }

        // TODO (checked) -- this needs to be in ScreenFixture, Serenity
        protected IWebElement findElement(Expression<Func<LoginRequest, object>> property)
        {
            return Driver.FindElement(By.Name(property.ToAccessor().Name));
        }

        [FormatAs("Check the 'Remember me' checkbox")]
        public void CheckRememberMe()
        {
            IWebElement checkbox = findElement(x => x.RememberMe);
            if (!checkbox.Selected)
            {
                checkbox.Click();
            }
        }

        [FormatAs("The displayed user name should be {UserName}")]
        public string CheckUserName()
        {
            return Driver.FindElement(By.Name("UserName")).Text;
        }

        [FormatAs("No message is shown")]
        public bool NoMessageIsShown()
        {
            return TheMessageShouldBe().IsEmpty();
        }

        [FormatAs("The message displayed should be {message}")]
        public string TheMessageShouldBe()
        {
            Wait.Until(() => Driver.FindElements(LoginMessageText).Any());

            return Driver.FindElement(LoginMessageText).Text;
        }

        [FormatAs("The user account locked out message should be displayed")]
        public bool TheLockedOutMessageShouldBeDisplayed()
        {
            string theMessage = TheMessageShouldBe().Trim();
            StoryTellerAssert.Fail(theMessage != LoginKeys.LockedOut.ToString(), () => "Was '{0}'".ToFormat(theMessage));

            return true;
        }

        [FormatAs("Should be on the login screen")]
        public bool IsOnTheLoginScreen()
        {
            string url = Urls.UrlFor(new LoginRequest(), "GET");


            var actual = Driver.Url;

            StoryTellerAssert.Fail(!actual.Matches(url), "The actual url was " + actual);

            return true;
        }

        [FormatAs("Should have moved off the login screen")]
        public bool IsNotOnTheLoginScreen()
        {
            string url = Urls.UrlFor(new LoginRequest(), "GET");
            return !Driver.Url.Matches(url);
        }

        [FormatAs("The url should now be {url}")]
        public bool TheUrlIs(string url)
        {
            StoryTellerAssert.Fail(Driver.Url.Matches(url), "The actual url is " + url);

            return true;
        }

        [FormatAs("Try to open the home page")]
        public void TryToGoHome()
        {
            Navigation.NavigateToHome();
        }

        [FormatAs("Should be on the home page")]
        public bool ShouldBeOnTheHomePage()
        {
            return Driver.Url.StartsWith(RootUrl);
        }

        [FormatAs("After {number} of minutes, reopen the login page")]
        public void ReopenTheLoginScreen(int number)
        {
            var clock = (Clock)Context.Service<IClock>();

            clock.RestartAtLocal(clock.UtcNow());

            OpenLoginScreen();
        }

        [FormatAs("After {number} minutes")]
        public void AfterMinutes(int number)
        {
            AdvanceTheClock(number.Minutes());
        }
    }
}