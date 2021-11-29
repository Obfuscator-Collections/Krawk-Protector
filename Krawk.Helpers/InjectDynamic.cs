using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krawk.Helpers
{
    public static class InjectDynamic
    {
        const string Typer = "Dynamic";
        static readonly Dictionary<string, int> field2index = new Dictionary<string, int> {
            { "Int1",  1},
            { "Int2",  2}
        };
        public static void InjectInt(MethodDef method, int keyId, int key)
        {
            foreach (Instruction instr in method.Body.Instructions)
            {
                if (instr.OpCode == OpCodes.Ldsfld)
                {
                    var field = (IField)instr.Operand;
                    int _keyId;
                    if (field.DeclaringType.FullName == Typer &&
                        field2index.TryGetValue(field.Name, out _keyId) &&
                        _keyId == keyId)
                    {
                        instr.OpCode = OpCodes.Ldc_I4;
                        instr.Operand = key;
                    }
                }
            }
        }

        static readonly Dictionary<string, string> field3index = new Dictionary<string, string> {
            { "Str1",  "a"},
            { "Str2",  "b"}
        };
        public static void InjectString(MethodDef method, string keyId, string key)
        {
            foreach (Instruction instr in method.Body.Instructions)
            {
                if (instr.OpCode == OpCodes.Ldsfld)
                {
                    var field = (IField)instr.Operand;
                    string _keyId;
                    if (field.DeclaringType.FullName == Typer &&
                        field3index.TryGetValue(field.Name, out _keyId) &&
                        _keyId == keyId)
                    {
                        instr.OpCode = OpCodes.Ldstr;
                        instr.Operand = key;
                    }
                }
            }
        }
    }
    public static class MutationHelperNative
    {
        const string mutationType = "Dynamic";
        static readonly Dictionary<string, int> field2index = new Dictionary<string, int> { { "NativeKeyI0", 0 }};
        public static void InjectKey(MethodDef method, int keyId, MethodDef key)
        {
            foreach (Instruction instr in method.Body.Instructions)
            {
                if (instr.OpCode == OpCodes.Ldsfld)
                {
                    var field = (IField)instr.Operand;
                    int _keyId;
                    if (field.DeclaringType.FullName == mutationType &&
                        field2index.TryGetValue(field.Name, out _keyId) &&
                        _keyId == keyId)
                    {
                        instr.OpCode = OpCodes.Call;
                        instr.Operand = key;
                    }
                }
            }
        }
    }
}
