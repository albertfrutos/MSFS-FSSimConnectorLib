using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    public class Helpers
    {

        public async Task WaitWhile(Func<bool> condition, int frequency = 20, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (condition())
                {
                    await Task.Delay(frequency);
                }
            });

            if (waitTask != await Task.WhenAny(waitTask,
                    Task.Delay(timeout)))
                throw new TimeoutException();
        }

        public List<string> SearchFileInAllDirectories(string parentDirectory, string filename)
        {
            return Directory.GetFiles(parentDirectory, filename, SearchOption.AllDirectories).ToList();
        }

    }
}
