namespace SchoolHub.Services
{
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Grade;

    public interface IGradeService
    {
        Task AddGradeAsync(GradeFormModel formModel);
    }
}
