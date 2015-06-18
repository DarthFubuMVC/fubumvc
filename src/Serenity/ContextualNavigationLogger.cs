using System;
using StoryTeller;

namespace Serenity
{
    public class ContextualNavigationLogger : INavigationLogger
    {
        private readonly ISpecContext _context;

        public ContextualNavigationLogger(ISpecContext context)
        {
            _context = context;
        }

        public void Navigating(string url, Action action)
        {
            using (_context.Timings.Subject("Navigation", url))
            {
                action();
            }
        }
    }
}