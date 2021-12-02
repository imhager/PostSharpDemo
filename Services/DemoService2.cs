using PostSharpDemo.Model;

using System;

namespace PostSharpDemo.Services
{
    public class DemoService2 : IDemoService2
    {
        public DemoUser GetUser2(int userId)
        {
            var user = new DemoUser
            {
                Id = userId,
                UserName = userId.ToString(),
                UserId = userId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            return user;
        }
    }
}
