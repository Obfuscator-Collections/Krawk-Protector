using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
namespace Krawk.Runtime
{
    public static class AntiDebug
    {
        public static void StartAntiDebug()
        {
            string x = "COR";
            if (Environment.GetEnvironmentVariable(x + "_PROFILER") != null ||
                Environment.GetEnvironmentVariable(x + "_ENABLE_PROFILING") != null)
                Environment.FailFast(null);
            
                var thread = new Thread(Worker);
                thread.IsBackground = true;
                thread.Start(null);
            
            
        }
        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern int OutputDebugString(string str);
        static void Worker(object thread)
        {
            var th = thread as Thread;
            if (th == null)
            {
                th = new Thread(Worker);
                th.IsBackground = true;
                th.Start(Thread.CurrentThread);
                Thread.Sleep(500);
            }
            for (; ; )
            {
                // Managed
                if (Debugger.IsAttached || Debugger.IsLogging())
                    Environment.FailFast("");

                // IsDebuggerPresent
                if (IsDebuggerPresent())
                    Environment.FailFast("");

         
                if (!th.IsAlive)
                    Environment.FailFast("");

                Thread.Sleep(1000);
            }
        }
    }
}
