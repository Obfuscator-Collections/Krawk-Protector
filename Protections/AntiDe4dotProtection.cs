using dnlib.DotNet;
using Krawk_Protector.Utils;

namespace Krawk_Protector.Protections
{
    class AntiDe4dotProtection : IProtector
    {
        string IProtector.Name => "AntiDe4dotProtection";
        

        void IProtector.InjectPhase(Context krawk) { }

        void IProtector.ProtectionPhase(Context krawk)
        {
            var ManifestModule = krawk.ManifestModule;
            TypeDef typeDef1 = new TypeDefUser("", "Form", ManifestModule.CorLibTypes.GetTypeRef("System", "Attribute"));
            InterfaceImpl item1 = new InterfaceImplUser(typeDef1);
            ManifestModule.Types.Add(typeDef1);
            typeDef1.Interfaces.Add(item1);

            TypeDef typeDef2 = new TypeDefUser("", "Program", ManifestModule.CorLibTypes.GetTypeRef("System", "Attribute"));
            InterfaceImpl item2 = new InterfaceImplUser(typeDef2);
            ManifestModule.Types.Add(typeDef2);
            typeDef2.Interfaces.Add(item2);
        }

    }
}
