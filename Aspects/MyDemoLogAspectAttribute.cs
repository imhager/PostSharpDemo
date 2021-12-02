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
    /// https://blog.csdn.net/XuWei_XuWei/article/details/34103647
    /// </summary>
    [PSerializable] // 注意aspect必须是可序列化，用于assembly指令切入
    [MulticastAttributeUsage(MulticastTargets.Method)]
    public class MyDemoLogAspectAttribute : OnMethodBoundaryAspect
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
