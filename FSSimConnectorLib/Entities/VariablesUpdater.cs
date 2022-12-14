using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using FSSimConnectorLib;

namespace FSSimConnectorLib
{
    internal class VariablesUpdater : Entities
    {
        List<Variable> variablesList = new List<Variable>();

        public void UpdateVariables(VariablesConfig varConf)
        {
            string variablesFile = varConf.variablesFile;

            HtmlWeb web = new HtmlWeb();

            foreach (string url in varConf.variablesURLs)
            {
                HtmlDocument doc = web.Load(url);
                var group = Path.GetFileNameWithoutExtension(url);
                ParseWeb(doc, group);
            }

            string json = JsonConvert.SerializeObject(variablesList);

            if (File.Exists(@variablesFile))
            {
                File.Delete(@variablesFile);
            }
            File.WriteAllText(@variablesFile, json);

            var uniqueVariablesList = variablesList.Select(x => x.unit).Distinct().ToList();
            Console.WriteLine("Variables updated.");
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
                        variable.unit = GetCorrectedUnit(cells[2].InnerText.Replace("\n", string.Empty).Trim());
                        variable.type = GetType(variable.unit);
                        variable.name = cells[0].InnerText.Replace("\n", string.Empty);
                        //Console.WriteLine(variable.name);
                        variable.description = cells[1].InnerText.Replace("\n", string.Empty);
                        variable.group = variableGroup;
                        variablesList.Add(variable);
                    }
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
