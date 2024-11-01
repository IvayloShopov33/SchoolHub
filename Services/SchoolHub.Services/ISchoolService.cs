namespace SchoolHub.Services
{
    using System.Linq;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.School;

    public interface ISchoolService
    {
        IQueryable All();

        Task AddSchoolAsync(SchoolFormModel formModel);
    }
}
