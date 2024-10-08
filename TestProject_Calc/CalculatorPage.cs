using OpenQA.Selenium.Chrome;

namespace TestProject_Calc
{
    public class CalculatorPage(ChromeDriver driver)
    {
        public static string URL = "http://localhost:5000/Calculator";
    }
}
