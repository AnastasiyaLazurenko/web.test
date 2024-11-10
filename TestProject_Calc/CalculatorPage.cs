using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Reflection;

namespace TestProject_Calc
{
    public class CalculatorPage(ChromeDriver driver)
    {
        public static string URL = "http://localhost:5000/Calculator";
        private IWebElement DepositAmmountField => driver.FindElement(By.XPath("//input[@id='amount']"));
        private IWebElement RateOfInterestField => driver.FindElement(By.XPath("//input[@id='percent']"));
        private IWebElement InvestmentTermField => driver.FindElement(By.XPath("//input[@id='term']"));
        private IWebElement StartDateDay => driver.FindElement(By.XPath("//select[@id='day']"));
        private IWebElement StartDateMonth => driver.FindElement(By.XPath("//select[@id='month']"));
        private IWebElement StartDateYear => driver.FindElement(By.XPath("//select[@id='year']"));
        private IWebElement RadioButton360 => driver.FindElement(By.XPath("//input[@onchange='SetYear(365)']")); //!!
        private IWebElement RadioButton365 => driver.FindElement(By.XPath("//input[@onchange='SetYear(360)']")); //!!
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
            return (RadioButton360.Selected) ? FinancialYearInput.part :
                   (RadioButton365.Selected) ? FinancialYearInput.full : null;
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

        public CalculatorValues GenerateExpectedDTO(decimal depositValue = 1, decimal rateValue = 1, int term = 1, int day = 1, Month month = Month.January, int year = 2014, FinancialYearInput financialYear = FinancialYearInput.full)
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

        public CalculatorValues ChangeExpectedDTO(CalculatorValues inputData, string errorField)
        {
            foreach (var property in calculatorProperty.Where(w => w.Name == errorField))
            {
                if (property.GetValue(inputData) != "") property.SetValue(inputData, "0");
            }

            return inputData;
        }

        public CalculatorValues GenerateExpectedErrorDTO(string depositValue = "1", string rateValue = "1", string term = "1", string day = "1", Month month = Month.January, string year = "2014", FinancialYearInput? financialYear = FinancialYearInput.full)
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

            return inputData;
        }

        public CalculatorValues EnterInputs_GetOutput(CalculatorValues inputData, string errorField = "")
        {
            EnterInputs(inputData);

            if (String.IsNullOrEmpty(errorField)) ClickCalculate();
            else ChangeExpectedDTO(inputData, errorField);

            return GetActualDTO();
        }

        public void EnterInputs(CalculatorValues inputData)
        {
            InsertDepositAmmount(inputData.DepositValue);
            InsertRateOfInterest(inputData.RateValue);
            InsertInvestmentTerm(inputData.Term);
            InsertStartDateDay(inputData.Day);
            InsertStartDateMonth(inputData.Month);
            InsertStartDateYear(inputData.Year);
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

                if (!input.Equals(output))
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