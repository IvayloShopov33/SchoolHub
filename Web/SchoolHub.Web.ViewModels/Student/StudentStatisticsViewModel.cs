namespace SchoolHub.Web.ViewModels.Student
{
    public class StudentStatisticsViewModel
    {
        public string StudentName { get; set; }

        public double AverageGPA { get; set; }

        public double TotalAbsences { get; set; }

        public int PraisingRemarksCount { get; set; }

        public int NegativeRemarksCount { get; set; }
    }
}
