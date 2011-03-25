using System;
using System.Web.Script.Serialization;
using FubuLocalization;
using FubuMVC.Core;
using FubuValidation;

namespace FubuFastPack
{
    public class AjaxContinuation : JsonMessage
    {
        public ValidationError[] errors;
        public Guid Id { get; set; }

        /// <summary>
        ///   Opens a page in the stack, and then refreshes the original page when the stack is closed
        /// </summary>
        public string showDialog { get; set; }

        /// <summary>
        ///   Opens a page in the stack, and lets the opened page determine where to go when the stack is closed
        /// </summary>
        public string showPage { get; set; }

        /// <summary>
        ///   Refresh the current page
        /// </summary>
        public bool refresh { get; set; }

        /// <summary>
        ///   Navigates the whole window to the specified URL
        /// </summary>
        public string navigatePage { get; set; }

        public string message { get; set; }
        public string caption { get; set; }
        public bool success { get; set; }

        [ScriptIgnore]
        public object Target { get; set; }

        public static AjaxContinuation ForMessage(StringToken key)
        {
            return new AjaxContinuation
            {
                message = LocalizationManager.GetTextForKey(key),
                success = false
            };
        }

        public static AjaxContinuation ForMessage(StringToken key, object target)
        {
            var continuation = ForMessage(key);
            continuation.Target = target;

            return continuation;
        }

        public static AjaxContinuation ForDialog(string url, object target)
        {
            return new AjaxContinuation
            {
                showDialog = url,
                Target = target
            };
        }

        public static AjaxContinuation ForPage(string url, object target)
        {
            return new AjaxContinuation
            {
                showPage = url,
                Target = target
            };
        }

        public static AjaxContinuation ForNavigateWholePage(string url)
        {
            return new AjaxContinuation
            {
                navigatePage = url,
                success = true
            };
        }

        public static AjaxContinuation ForRefresh(object target)
        {
            return new AjaxContinuation
            {
                Target = target,
                success = true,
                refresh = true
            };
        }

        public static AjaxContinuation ForError(Notification notification)
        {
            return new AjaxContinuation
            {
                success = notification.IsValid(),
                errors = notification.ToValidationErrors()
            };
        }

        public AjaxContinuation WithSubmission(Notification notification, object target)
        {
            Target = target;
            success = notification.IsValid();
            errors = notification.ToValidationErrors();

            return this;
        }

        public static AjaxContinuation ForSuccess()
        {
            return new AjaxContinuation
            {
                success = true
            };
        }
    }
}