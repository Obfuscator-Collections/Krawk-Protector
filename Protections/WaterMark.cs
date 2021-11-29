using Krawk_Protector.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
namespace Krawk_Protector.Protections
{
    class WaterMark : IProtector
    {

        string IProtector.Name { get { return "WaterMark"; } }
     
        public void InjectPhase(Context krawk)
        { }
        public void ProtectionPhase(Context krawk)
        {
            TypeRef attrRef = krawk.ManifestModule.CorLibTypes.GetTypeRef("System", "Attribute");
            var attrType = new TypeDefUser("", "Protected", attrRef);

            krawk.ManifestModule.Types.Add(attrType);
            var ctor = new MethodDefUser(
                    ".ctor",
                    MethodSig.CreateInstance(krawk.ManifestModule.CorLibTypes.Void, krawk.ManifestModule.CorLibTypes.String),
                    MethodImplAttributes.Managed,
                    MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            ctor.Body = new CilBody();
            ctor.Body.MaxStack = 1;
            ctor.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            ctor.Body.Instructions.Add(OpCodes.Call.ToInstruction(new MemberRefUser(krawk.ManifestModule, ".ctor", MethodSig.CreateInstance(krawk.ManifestModule.CorLibTypes.Void), attrRef)));
            ctor.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
            
            attrType.Methods.Add(ctor);
            
            var attr = new CustomAttribute(ctor);
            attr.ConstructorArguments.Add(new CAArgument(krawk.ManifestModule.CorLibTypes.String, "Krawk Protector v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version));

            krawk.ManifestModule.CustomAttributes.Add(attr);
           
        }
    }
}
