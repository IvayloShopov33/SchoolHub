namespace SchoolHub.Services
{
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Absence;

    public interface IAbsenceService
    {
        Task AddAbsenceAsync(AbsenceFormModel formModel);
    }
}
