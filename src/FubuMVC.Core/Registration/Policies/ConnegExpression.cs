using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Registration.Policies
{
    public class ConnegExpression
    {
        private readonly Policy _policy;

        public ConnegExpression(Policy policy)
        {
            _policy = policy;
        }


        /// <summary>
        /// The behavior chain will respond to either form posts (application/x-www-form-urlencoded) or json, and render
        /// as json
        /// </summary>
        public void MakeAsymmetricJson()
        {
            _policy.ModifyBy(chain => chain.MakeAsymmetricJson());
            //_policy.ModifyWith<AsymmetricJsonModification>();
        }

        /// <summary>
        /// The behavior chain will only respond to application/json or text/json, and renders json
        /// </summary>
        public void MakeSymmetricJson()
        {
            _policy.ModifyBy(chain => chain.MakeSymmetricJson());
            //_policy.ModifyWith<SymmetricJsonModification>();
        }

        /// <summary>
        /// The resource (output) model will be rendered as text/html by calling
        /// ToString() on the resource model
        /// </summary>
        public void AddHtml()
        {
            _policy.ModifyBy(chain => {
                if (chain.HasResourceType())
                {
                    chain.Output.Add(typeof (HtmlStringWriter<>));
                }
            });
        }

        /// <summary>
        /// The chain will respond to form posts with a mimetype of 'application/x-www-form-urlencoded'
        /// </summary>
        public void AllowHttpFormPosts()
        {
            _policy.ModifyBy(chain => {
                if (chain.InputType() != null)
                {
                    chain.Input.Add(typeof (ModelBindingReader<>));
                }
            });
        }

        /// <summary>
        /// The chain will accept Json by deserializing to the input type
        /// </summary>
        public void AcceptJson()
        {
            // TODO -- this has to be redone
            _policy.ModifyBy(chain => {
                if (chain.InputType() != null)
                {
                    chain.Input.Add(new JsonSerializer());
                }
            });
        }

        /// <summary>
        /// Add an additional writer to the chain
        /// </summary>
        /// <param name="writerType"></param>
        public void AddWriter(Type writerType)
        {
            _policy.ModifyBy(chain => {
                if (chain.HasResourceType())
                {
                    chain.Output.Add(writerType);
                }
            });
        }

        /// <summary>
        /// Removes any writers already registered against this chain
        /// </summary>
        public void ClearAllWriters()
        {
            _policy.ModifyBy(chain => chain.Output.ClearAll(), configurationType: ConfigurationType.Conneg);
        }

        /// <summary>
        /// Adds both json and xml formatters to the chain as well as accepting form posts with a mimetype of 'application/x-www-form-urlencoded'
        /// </summary>
        public void ApplyConneg()
        {
            _policy.ModifyBy(chain => chain.ApplyConneg(), configurationType: ConfigurationType.Conneg);
        }

    }
}