using TestProject_Calc.Pages;

namespace TestProject_Calc.Helpers
{
    public class CalculatorValues
    {
        public string DepositValue { get; set; }
        public string RateValue { get; set; }
        public string Term { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public FinancialYearInput? FinancialYear { get; set; }
        public string InteresetEarned { get; set; }
        public string Income { get; set; }
        public string EndDate { get; set; }
    }
}
