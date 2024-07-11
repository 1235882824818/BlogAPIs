using Microsoft.AspNetCore.Identity;

namespace BlogAPIs.Entities
{
    // Models/User.cs

    public class User : IdentityUser<Guid>
    {

    }

    // Models/Post.cs
    public class Blog
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public Guid AuthorId { get; set; }
        public User Author { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }

}
