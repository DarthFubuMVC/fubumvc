using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Serenity
{
    public static class Kill
    {
        public static void Processes(params string[] names)
        {
            names.Each(process =>
            {
                try
                {
                    Process.GetProcessesByName(process).Each(x =>
                    {
                        Console.WriteLine("Trying to kill process " + x);
                        x.Kill();
                    });
                }
                catch (Exception e)
                {
                    // send it out to Console, but otherwise just kill it
                    Console.WriteLine(e);
                }
            });
        }
    }
}