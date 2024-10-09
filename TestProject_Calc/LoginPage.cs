using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TestProject_Calc
{
    public class LoginPage(ChromeDriver driver)

    {
        public static string URL = "http://localhost:5000/";
        private IWebElement LoginField => driver.FindElement(By.XPath("//input[@id='login']"));
        private IWebElement PasswordField => driver.FindElement(By.XPath("//input[@id='password']"));
        private IWebElement LoginButtonLocator => driver.FindElement(By.XPath("//button[text()='Login']"));

        private By ErrorLocator = By.XPath("//th[@id='errorMessage']");

        public void InsertLoginValue(string loginValue) => LoginField.SendKeys(loginValue);

        public void InsertPasswordValue(string passwordValue) => PasswordField.SendKeys(passwordValue);

        public void ClickLogin() => LoginButtonLocator.Click();

        public string ErrorMessageText => driver.FindElement(ErrorLocator).Text;

        public void EnterCredentialsAndLogin(string login, string password)
        {
            InsertLoginValue(login);
            InsertPasswordValue(password);
            ClickLogin();
        }
    }
}