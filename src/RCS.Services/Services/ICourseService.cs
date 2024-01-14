using RCS.Data.Entities;
using RCS.Data.Enums;

namespace RCS.Services.Services
{
    public interface ICourseService
    {
        Task<Course> AddCourseAsync(string title, string description, string videoUrl, string thumbnailImage, decimal price, DifficultyLevel difficultyLevel);
    }
}
