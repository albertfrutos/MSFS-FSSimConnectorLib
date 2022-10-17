using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Microsoft.FlightSimulator.SimConnect;

namespace FSSimConnectorLib
{
    internal class EventsUpdater
    {
        List<Event> eventsList = new List<Event>();
        List<string> enumEventsList = new List<string>();

        List<string> URLs = new List<string>
        {
            "https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/Aircraft_Autopilot_Flight_Assist_Events.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/Aircraft_Electrical_Events.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/Aircraft_Engine_Events.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/Aircraft_Flight_Control_Events.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/Aircraft_Fuel_System_Events.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/Aircraft_Instrumentation_Events.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/Aircraft_Misc_Events.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/Aircraft_Radio_Navigation_Events.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/Miscellaneous_Events.htm",
            "https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/View_Camera_Events.htm"
        };
        string eventsFile = @"Files\events.json";
        string enumEventsFile = @"Files\enumEvents.txt";
        internal void UpdateEvents()
        {
            HtmlWeb web = new HtmlWeb();

            foreach (string url in URLs)
            {
                HtmlDocument doc = web.Load(url);
                var group = Path.GetFileNameWithoutExtension(url);
                ParseWeb(doc, group);
            }

            string json = JsonConvert.SerializeObject(eventsList);
            if (File.Exists(eventsFile))
            {
                File.Delete(eventsFile);
            }
            File.WriteAllText(eventsFile, json);
            File.WriteAllText(enumEventsFile, "internal enum EVENTS\n{\n" + string.Join(",\n", enumEventsList) +"\n}");

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
                        if (currentEvent.name.Contains(" or "))
                        {
                            Console.WriteLine("a");
                        }

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
