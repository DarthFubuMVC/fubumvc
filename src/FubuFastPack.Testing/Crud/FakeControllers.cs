using System;
using FubuFastPack.Crud;
using FubuFastPack.Testing.Security;

namespace FubuFastPack.Testing.Crud
{
    public class EditCaseViewModel : EditEntityModel
    {
        public EditCaseViewModel(Case target)
            : base(target)
        {
        }
    }

    public class CasesController : CrudController<Case, EditCaseViewModel>
    {
        public EditCaseViewModel Edit(Case model)
        {
            throw new NotImplementedException();
        }

        public CreationRequest<EditCaseViewModel> Create(EditCaseViewModel input)
        {
            throw new NotImplementedException();
        }

        public Case New()
        {
            throw new NotImplementedException();
        }
    }



    public class EditSiteViewModel : EditEntityModel
    {
        public EditSiteViewModel(Site target)
            : base(target)
        {
        }
    }

    public class SitesController : CrudController<Site, EditSiteViewModel>
    {
        public EditSiteViewModel Edit(Site model)
        {
            throw new NotImplementedException();
        }

        public CreationRequest<EditSiteViewModel> Create(EditSiteViewModel input)
        {
            throw new NotImplementedException();
        }

        public Site New()
        {
            throw new NotImplementedException();
        }
    }
}