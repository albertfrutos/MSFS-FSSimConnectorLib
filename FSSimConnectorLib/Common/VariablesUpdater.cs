using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace FSSimConnectorLib
{
    internal class VariablesUpdater : Entities
    {
        List<Variable> variablesList = new List<Variable>();
        List<string> URLs = new List<string>
        {
            "https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_SimVars/Aircraft_AutopilotAssistant_Variables.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_SimVars/Aircraft_Brake_Landing_Gear_Variables.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_SimVars/Aircraft_Control_Variables.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_SimVars/Aircraft_Electrics_Variables.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_SimVars/Aircraft_Engine_Variables.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_SimVars/Aircraft_FlightModel_Variables.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_SimVars/Aircraft_Fuel_Variables.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_SimVars/Aircraft_Misc_Variables.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_SimVars/Aircraft_RadioNavigation_Variables.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_SimVars/Aircraft_System_Variables.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Services_Variables.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Miscellaneous_Variables.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Camera_Variables.htm"
        };
        string variablesFile = @"Files\variables.json";
        public void UpdateVariables()
        {
            HtmlWeb web = new HtmlWeb();

            foreach (string url in URLs)
            {
                HtmlDocument doc = web.Load(url);
                var group = Path.GetFileNameWithoutExtension(url);
                ParseWeb(doc, group);
            }
            //File.WriteAllText(@"Files\units.txt", String.Join("\n", variablesList.Select(x => x.unit).Distinct()));
            string json = JsonConvert.SerializeObject(variablesList);
            //return;
            if (File.Exists(variablesFile))
            {
                File.Delete(variablesFile);
            }
            File.WriteAllText(variablesFile, json);

            var uniqueVariablesList = variablesList.Select(x => x.unit).Distinct().ToList();
            Console.WriteLine(uniqueVariablesList);

        }

        private void ParseWeb(HtmlDocument doc, string variableGroup)
        {
            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//*/table"))
            {

                foreach (HtmlNode row in table.SelectNodes("tbody/tr").Skip(1))
                {
                    Variable variable = new Variable();
                    var cells = row.SelectNodes("td").ToList();
                    if (cells.Count == 4)
                    {
                        variable.unit = cells[2].InnerText.Replace("\n", string.Empty); // GetCorrectedUnit(cells[2].InnerText.Replace("\n", string.Empty).Trim());
                        variable.type = GetType(variable.unit);
                        variable.name = cells[0].InnerText.Replace("\n", string.Empty);
                        Console.WriteLine(variable.name);
                        variable.description = cells[1].InnerText.Replace("\n", string.Empty);
                        variable.group = variableGroup;
                        variablesList.Add(variable);
                    }
                    //Console.WriteLine(cells[3].InnerText.Replace("\n", string.Empty));
                    //Console.WriteLine("");

                }
            }
        }

        private string GetCorrectedUnit(string initialUnit)
        {
            var correctedUnit = initialUnit;

            if (unitCorrections.ContainsKey(initialUnit)){
                return unitCorrections[initialUnit];
            }
            else
            {
                return correctedUnit;
            }           
        }

        private string GetType(string unit)
        {
            if (unit.ToLower().Contains("bool"))
            {
                return "bool";
            }
            else if (unit.ToLower().Contains("string"))
            {
                return "string";
            }
            else
            {
                List<string> listNumberUnits = new List<string>()
                {
                    "psi","PSI","Feet","Percent","Knots","Number","Degrees","Radians","Integer","inHg","Position","Pounds","RPM","Frequency",
                    "Pound","Centimeters","Amperes","Volts","Amps","Rankine","Ratio","Scalar","Foot","Hours","Seconds","Psf","Celsius",
                    "Gallons","Mask","kias","radian","Mach","Gforce","second","pascal","Hz","Meters","miles","MHz","Meter","Millibars","millimeters"
                };

                foreach (string unitIsForNumber in listNumberUnits)
                {
                    if (unit.ToLower().Contains(unitIsForNumber.ToLower()))
                    {
                        return "int32";
                    }
                }

                return null;
                
            }
        }
    }
}
