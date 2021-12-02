
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using PostSharp.Extensibility;

using PostSharpDemo.Aspects;

/// <summary>
/// 利用assembly的方式
/// https://doc.postsharp.net/attribute-multicasting
/// </summary>

// 拦截PostSharpDemo.Services下所有类
[assembly: MyDemoLogAspect(AspectPriority = 1, AttributeTargetTypes = "PostSharpDemo.Services.*")]

// 排除AttributeTargetTypes指定目录下的某一些方法或者类，比如排除PostSharpDemo.Helper下所有的类
[assembly: MyDemoLogAspect(AttributePriority = 1, AttributeExclude = true, AttributeTargetTypes = "PostSharpDemo.Helper.*")]

// Add logging to System.Math to show we can add logging to anything.
[assembly: MyDemoLogAspect(AttributePriority = 2, AttributeTargetAssemblies = "mscorlib", AttributeTargetTypes = "System.Math")]

// 按类可见性过滤，例如，您可能只想将方面应用于定义为公共的类。
[assembly: MyDemoLogAspect(AttributePriority = 1, AttributeTargetTypes = "PostSharpDemo.Services.*", AttributeTargetTypeAttributes = MulticastAttributes.Public)]


// 按方法修饰符过滤，使用这种技术，您可以根据静态、抽象和虚拟关键字等事物的存在与否来应用方法级方面，或停止应用它。
[assembly: MyDemoLogAspect(AttributePriority = 1, AttributeTargetTypes = "PostSharpDemo.Services.*", AttributeTargetMemberAttributes = MulticastAttributes.Virtual)]

namespace PostSharpDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
