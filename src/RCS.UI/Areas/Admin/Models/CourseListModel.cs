using RCS.Services.Services;

namespace RCS.UI.Areas.Admin.Models
{
    public class CourseListModel
    {
        private readonly ICourseService _courseService;
        public CourseListModel()
        {
            
        }

        public CourseListModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        internal async Task DeleteCourse(Guid id)
        {
            await _courseService.DeleteCourseAsync(id);
        }


    }
}
