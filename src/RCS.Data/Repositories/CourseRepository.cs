﻿using NHibernate;
using RCS.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCS.Data.Repositories
{
    public class CourseRepository : Repository<Course, Guid>, ICourseRepository
    {
        public CourseRepository(ISession session) : base(session)
        {

        }

        public async Task<bool> IsDuplicateNameAsync(string name, Guid? id)
        {
            int? existingCourseCount = null;

            if (id.HasValue)
                existingCourseCount = await GetCountAsync(x => x.Title == name && x.Id != id.Value);
            else
                existingCourseCount = await GetCountAsync(x => x.Title == name);

            return existingCourseCount > 0;
        }
    }
}
