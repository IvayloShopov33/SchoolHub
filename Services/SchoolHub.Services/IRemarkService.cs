namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Remark;

    public interface IRemarkService
    {
        Task<List<IndexRemarkViewModel>> GetRemarksByStudentIdAsync(string studentId);

        Task AddRemarkAsync(RemarkFormModel formModel);
    }
}
