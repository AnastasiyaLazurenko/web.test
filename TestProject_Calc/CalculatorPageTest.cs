using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TestProject_Calc
{
    public class CalculatorPageTest
    {
        private ChromeDriver driver;
        private CalculatorPage calculatorPage;

        [OneTimeSetUp]
        public void SetUpDriver()
        {
            // var options = new ChromeOptions();
            // options.AddArgument("--headless");
            // driver = new ChromeDriver(options);
            driver = new ChromeDriver(); //!!
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            calculatorPage = new CalculatorPage(driver);
        }

        [SetUp]
        public void OpenLoginPage()
        {
            driver.Navigate().GoToUrl(CalculatorPage.URL);
        }

        [OneTimeTearDown]
        public void TearDownTest()
        {
            driver.Close();
        }

        [Test]
        [TestCase(100000, 0.01, 1, "1", Month.January, 2010, FinancialYearInput.full)]
        [TestCase(100000, 0.01, 365, "28", Month.February, 2011, FinancialYearInput.full)]
        [TestCase(100000, 99.99, 1, "20", Month.March, 2012, FinancialYearInput.part)]

        [TestCase(100000, 99.99, 360, "21", Month.April, 2013, FinancialYearInput.part)]
        [TestCase(100000, 100, 1, "11", Month.May, 2014, FinancialYearInput.part)]
        [TestCase(100000, 100, 360, "5", Month.June, 2015, FinancialYearInput.part)]

        [TestCase(99999.99, 0.01, 1, "3", Month.July, 2016, FinancialYearInput.full)]
        [TestCase(99999.99, 0.01, 365, "31", Month.August, 2017, FinancialYearInput.full)]
        [TestCase(99999.99, 99.99, 1, "30", Month.September, 2018, FinancialYearInput.part)]

        [TestCase(99999.99, 99.99, 360, "1", Month.October, 2019, FinancialYearInput.part)]
        [TestCase(99999.99, 100, 1, "29", Month.November, 2020, FinancialYearInput.full)]
        [TestCase(99999.99, 100, 365, "4", Month.December, 2021, FinancialYearInput.full)]

        [TestCase(0.01, 0.01, 1, "1", Month.May, 2029, FinancialYearInput.part)]
        [TestCase(0.01, 0.01, 360, "1", Month.May, 2028, FinancialYearInput.part)]
        [TestCase(0.01, 99.99, 1, "1", Month.May, 2027, FinancialYearInput.part)]

        [TestCase(0.01, 99.99, 360, "1", Month.May, 2026, FinancialYearInput.part)]
        [TestCase(0.01, 100, 1, "1", Month.May, 2025, FinancialYearInput.full)]
        [TestCase(0.01, 100, 365, "29", Month.February, 2024, FinancialYearInput.full)]

        public void InsertValidInputs_GetResults(decimal depositValue, decimal rateValue, int term, int day, Month month, int year, FinancialYearInput financialYear)
        {
            //Arrange
            var expectedValues = calculatorPage.GenerateExpectedDTO(depositValue, rateValue, term, day, month, year, financialYear);

            //Act
            var actualValues = calculatorPage.EnterInputs_GetOutput(expectedValues);

            //Assert
            var log = calculatorPage.CompareValues(expectedValues, actualValues);
            Assert.That(log, Is.Empty);
        }

        [Test]
        [TestCase("100000.01")]
        [TestCase("100000.001")]
        [TestCase("0")]
        [TestCase("-")]
        [TestCase("null")]
        [TestCase("0,1")]
        public void DepositAmount_InvalidValues_GetResults(string depositValue)
        {
            //Arrange
            var expectedValues = calculatorPage.GenerateExpectedErrorDTO(depositValue: depositValue);

            //Act
            var actualValues = calculatorPage.EnterInputs_GetOutput(expectedValues, errorField: "DepositValue");

            //Assert
            var log = calculatorPage.CompareValues(expectedValues, actualValues);
            Assert.Multiple(() =>
            {
                Assert.That(log, Is.Empty);
                Assert.That(!calculatorPage.CalculateButton.Enabled);
            }
            );
        }

        [Test]
        [TestCase("100.01")]
        [TestCase("0")]
        [TestCase("-")]
        [TestCase("null")]
        public void InterestRate_InvalidValues_GetResults(string rateValue)
        {
            //Arrange
            var expectedValues = calculatorPage.GenerateExpectedErrorDTO(rateValue: rateValue);

            //Act
            var actualValues = calculatorPage.EnterInputs_GetOutput(expectedValues, errorField: "RateValue");

            //Assert
            var log = calculatorPage.CompareValues(expectedValues, actualValues);
            Assert.Multiple(() =>
            {
                Assert.That(log, Is.Empty);
                Assert.That(!calculatorPage.CalculateButton.Enabled);
            }
            );
        }

        [Test]
        [TestCase("366")]
        [TestCase("0")]
        [TestCase("-")]
        [TestCase("null")]
        [TestCase("")]
        public void InvestmentTerm_InvalidValues_GetResults(string investmentTerm)
        {
            //Arrange
            var expectedValues = calculatorPage.GenerateExpectedErrorDTO(term: investmentTerm);

            //Act
            var actualValues = calculatorPage.EnterInputs_GetOutput(expectedValues, errorField: "Term");

            //Assert
            var log = calculatorPage.CompareValues(expectedValues, actualValues);
            Assert.Multiple(() =>
            {
                Assert.That(log, Is.Empty);
                Assert.That(!calculatorPage.CalculateButton.Enabled);
            }
            );
        }

        [Test]
        [TestCase("0")]
        [TestCase("32")]
        public void StartDate_NonexistentDay_GetResults(string day)
        {
            //Arrange
            var expectedValues = calculatorPage.GenerateExpectedErrorDTO(day: day);

            //Assert
            Assert.Throws<NoSuchElementException>(() => calculatorPage.EnterInputs(expectedValues));
        }

        [Test]
        [TestCase("2009")]
        [TestCase("2030")]
        public void StartDate_NonexistentYear_GetResults(string year)
        {
            //Arrange
            var expectedValues = calculatorPage.GenerateExpectedErrorDTO(year: year);
            calculatorPage.EnterInputs(expectedValues);

            //Assert
            Assert.Throws<NoSuchElementException>(() => calculatorPage.EnterInputs(expectedValues));
        }

        [Test]
        [TestCase("29", Month.February, "2023")]
        public void InsertNonexistentDate(string day, Month month, string year)
        {
            //Arrange
            var expectedValues = calculatorPage.GenerateExpectedErrorDTO(day: day, month: month, year: year);
            calculatorPage.EnterInputs(expectedValues);

            //Assert
            Assert.That(!calculatorPage.CalculateButton.Enabled);
        }
    }
}
