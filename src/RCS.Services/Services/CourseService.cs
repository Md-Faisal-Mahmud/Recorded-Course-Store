﻿using RCS.Data.Entities;
using RCS.Data.Enums;
using RCS.Data.UnitOfWorks;
using System.Data;
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

        public async Task<Course> AddCourseAsync(string title, string description, string thumbnailImage, decimal price, DifficultyLevel difficultyLevel)
        {
            Course course = new Course()
            {
                Title = title,
                Description = description,
                ThumbnailImageName = thumbnailImage,
                Price = price,
                DifficultyLevel = difficultyLevel
            };

            await _unitOfWork.BeginTransaction();
            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.Commit();

            return course;
        }

        public async Task<Course> GetCourseAsync(Guid id)
        {
            var course = await _unitOfWork.Courses.GetSingleAsync(x => x.Id == id);
            return course;
        }

        public async Task UpdateCourseAsync(Guid id,string title, string description, string thumbnailImage, decimal price, DifficultyLevel difficultyLevel) 
        {
            if (await _unitOfWork.Courses.IsDuplicateNameAsync(title, id))
            {
                throw new DuplicateNameException("Course name is duplicate");
            }

            Course course = await _unitOfWork.Courses.GetSingleAsync(id);
            course.Title = title;
            course.Description = description;
            course.ThumbnailImageName = thumbnailImage;
            course.Price = price;
            course.DifficultyLevel = difficultyLevel;

            await _unitOfWork.BeginTransaction();
            await _unitOfWork.Courses.UpdateAsync(course);
            await _unitOfWork.Commit();
        }
    }
}
