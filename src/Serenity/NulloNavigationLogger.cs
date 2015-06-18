using System;
using System.Diagnostics;

namespace Serenity
{
    public class NulloNavigationLogger : INavigationLogger
    {
        public void Navigating(string url, Action action)
        {
            Debug.WriteLine("Navigating the browser to " + url);
            action();
        }
    }
}