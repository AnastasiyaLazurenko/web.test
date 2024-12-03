using OpenQA.Selenium.Chrome;
using TestProject_Calc.Pages;

namespace TestProject_Calc.Tests
{
    public class BaseTest
    {
        public static ChromeDriver driver;
        public LoginPage loginPage;
        public CalculatorPage calculatorPage;

        public BaseTest()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            driver = new ChromeDriver(options);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);

            loginPage = new(driver);
            calculatorPage = new(driver);
        }

        public static void OpenPage(string URL)
        {
            driver.Navigate().GoToUrl(URL);
        }
    }
}
