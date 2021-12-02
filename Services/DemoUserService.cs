using System;

using PostSharpDemo.Aspects;
using PostSharpDemo.Model;

namespace PostSharpDemo.Services
{
    //[MyDemoLogAspect] // 应用于类下所有方法
    public class DemoUserService : IDemoUserService
    {
        //[MyDemoLogAspect] // 应用于单个方法
        public DemoUser GetUser(int userId)
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
