# PostSharpDemo

基于 postsharp 6.1.x + netCore 6.0 示例 aop 实现

## PostSharp 使用

1、项目中引用 nuget 包 PostSharp 6.9.12

2、如果是试用版本的话，目前版本暂时不支持 VS2022

## PostSharp 中 Aspect(切面)类型

### `OnMethodBoundaryAspect`

postsharp 中一个可序列化的抽象类，System.Attribute 的子类，可以 override OnEntry、OnExit、OnSuccess、OnException 等方法实施拦截，可以读取入参，修改 ref、out 类型的入参，决定是否调用被拦截的方法体，以及读取、修改方法的返回值等，也可以进行日志记录等操作，拦截时，PostSharp 对原方法注入一个 try{}catch(){}语句，在适当的位置注入各个拦截事件的调用代码

### `OnFieldAccessAspect`

对 field 的读取、设置进行拦截处理，override OnGetValue、OnSetValue 方法实施拦截，测试过程中无法读取到 FieldInfo 属性，不知道是不是只有商业版注册后才可以使用这个功能，对 field 访问的拦截只能适用于当前程序集，如果其他程序集直接诶访问 field 无法实现拦截，所以对于 public、protected 类型的 field，PostSharp 会将 field 重命名，然后自动生成一个原 field 名字的 property，这会导致依赖的程序集二进制兼容性被破坏，需要重新编译。这个行为也可以通过选项进行配置，阻止 PostSharp 这样做

### `OnExceptionAspect`

用于实现异常捕获，可以运用于第三方开发的，没有源代码的程序上

### `OnMethodInvocationAspect`

override OnInvocation 方法实施拦截，PostSharp 不是直接修改注入目标程序集，而是为目标方法生成一个委托，修改当前程序集中的调用，改为调用委托，从而实现拦截（这种方式叫做 Call-site weaving，调用方织入，对应的另一种方式叫做 Target-site weaving，目标织入），这种方式可以实现对第三方程序集方法实施拦截

### `ImplementMethodAspect`

这个 aspect 用于 extern 方法、abstract 类的方法进行拦截，不要求目标方法有具体的实现

翻了一下源码，有这么些 aspect 的抽象定义。

```text
// PostSharp.Aspects
//
// 类型:
//
// Aspect
// AssemblyLevelAspect
// CompositionAspect
// CustomAttributeIntroductionAspect
// EventInterceptionAspect
// EventLevelAspect
// FieldLevelAspect
// IAssemblyLevelAspect
// IAsyncMethodInterceptionAspect
// ICloneAwareAspect
// ICompositionAspect
// ICustomAttributeIntroductionAspect
// IEventInterceptionAspect
// IEventLevelAspect
// IFieldLevelAspect
// IInstanceScopedAspect
// ILicensedAspect
// ILocationInterceptionAspect
// ILocationLevelAspect
// ILocationValidationAspect
// IManagedResourceIntroductionAspect
// IMethodInterceptionAspect
// IMethodLevelAspect
// InstanceLevelAspect
// IOnExceptionAspect
// IOnInstanceLocationInitializedAspect
// IOnMethodBoundaryAspect
// IOnStateMachineBoundaryAspect
// ITypeLevelAspect
// LocationInterceptionAspect
// LocationLevelAspect
// ManagedResourceIntroductionAspect
// MethodImplementationAspect
// MethodInterceptionAspect
// MethodLevelAspect
// OnExceptionAspect
// OnMethodBoundaryAspect
// TypeLevelAspect
```

### 实现记录 log 日志相关的 aop

