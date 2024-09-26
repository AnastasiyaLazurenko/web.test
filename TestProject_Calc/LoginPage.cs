using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TestProject_Calc
{
    public class LoginPage(ChromeDriver driver)
    {
        private IWebElement LoginField => driver.FindElement(By.XPath("//input[@id='login']"));
        private IWebElement PasswordField => driver.FindElement(By.XPath("//input[@id='password']"));
        private IWebElement LoginButtonLocator => driver.FindElement(By.XPath("//button[text()='Login']"));
        public IWebElement ErrorMessage => driver.FindElement(By.XPath("//th[@id='errorMessage']"));

        public void InsertLoginValue(string loginValue) => LoginField.SendKeys(loginValue);

        public void InsertPasswordValue(string passwordValue) => PasswordField.SendKeys(passwordValue);

        public void ClickLogin() => LoginButtonLocator.Click();
    }
}