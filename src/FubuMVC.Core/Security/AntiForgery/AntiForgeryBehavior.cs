﻿using System.Net;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class AntiForgeryBehavior : BasicBehavior
    {
        private readonly IOutputWriter _outputWriter;
        private readonly string _salt;
        private readonly IAntiForgeryValidator _validator;

        public AntiForgeryBehavior(string salt, IAntiForgeryValidator validator, IOutputWriter outputWriter)
            : base(PartialBehavior.Executes)
        {
            _salt = salt;
            _validator = validator;
            _outputWriter = outputWriter;
        }

        protected override Task<DoNext> performInvoke()
        {
            if (_validator.Validate(_salt))
            {
                return Task.FromResult(DoNext.Continue);
            }

            _outputWriter.WriteResponseCode(HttpStatusCode.InternalServerError);
            return Task.FromResult(DoNext.Stop);
        }
    }
}