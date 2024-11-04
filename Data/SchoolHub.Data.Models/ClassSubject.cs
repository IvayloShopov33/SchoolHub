namespace SchoolHub.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.EntityFrameworkCore;

    [PrimaryKey(nameof(ClassId), nameof(SubjectId))]
    public class ClassSubject
    {
        [ForeignKey(nameof(Class))]
        public string ClassId { get; set; } = null!;

        public virtual Class Class { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        public virtual Subject Subject { get; set; }
    }
}
