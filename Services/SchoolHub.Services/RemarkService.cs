namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Remark;

    public class RemarkService : IRemarkService
    {
        private readonly IDeletableEntityRepository<Remark> remarkRepository;

        public RemarkService(IDeletableEntityRepository<Remark> remarkRepository)
        {
            this.remarkRepository = remarkRepository;
        }

        public async Task<RemarkFormModel> GetRemarkByIdAsync(string id)
            => await this.remarkRepository
                .All()
                .Where(x => x.Id == id)
                .To<RemarkFormModel>()
                .FirstOrDefaultAsync();

        public async Task<(List<IndexRemarkViewModel> Remarks, int TotalCount)> GetRemarksByStudentIdAsync(string studentId, int page, int itemsPerPage)
        {
            var remarks = await this.remarkRepository
                .AllAsNoTracking()
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(x => x.Date)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<IndexRemarkViewModel>()
                .ToListAsync();

            var totalCount = await this.remarkRepository
                .AllAsNoTracking()
                .CountAsync(a => a.StudentId == studentId);

            return (remarks, totalCount);
        }

        public async Task<List<GroupedRemarksViewModel>> GetGroupedRemarksByStudentAsync(string studentId)
            => await this.remarkRepository
                .AllAsNoTracking()
                .Where(a => a.StudentId == studentId)
                .GroupBy(a => new { a.Subject.Name, TeacherName = a.Teacher.FullName })
                .Select(g => new GroupedRemarksViewModel
                {
                    SubjectName = g.Key.Name,
                    TeacherName = g.Key.TeacherName,
                    PositiveRemarksCount = g.Count(r => r.IsPraise),
                    NegativeRemarksCount = g.Count(r => !r.IsPraise),
                })
                .OrderByDescending(x => x.PositiveRemarksCount)
                .ThenBy(x => x.SubjectName)
                .ToListAsync();

        public async Task AddRemarkAsync(RemarkFormModel formModel)
        {
            var remark = AutoMapperConfig.MapperInstance.Map<Remark>(formModel);

            await this.remarkRepository.AddAsync(remark);
            await this.remarkRepository.SaveChangesAsync();
        }

        public async Task EditRemarkAsync(string id, RemarkFormModel formModel)
        {
            var remarkById = await this.remarkRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            remarkById.Comment = formModel.Comment;
            remarkById.Date = formModel.Date;
            remarkById.IsPraise = formModel.IsPraise;
            remarkById.SubjectId = formModel.SubjectId;
            remarkById.TeacherId = formModel.TeacherId;
            remarkById.StudentId = formModel.StudentId;

            await this.remarkRepository.SaveChangesAsync();
        }

        public async Task DeleteRemarkAsync(string id)
        {
            var remarkById = await this.remarkRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            remarkById.IsDeleted = true;
            await this.remarkRepository.SaveChangesAsync();
        }
    }
}