```C#
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

using System;
using System.Diagnostics;

namespace PostSharpDemo.Aspects
{
    /// <summary>
    /// 日志aop拦截器
    /// warning 注意aspect必须是可序列化，用于assembly指令切入
    /// </summary>
    [PSerializable] // 注意aspect必须是可序列化，用于assembly指令切入
    [MulticastAttributeUsage(MulticastTargets.Method)]
    public class MyDemoLogAspectAttribute : PostSharp.Aspects.OnMethodBoundaryAspect
    {
        [NonSerialized]
        private Stopwatch _stopWatch;

        /// <summary>
        /// Method invoked before the target method is executed
        /// </summary>
        /// <param name="args"></param>
        public override void OnEntry(MethodExecutionArgs args)
        {
            //_stopWatch = Stopwatch.StartNew();
            Console.WriteLine("进入方法时，执行 [ {0} ] ...", args.Method);

            //base.OnEntry(args);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Console.WriteLine("退出方法时，执行 [ {0} ] ...", args.Method);
            //base.OnExit(args);
        }

        /// <summary>
        /// Method invoked after the target method has successfully completed.
        /// </summary>
        /// <param name="args"></param>
        public override void OnSuccess(MethodExecutionArgs args)
        {
            Console.WriteLine("调用方法执行成功时，执行 [ {0} ] ...", args.Method);

            //base.OnSuccess(args);
        }
    }
}

```

### 使用方式

这里使用方式有两种：

- 注解形式，即：在方法上增加` [MyDemoLogAspect]`注解。

```c#
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

```

​

- assembly 方式，即：在项目中添加一个 assembly 指令(就是项目->properties->AssemblyInfo.cs 中添加；
  - netCore 的话，在 program.cs 中添加也可以的)，告诉 PostSharp 对哪些目标实施拦截。比如`[assembly: MyDemoLogAspect(AttributeTargetTypes = "PostSharpDemo.Aspects.*", AttributePriority = 1)]` ，这样可以利用命名空间模糊匹配。

```C#
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


```

> 更多配置选项，可以参考源码 PostSharp.Extensibility.MulticastAttribute 中的字段定义。

编译后的代码，通过反编译后，其实可以看到已经把 aop 相关拦截逻辑编织进了你写的逻辑内了：

```c#
using System;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.ImplementationDetails._4713a2d7;
using PostSharpDemo.Model;

namespace PostSharpDemo.Services
{
	// Token: 0x02000009 RID: 9
	public class DemoUserService : IDemoUserService
	{
		// Token: 0x06000012 RID: 18 RVA: 0x000021C0 File Offset: 0x000003C0
		public DemoUser GetUser(int userId)
		{
			MethodExecutionArgs methodExecutionArgs = new MethodExecutionArgs(null, null);
			MethodExecutionArgs methodExecutionArgs2 = methodExecutionArgs;
			MethodBase <<EMPTY_NAME>> = <>z__a_1._2;
			methodExecutionArgs2.Method = <<EMPTY_NAME>>;
			<>z__a_1.a1.OnEntry(methodExecutionArgs);
			DemoUser result;
			try
			{
				DemoUser user = new DemoUser
				{
					Id = userId,
					UserName = userId.ToString(),
					UserId = userId,
					CreatedDate = DateTime.Now,
					UpdatedDate = DateTime.Now
				};
				DemoUser demoUser = user;
				result = demoUser;
				<>z__a_1.a1.OnSuccess(methodExecutionArgs);
			}
			finally
			{
				<>z__a_1.a1.OnExit(methodExecutionArgs);
			}
			return result;
		}
	}
}

```

到此，我们整理了两种使用方式：基于注解和基于 assembly 方式的。具体使用可以根据具体场景，灵活选择。

### 使用 PostSharp 的优点和缺点（即使用 AOP 的优点和缺点）

**总体来说，使用 PostSharp，将会带来如下优点：**

- 横切关注点单独分离出来，提高了代码的清晰性和可维护性。
- 只要在 Aspect 中编写辅助性功能代码，在一定程度上减少了工作量和冗余代码。

当然，使用 PostSharp 也不是没有缺点，**主要缺点有如下两方面：**

- 增加了调试的难度。
- 相比于不用 AOP 的代码，运行效率有所降低。

所以，对于是否引入 AOP，请根据项目具体情况，权衡而定。

## 资料

- 官方文档 https://www.postsharp.net/documentation
- 官方示例 https://samples.postsharp.net/
- https://www.postsharp.net/aop.net
- https://doc.postsharp.net/attribute-multicasting
- https://blog.postsharp.net/post/aspect-oriented-programming-vs-dependency-injection.html
- https://www.cnblogs.com/leoo2sk/archive/2010/11/30/aop-postsharp.html
- https://www.cnblogs.com/wwj1992/p/9232920.html
