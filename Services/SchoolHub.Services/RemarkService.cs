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

        public async Task<List<IndexRemarkViewModel>> GetRemarksByStudentIdAsync(string studentId)
            => await this.remarkRepository
                .All()
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(x => x.Date)
                .To<IndexRemarkViewModel>()
                .ToListAsync();

        public async Task AddRemarkAsync(RemarkFormModel formModel)
        {
            var remark = AutoMapperConfig.MapperInstance.Map<Remark>(formModel);

            await this.remarkRepository.AddAsync(remark);
            await this.remarkRepository.SaveChangesAsync();
        }
    }
}
