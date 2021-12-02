using System;

namespace PostSharpDemo.Model
{
    public class DemoUser
    {
        public DemoUser() { }

        public int Id { get; set; }

        public int UserId { get; set; }

        public string? UserName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime UpdatedDate { get; set; }

    }
}
