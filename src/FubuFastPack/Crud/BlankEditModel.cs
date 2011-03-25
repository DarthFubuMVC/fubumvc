using System;
using FubuFastPack.Domain;

namespace FubuFastPack.Crud
{
    [Serializable]
    public class BlankEditModel : EditEntityModel
    {
        public BlankEditModel(DomainEntity target)
            : base(target)
        {
        }
    }
}