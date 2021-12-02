using PostSharpDemo.Model;

namespace PostSharpDemo.Services
{
    public interface IDemoUserService
    {
        DemoUser GetUser(int userId);
    }
}