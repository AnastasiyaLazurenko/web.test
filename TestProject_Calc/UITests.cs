using OpenQA.Selenium.Chrome;

namespace TestProject_Calc
{
    public class UITests
    {
        private ChromeDriver _driver;
        private readonly string _homeURL = "http://localhost:5000/";
        private LoginPage _loginPage;

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
        [TestCase("", "")]
        public void Login_EmptyCredentials_ErrorMessageShown(string login, string password)
        {
            InsertValueClickLogin(login, password);
            Assert.That(_loginPage.ErrorMessage.Text, Is.EqualTo("Please enter your user name and password")); //User not found!
        }

        [Test]
        [TestCase("", "newyork1")]
        public void Login_EmptyLogin_ErrorMessageShown(string login, string password)
        {
            InsertValueClickLogin(login, password);
            Assert.That(_loginPage.ErrorMessage.Text, Is.EqualTo("Please enter your user name")); //Incorrect user name!
        }

        [Test]
        [TestCase("tes", "newyork1")]
        public void Login_WrongLogin_ErrorMessageShown(string login, string password)
        {
            InsertValueClickLogin(login, password);
            Assert.That(_loginPage.ErrorMessage.Text, Is.EqualTo("Your username or password is incorrect")); //Incorrect user name!
        }

        [Test]
        [TestCase("test", "")]
        public void Login_EmptyPass_ErrorMessageShown(string login, string password)
        {
            InsertValueClickLogin(login, password);
            Assert.That(_loginPage.ErrorMessage.Text, Is.EqualTo("Please enter your password"));//Incorrect password!
        }

        [Test]
        [TestCase("test", "1")]
        public void Login_WrongPass_ErrorMessageShown(string login, string password)
        {
            InsertValueClickLogin(login, password);
            Assert.That(_loginPage.ErrorMessage.Text, Is.EqualTo("Your username or password is incorrect"));//Incorrect password!
        }

        [Test]
        [TestCase("TEST", "newyork1")]
        public void Login_UpperCaseLogin_ErrorMessageShown(string login, string password)
        {
            InsertValueClickLogin(login, password);
            Assert.That(_loginPage.ErrorMessage.Text, Is.EqualTo("Your username or password is incorrect"));//Incorrect password!?
        }

        [Test]
        [TestCase("test", "NEWYORK1")]
        public void Login_UpperCasePassword_ErrorMessageShown(string login, string password)
        {
            InsertValueClickLogin(login, password);
            Assert.That(_loginPage.ErrorMessage.Text, Is.EqualTo("Your username or password is incorrect"));//Incorrect password!
        }

        private void InsertValueClickLogin(string login, string password)
        {
            _loginPage.InsertLoginValue(login);
            _loginPage.InsertPasswordValue(password);
            _loginPage.ClickLogin();
        }
    }
}
