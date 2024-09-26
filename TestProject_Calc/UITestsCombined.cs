using OpenQA.Selenium.Chrome;
using System.Collections;

namespace TestProject_Calc
{
    public class UITestsCombined
    {
        private ChromeDriver _driver;
        private readonly string _homeURL = "http://localhost:5000/";
        private LoginPage _loginPage;

        private static IEnumerable TestValues()
        {
            yield return new TestCaseData("", "", "Please enter your user name and password");
            yield return new TestCaseData("", "newyork1", "Please enter your user name");
            yield return new TestCaseData("tes", "newyork1", "Your username or password is incorrect");
            yield return new TestCaseData("test", "", "Please enter your password");
            yield return new TestCaseData("test", "1", "Your username or password is incorrect");
            yield return new TestCaseData("TEST", "newyork1", "Your username or password is incorrect");
            yield return new TestCaseData("test", "NEWYORK1", "Your username or password is incorrect");
        }

        [OneTimeSetUp]
        public void SetUpDriver()
        {
            _driver = new ChromeDriver();
            _loginPage = new LoginPage(_driver);
        }

        [SetUp]
        public void LoginPage()
        {
            _driver.Navigate().GoToUrl(_homeURL);
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
            InsertValueClickLogin(login, password);
            Thread.Sleep(1000);
            Assert.That(_driver.Url, Is.EqualTo("http://localhost:5000/Calculator"), "The calculator page was not opened");
        }

        [Test]
        [TestCaseSource(nameof(TestValues))]
        public void Login_InvalidCredentials_ErrorMessageShown(string login, string password, string expectedError)
        {
            InsertValueClickLogin(login, password);
            Assert.That(_loginPage.ErrorMessage.Text, Is.EqualTo(expectedError));
        }

        private void InsertValueClickLogin(string login, string password)
        {
            _loginPage.InsertLoginValue(login);
            _loginPage.InsertPasswordValue(password);
            _loginPage.ClickLogin();
        }
    }
}
