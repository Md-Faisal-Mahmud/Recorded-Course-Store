using RCS.Data.Entities;
using RCS.Data.Enums;
using RCS.Data.UnitOfWorks;
using System.Xml.Linq;

namespace RCS.Services.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;


        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Course> AddCourseAsync(string title, string description, string videoUrl, string thumbnailImage, decimal price, DifficultyLevel difficultyLevel)
        {
            Course course = new Course()
            {
                Title = title,
                Description = description,
                VideoURL = videoUrl,
                ThumbnailImage = thumbnailImage,
                Price = price,
                DifficultyLevel = difficultyLevel
            };

            await _unitOfWork.BeginTransaction();
            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.Commit();

            return course;
        }
    }
}
