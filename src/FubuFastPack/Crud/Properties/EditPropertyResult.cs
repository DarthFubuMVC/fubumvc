using System;
using FubuCore;
using FubuCore.Reflection;
using FubuFastPack.Lists;
using FubuLocalization;
using FubuValidation;

namespace FubuFastPack.Crud.Properties
{
    public class EditPropertyResult
    {
        public EditPropertyResult(Accessor accessor, Type entityType, string previousValue, string newValue)
            : this(accessor, entityType)
        {
            PreviousValue = previousValue;
            NewValue = newValue;
        }

        public EditPropertyResult(Accessor accessor, Type entityType)
        {
            Accessor = accessor;

            SetupListName(entityType);
        }

        private EditPropertyResult()
        {
        }

        public bool WasNotApplied { get; set; }

        public Accessor Accessor { get; private set; }
        public string PreviousValue { get; set; }
        public string NewValue { get; set; }
        public string ListName { get; private set; }
        public string DisplayValue { get; set; }

        public string FailureMessage { get; set; }

        private void SetupListName(Type entityType)
        {
            Accessor.ForAttribute<ListValueAttribute>(att =>
            {
                // I KNOW THIS IS AN 'EFFIN HACK, OK???????
                if (Accessor is SingleProperty)
                {
                    ListName = att.GetListName(entityType);
                }
                else
                {
                    ListName = att.GetListName(Accessor.OwnerType);
                }
            });
        }

        public static EditPropertyResult Failure(Accessor accessor, Type entityType, string failureMessage)
        {
            return new EditPropertyResult(accessor, entityType)
            {
                WasNotApplied = true,
                FailureMessage = failureMessage
            };
        }

        public static EditPropertyResult NotChangedResult()
        {
            return new EditPropertyResult
            {
                WasNotApplied = true
            };
        }

        public virtual ValidationError[] ToValidationErrors()
        {
            if (FailureMessage.IsEmpty()) return new ValidationError[0];

            return new[]{
                new ValidationError(Accessor.ToHeader(), FailureMessage),
            };
        }

        public bool IsListAccessor()
        {
            return ListName != null;
        }
    }
}