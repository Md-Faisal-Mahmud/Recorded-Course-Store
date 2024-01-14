using RCS.Data.Enums;

namespace RCS.Data.Entities
{
    public class Course : IEntity<Guid>
    {
        public virtual Guid Id { get; set; }
        public virtual string Title { get; set; }

        public virtual string? Description { get; set; }

        public virtual string VideoURL { get; set; }

        public virtual string? ThumbnailImage { get; set; }
        public virtual decimal Price { get; set; }

        public virtual DifficultyLevel DifficultyLevel { get; set; }

    }
}
