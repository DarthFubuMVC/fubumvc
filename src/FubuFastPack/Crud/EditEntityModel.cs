using System;
using System.Web.Script.Serialization;
using FubuFastPack.Crud.Properties;
using FubuFastPack.Domain;
using FubuFastPack.Validation;
using FubuLocalization;
using FubuValidation;

namespace FubuFastPack.Crud
{
    [Serializable]
    public class EditEntityModel : IHaveSubmitAction
    {
        [NonSerialized]
        private Notification _notification;
        private SaveResult _result = SaveResult.Initial;

        public EditEntityModel(DomainEntity target)
        {
            SaveButtonText = FastPackKeys.SAVE_KEY;
            Target = target;
            _notification = new Notification();
        }

        private Guid _id = Guid.Empty;
        public Guid Id
        {
            get
            {
                if (_id != Guid.Empty)
                {
                    return _id;
                }

                return Target == null ? _id : Target.Id;
            }
            set
            {
                _id = value;
            }
        }


        public Notification Notification
        {
            get { return _notification; }
            set { _notification = value; }
        }

        public Type EntityType
        {
            get
            {
                return Target == null ? null : Target.GetTrueType();
            }
        }

        [ScriptIgnore]
        public string TypeName
        {
            get { return Target == null ? string.Empty : Target.GetTrueType().Name; }
        }

        [ScriptIgnore]
        public DomainEntity Target { get; private set; }

        [ScriptIgnore]
        public StringToken SaveButtonText { get; set; }

        public SaveResult Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public bool IsNewMode { get; set; }

        public string PropertyUpdateUrl { get; set; }
        public string SubmitAction { get; set; }

        public virtual bool IsNew()
        {
            return Target == null ? false : Target.IsNew();
        }

        public virtual string Flatten()
        {
            return "{}";
        }
    }
}