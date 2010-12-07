﻿using FubuCore.Reflection;
using FubuMVC.Core.UI.Configuration;

namespace FubuMVC.Core.UI.Security
{
    public enum FieldAccessCategory
    {
        LogicCondition,
        Authorization
    }

    public interface IFieldAccessRule
    {
        AccessRight RightsFor(ElementRequest request);
        bool Matches(Accessor accessor);
        FieldAccessCategory Category { get; }
    }
}