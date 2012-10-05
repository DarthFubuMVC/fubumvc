using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Assets.Combination
{
    public class AssetGrouper<T>
    {
        private readonly Stack<IList<T>> _groups = new Stack<IList<T>>();
        private bool _inGroup;

        public IEnumerable<IList<T>> GroupSubjects(IEnumerable<T> subjects, Func<T, bool> filter)
        {
            subjects.Each(x => visit(x, filter));

            return _groups.Reverse();
        }

        private void visit(T subject, Func<T, bool> filter)
        {
            if (filter(subject))
            {
                hit(subject);
            }
            else
            {
                miss();
            }
        }

        private void hit(T subject)
        {
            if (!_inGroup)
            {
                _groups.Push(new List<T>());
                _inGroup = true;
            }

            _groups.Peek().Add(subject);
        }

        private void miss()
        {
            _inGroup = false;
        }

    }
}