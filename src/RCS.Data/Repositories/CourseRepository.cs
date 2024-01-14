using NHibernate;
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
    }
}
