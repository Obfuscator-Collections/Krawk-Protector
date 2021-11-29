using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Krawk_Protector.Utils
{
    class Core
    {
        private Context krawk;
        private Rule rule;
        private List<IProtector> Tasks = new List<IProtector>();
        public Core(Context krawk, Rule rule)
        {
            this.krawk = krawk;
            this.rule = rule;
        }

        public void DoObfuscation()
        {
            rule.AddTasksToList();
            Tasks = rule.GetTasks();
            Console.WriteLine(Tasks.Count.ToString());
            if (!(Tasks.Count == 0))
                RunTasks();
            else { throw new Exception("You have to select at least one module!"); }
        }
        private void RunTasks()
        {
            foreach(IProtector prot in Tasks)
            {
                try
                {             
                    prot.InjectPhase(krawk);           
                    prot.ProtectionPhase(krawk);          
                }
                catch (Exception ex)
                {
                }
            }

            
        }

    }
}
