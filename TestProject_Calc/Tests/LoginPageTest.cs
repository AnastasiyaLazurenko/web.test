using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using TestProject_Calc.Pages;

namespace TestProject_Calc.Tests
{
    public class LoginPageTest : BaseTest
    {
        [SetUp]
        public void OpenLoginPage()
        {
            driver.Navigate().GoToUrl(LoginPage.URL);
        }

        [Test]
        [TestCase("test", "newyork1")]
        [TestCase("TEST", "newyork1")]
        public void Login_InsertValidValue_LoginOccurs(string login, string password)
        {
            loginPage.EnterCredentialsAndLogin(login, password);
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(40));
            wait.Until(ExpectedConditions.UrlContains(CalculatorPage.URL));

            Assert.That(driver.Url, Is.EqualTo(CalculatorPage.URL), "The calculator page was not opened");
        }

        [Test]
        [TestCase("", "", "Please enter your user name and password")]
        [TestCase("", "newyork1", "Please enter your user name")]
        [TestCase("tes", "newyork1", "Your username or password is incorrect")]
        [TestCase("test", "", "Please enter your password")]
        [TestCase("test", "1", "Your username or password is incorrect")]
        [TestCase("test", "NEWYORK1", "Your username or password is incorrect")]
        public void Login_InvalidCredentials_ErrorMessageShown(string login, string password, string errorText)
        {
            loginPage.EnterCredentialsAndLogin(login, password);
            Assert.That(loginPage.ErrorMessageText, Is.EqualTo(errorText));
        }
    }
}
