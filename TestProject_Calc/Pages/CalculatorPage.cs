using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Reflection;
using TestProject_Calc.Helpers;

namespace TestProject_Calc.Pages
{
    public class CalculatorPage(ChromeDriver driver)
    {
        public static string URL = "http://localhost:5000/Calculator";

        private IWebElement DepositAmmountField => driver.FindElement(By.Id("amount"));
        private IWebElement RateOfInterestField => driver.FindElement(By.Id("percent"));
        private IWebElement InvestmentTermField => driver.FindElement(By.CssSelector("#term"));
        private IWebElement StartDateDay => driver.FindElement(By.CssSelector("#day"));
        private IWebElement StartDateMonth => driver.FindElement(By.XPath("//select[@id='month']"));
        public IWebElement StartDateYear => driver.FindElement(By.XPath("//select[@id='year']"));
        private IWebElement RadioButton360 => driver.FindElement(By.XPath("//input[@id='finYear360']"));
        private IWebElement RadioButton365 => driver.FindElement(By.XPath("//input[@id='finYear365']"));
        public IWebElement CalculateButton => driver.FindElement(By.XPath("//button[@id='calculateBtn']"));
        private IWebElement InteresetEarnedField => driver.FindElement(By.XPath("//input[@id='interest']"));
        private IWebElement IncomeField => driver.FindElement(By.XPath("//input[@id='income']"));
        private IWebElement EndDateField => driver.FindElement(By.XPath("//input[@id='endDate']"));
        private PropertyInfo[] calculatorProperty = typeof(CalculatorValues).GetProperties();

        public void InsertDepositAmmount(string depositValue) => DepositAmmountField.SendKeys(depositValue);
        public void InsertRateOfInterest(string rateValue) => RateOfInterestField.SendKeys(rateValue);
        public void InsertInvestmentTerm(string term) => InvestmentTermField.SendKeys(term);
        public void InsertStartDateDay(string day) => new SelectElement(StartDateDay).SelectByValue(day);
        public void InsertStartDateMonth(string month) => StartDateMonth.SendKeys(month);
        public void InsertStartDateYear(string year) => StartDateYear.SendKeys(year);

        public FinancialYearInput? CheckFinancialYear()
        {
            return RadioButton360.Selected ? FinancialYearInput.part :
                   RadioButton365.Selected ? FinancialYearInput.full : null;
        }

        public void ChoseFinancialYear(FinancialYearInput? financialYear)
        {
            switch (financialYear)
            {
                case FinancialYearInput.full:
                    RadioButton365.Click();
                    break;
                case FinancialYearInput.part:
                    RadioButton360.Click();
                    break;
            }
        }

        public void ClickCalculate() => CalculateButton.Click();

        public CalculatorValues GenerateInputOLD(decimal depositValue = 1, decimal rateValue = 1, int term = 1, int day = 1, Month month = Month.January, int year = 2014, FinancialYearInput financialYear = FinancialYearInput.full)
        {
            decimal expectedInteresetEarned = Math.Round(depositValue * rateValue / 100 * term / (int)financialYear, 2);
            decimal expectedIncome = depositValue + expectedInteresetEarned;
            DateTime startDate = new(year, (int)month, day);
            var expectedEndDate = startDate.AddDays(term).ToString("dd/MM/yyyy");

            var inputData = new CalculatorValues
            {
                DepositValue = depositValue.ToString(),
                RateValue = rateValue.ToString(),
                Term = term.ToString(),
                Day = day.ToString(),
                Month = month.ToString(),
                Year = year.ToString(),
                FinancialYear = financialYear,
                InteresetEarned = expectedInteresetEarned.ToString("##,0.00"),
                Income = expectedIncome.ToString("##,0.00"),
                EndDate = expectedEndDate.ToString()
            };

            return inputData;
        }

