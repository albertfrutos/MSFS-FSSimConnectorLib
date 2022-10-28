using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace FSSimConnectorLib
{
    public class Variable
    {
        public string name { get; set; }
        public string unit { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string group { get; set; }

        internal List<Variable> LoadVariables(string variablesFile)
        {
            var variables = File.ReadAllText(@variablesFile);
            var a = JsonConvert.DeserializeObject<List<Variable>>(variables);
            string b = "";
            foreach (Variable v in a)
            {
                b = b + v.name + "\r\n";
            }
            File.WriteAllText("vars.txt", b);
            return a;
        }
        public Variable GetVariableInformation(string variableName, List<Variable> variables)
        {
            return variables.FirstOrDefault(x => x.name == variableName);
        }

    }
}
