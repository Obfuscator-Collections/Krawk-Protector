using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Krawk.Runtime
{
    public static class CalliRuntime
    {
        public static IntPtr ResolveToken(long token)
        {
            System.Reflection.Module module = typeof(CalliRuntime).Module;
            return module.ResolveMethod((int)token).MethodHandle.GetFunctionPointer();
        }
    }
}
