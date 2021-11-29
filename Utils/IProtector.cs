using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krawk_Protector.Utils
{


    interface IProtector
    {
        string Name { get; }
        void InjectPhase(Context krawk);
        void ProtectionPhase(Context krawk);
        

    }
}

