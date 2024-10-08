using OpenQA.Selenium.Chrome;

namespace TestProject_Calc
{
    public class LoginPageTest
    {
        private ChromeDriver _driver;
        private LoginPage _loginPage;

        [OneTimeSetUp]
        public void SetUpDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(90);
            _loginPage = new LoginPage(_driver);
        }

        [SetUp]
        public void OpenLoginPage()
        {
            _driver.Navigate().GoToUrl(LoginPage.URL);
        }

        [OneTimeTearDown]
        public void TearDownTest()
        {
            _driver.Close();
        }

        [Test]
        [TestCase("test", "newyork1")]
        public void Login_InsertValidValue_LoginOccurs(string login, string password)
        {
            TestContext.Error.WriteLine($"Start loading the page");
            _loginPage.EnterCredentialsAndLogin(login, password);
            TestContext.Error.WriteLine(_driver.Url);

            Assert.That(_driver.Url, Is.EqualTo(CalculatorPage.URL), "The calculator page was not opened");
        }

        [Test]
        [TestCase("", "", "Please enter your user name and password")]
        [TestCase("", "newyork1", "Please enter your user name")]
        [TestCase("tes", "newyork1", "Your username or password is incorrect")]
        [TestCase("test", "", "Please enter your password")]
        [TestCase("test", "1", "Your username or password is incorrect")]
        [TestCase("TEST", "newyork1", "Your username or password is incorrect")]
        [TestCase("test", "NEWYORK1", "Your username or password is incorrect")]
        public void Login_InvalidCredentials_ErrorMessageShown(string login, string password, string errorText)
        {
            _loginPage.EnterCredentialsAndLogin(login, password);
            Assert.That(_loginPage.ErrorMessageText, Is.EqualTo(errorText));
        }
    }
}
