using System;

namespace Serenity
{
    public interface INavigationLogger
    {
        void Navigating(string url, Action action);
    }
}