        public CalculatorValues ChangeExpectedDTO(CalculatorValues inputData, string errorField, string expectedReplacement)
        {
            foreach (var property in calculatorProperty.Where(w => w.Name == errorField))
            {
                property.SetValue(inputData, expectedReplacement);
            }

            return inputData;
        }

        public CalculatorValues GenerateInput(string depositValue = "1", string rateValue = "1", string term = "1", string day = "1", Month month = Month.January, string year = "2014", FinancialYearInput? financialYear = FinancialYearInput.full, bool calculateExpectedValues = true)
        {
            var inputData = new CalculatorValues
            {
                DepositValue = depositValue,
                RateValue = rateValue,
                Term = term,
                Day = day,
                Month = month.ToString(),
                Year = year,
                FinancialYear = financialYear,
                InteresetEarned = "0.00",
                Income = "0.00",
                EndDate = DateTime.Now.ToString("dd/MM/yyyy")
            };

            if (calculateExpectedValues)
            {

                if (Decimal.TryParse(depositValue, out decimal depositValueDecimal) &&
                    Decimal.TryParse(rateValue, out decimal rateValueDecimal) &&
                    Int32.TryParse(term, out int termInt) &&
                    Int32.TryParse(year, out int yearInt) &&
                    Int32.TryParse(day, out int dayInt)
                    )
                {
                    var interesetEarnedDecimal = Math.Round(depositValueDecimal * rateValueDecimal / 100 * termInt / (int)financialYear, 2);
                    inputData.InteresetEarned = interesetEarnedDecimal.ToString("##,0.00");
                    inputData.Income = (depositValueDecimal + interesetEarnedDecimal).ToString("##,0.00");
                    DateTime startDate = new(yearInt, (int)month, dayInt);
                    inputData.EndDate = startDate.AddDays(termInt).ToString("dd/MM/yyyy");
                }
                else throw new Exception("Values of invalid format were provided for the calculation");
            }

            return inputData;
        }

        public CalculatorValues EnterInputs_GetOutput(CalculatorValues inputData, string errorField = "", string expectedReplacement = "0")
        {
            EnterInputs(inputData);

            if (string.IsNullOrEmpty(errorField)) ClickCalculate();
            else ChangeExpectedDTO(inputData, errorField, expectedReplacement);

            return GetActualDTO();
        }

        public void EnterInputs(CalculatorValues inputData)
        {
            InsertDepositAmmount(inputData.DepositValue);
            InsertRateOfInterest(inputData.RateValue);
            InsertInvestmentTerm(inputData.Term);
            InsertStartDateYear(inputData.Year);
            InsertStartDateMonth(inputData.Month);
            InsertStartDateDay(inputData.Day);
            ChoseFinancialYear(inputData.FinancialYear);
        }

        public CalculatorValues GetActualDTO()
        {
            static string value(IWebElement item) => item.GetAttribute("value");

            var outputData = new CalculatorValues
            {
                DepositValue = value(DepositAmmountField),
                RateValue = value(RateOfInterestField),
                Term = value(InvestmentTermField),
                Day = value(StartDateDay),
                Month = value(StartDateMonth),
                Year = value(StartDateYear),
                FinancialYear = CheckFinancialYear(),
                InteresetEarned = value(InteresetEarnedField),
                Income = value(IncomeField),
                EndDate = value(EndDateField)
            };

            return outputData;
        }

        public string? CompareValues(CalculatorValues inputData, CalculatorValues outputData)
        {
            var consoleLog = "";

            foreach (PropertyInfo prop in calculatorProperty)
            {
                var input = prop.GetValue(inputData);
                var output = prop.GetValue(outputData);

                if (input is not null && !input.Equals(output))
                {
                    consoleLog += $"In the {prop.Name} field: expected {input}, actual {output} \n";
                }
            }

            return consoleLog;
        }
    }

    public enum FinancialYearInput
    {
        full = 365,
        part = 360
    };

    public enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    };
}