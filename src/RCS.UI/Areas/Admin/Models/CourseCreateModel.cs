using Autofac;
using RCS.Data.Enums;
using RCS.Services.Services;
using System.ComponentModel.DataAnnotations;

namespace RCS.UI.Areas.Admin.Models
{
    public class CourseCreateModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        // Using Required and Display attributes to customize error message
        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "VideoURL is required")]
        [Url(ErrorMessage = "Please enter a valid URL for VideoURL")]
        public string VideoURL { get; set; }

        // Using Display attribute to customize the display name
        [Display(Name = "Thumbnail Image")]
        public string? ThumbnailImage { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 999999.99, ErrorMessage = "Price should be between 0 and 999999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "DifficultyLevel is required")]
        public DifficultyLevel DifficultyLevel { get; set; }

        private ICourseService _courseService;

        public CourseCreateModel()
        {

        }

        public CourseCreateModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        internal void ResolveDependency(ILifetimeScope scope)
        {
            _courseService = scope.Resolve<ICourseService>();
        }

        internal async Task CreateCourseAsync()
        {
            await _courseService.AddCourseAsync(Title, Description, VideoURL, ThumbnailImage, Price, DifficultyLevel);
        }
    }
}
