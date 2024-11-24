namespace SchoolHub.Services
{
    using System.Threading.Tasks;

    using SchoolHub.Data.Models;
    using SchoolHub.Web.ViewModels.Grade;

    public interface IGradeService
    {
        Task<bool> IsGradeAppliedByTeacherAsync(string teacherId, string gradeId);

        Task<DetailsGradeViewModel> GetGradeDetailsByIdAsync(string id);

        Task<Grade> GetGradeFurtherDetailsByIdAsync(string id);

        Task AddGradeAsync(GradeFormModel formModel);

        Task EditGradeAsync(string id, GradeFormModel formModel);

        Task DeleteGradeAsync(string id);
    }
}
