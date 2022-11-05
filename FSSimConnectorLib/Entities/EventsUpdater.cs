using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Microsoft.FlightSimulator.SimConnect;
using FSSimConnectorLib;

namespace FSSimConnectorLib
{
    internal class EventsUpdater
    {
        List<Event> eventsList = new List<Event>();
        List<string> enumEventsList = new List<string>();

        internal void UpdateEvents(EventsConfig eventsConfig)
        {
            HtmlWeb web = new HtmlWeb();

            foreach (string url in eventsConfig.eventsURLs)
            {
                HtmlDocument doc = web.Load(url);
                var group = Path.GetFileNameWithoutExtension(url);
                ParseWeb(doc, group);
            }

            string json = JsonConvert.SerializeObject(eventsList);
            if (File.Exists(eventsConfig.eventsFile))
            {
                File.Delete(eventsConfig.eventsFile);
            }
            File.WriteAllText(eventsConfig.eventsFile, json);
            File.WriteAllText(eventsConfig.enumEventsFile, "internal enum EVENTS\n{\n" + string.Join(",\n", enumEventsList) +"\n}");

            Console.WriteLine("Events updated. Updating app code and recompiling required.");
        }

        private void ParseWeb(HtmlDocument doc, string variableGroup)
        {
            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//*/table"))
            {

                foreach (HtmlNode row in table.SelectNodes("tbody/tr").Skip(1))
                {
                    Event currentEvent = new Event();
                    var cells = row.SelectNodes("td").ToList();
                    if (cells.Count >= 3)
                    {
                        currentEvent.name = cells[0].InnerText;

                        foreach( string multievent in currentEvent.name.Split(new string[] { "or", "\n" }, StringSplitOptions.RemoveEmptyEntries)){
                            if (multievent.Trim() != string.Empty)
                            {
                                currentEvent.name = cleanString(multievent);
                                currentEvent.description = cleanString(cells[2].InnerText);
                                currentEvent.unit = String.Empty;
                                currentEvent.parameters = cleanString(cells[1].InnerText);
                                currentEvent.group = variableGroup;
                                eventsList.Add(currentEvent);
                                enumEventsList.Add(currentEvent.name);
                            }
                        }
                    }
                }
            }
        }


        private string cleanString(string text)
        {
            return text.Replace("\n", string.Empty).Trim(' ');
        }
    }
}
