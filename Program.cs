
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using PostSharp.Extensibility;

using PostSharpDemo.Aspects;

/// <summary>
/// ����assembly�ķ�ʽ
/// https://doc.postsharp.net/attribute-multicasting
/// </summary>

// ����PostSharpDemo.Services��������
[assembly: MyDemoLogAspect(AspectPriority = 1, AttributeTargetTypes = "PostSharpDemo.Services.*")]

// �ų�AttributeTargetTypesָ��Ŀ¼�µ�ĳһЩ���������࣬�����ų�PostSharpDemo.Helper�����е���
[assembly: MyDemoLogAspect(AttributePriority = 1, AttributeExclude = true, AttributeTargetTypes = "PostSharpDemo.Helper.*")]

// Add logging to System.Math to show we can add logging to anything.
[assembly: MyDemoLogAspect(AttributePriority = 2, AttributeTargetAssemblies = "mscorlib", AttributeTargetTypes = "System.Math")]

// ����ɼ��Թ��ˣ����磬������ֻ�뽫����Ӧ���ڶ���Ϊ�������ࡣ
[assembly: MyDemoLogAspect(AttributePriority = 1, AttributeTargetTypes = "PostSharpDemo.Services.*", AttributeTargetTypeAttributes = MulticastAttributes.Public)]


// ���������η����ˣ�ʹ�����ּ����������Ը��ݾ�̬�����������ؼ��ֵ�����Ĵ��������Ӧ�÷��������棬��ֹͣӦ������
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
