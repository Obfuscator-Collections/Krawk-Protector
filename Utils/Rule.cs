
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krawk_Protector.Protections;
using Krawk_Protector.Protections.ControlFlow;
using Krawk_Protector.Protections.ReferenceProxy;
using Krawk_Protector.Protections.Constants;
using Krawk_Protector.Protections.Math;
namespace Krawk_Protector.Utils
{
    class Rule
    {

        private bool enable_Constants = false;
        private bool enable_calli = false;
        private bool enable_cflow = false;
        private bool enable_debug = false;
        private bool enable_renamer = false;
        private bool enable_ildasm = false;
        private bool rename_assembly = false;
        private bool antide4dot = false;
        private bool refproxy = false;
        private List<IProtector> TaskList = new List<IProtector>();
        public Rule(bool enable_Constants, bool enable_calli, bool enable_cflow, bool enable_debug, bool enable_renamer, bool enable_assembly_name, bool antide4dot, bool refproxy)
        {
            this.enable_Constants = enable_Constants;
            this.enable_calli = enable_calli;
            this.enable_cflow = enable_cflow;
            this.enable_debug = enable_debug;
            this.enable_renamer = enable_renamer;
            this.enable_ildasm = enable_ildasm;
            this.rename_assembly = rename_assembly;
            this.antide4dot = antide4dot;
            this.refproxy = refproxy;
        }

        public void AddTasksToList()
        {
            if (enable_calli) { TaskList.Add(new CalliProtection()); }
            if (enable_Constants)
            {
                TaskList.Add(new ConstantsMelt());
                TaskList.Add(new NumbersMutation());
                TaskList.Add(new ConstantsProtection());
                TaskList.Add(new StringsMutation());
                TaskList.Add(new SizeofProtection());
                TaskList.Add(new MathProtection());

            }
            if (enable_cflow) { TaskList.Add(new ControlFlow()); }
            if (enable_debug) { TaskList.Add(new AntiDbg()); }
            if (refproxy) TaskList.Add(new ReferenceProxyProtection());
            if (enable_renamer) { TaskList.Add(new RenamerProtection()); }
            if (enable_ildasm) { TaskList.Add(new AntiILDasmProtection()); }
            if (rename_assembly) { TaskList.Add(new RenameAssembly()); }
            if (antide4dot) { TaskList.Add(new AntiDe4dotProtection()); }
            TaskList.Add(new WaterMark());
        }

        public List<IProtector> GetTasks()
        {
            return TaskList;
        }

    }

}
