namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Remark;

    public interface IRemarkService
    {
        Task<RemarkFormModel> GetRemarkByIdAsync(string id);

        Task<(List<IndexRemarkViewModel> Remarks, int TotalCount)> GetRemarksByStudentIdAsync(string studentId, int page, int itemsPerPage);

        Task<List<GroupedRemarksViewModel>> GetGroupedRemarksByStudentAsync(string studentId);

        Task AddRemarkAsync(RemarkFormModel formModel);

        Task EditRemarkAsync(string id, RemarkFormModel formModel);

        Task DeleteRemarkAsync(string id);
    }
}
