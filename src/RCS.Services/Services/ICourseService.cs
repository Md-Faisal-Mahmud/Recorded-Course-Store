using RCS.Data.Entities;
using RCS.Data.Enums;

namespace RCS.Services.Services
{
    public interface ICourseService
    {
        Task<Course> AddCourseAsync(string title, string description, string thumbnailImage, decimal price, DifficultyLevel difficultyLevel);
        //Course GetCourse(Guid id);
    }
}
