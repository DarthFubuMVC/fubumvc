using System;
using System.Collections.Generic;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IOutputNode
    {
        /// <summary>
        /// Add an IFormatter strategy for writing with an optional condition
        /// </summary>
        /// <param name="formatter"></param>
        /// <param name="condition"></param>
        void Add(IFormatter formatter, IConditional condition = null);

        /// <summary>
        /// Add a media writer and optional condition by an open type
        /// of IMediaWriter<T> where T is the resource type
        /// </summary>
        /// <param name="mediaWriterType"></param>
        /// <param name="condition"></param>
        void Add(Type mediaWriterType, IConditional condition = null);

        /// <summary>
        /// Explicitly register an IMediaWriter<T> where T is the resource type
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="condition"></param>
        void Add(object writer, IConditional condition = null);

        /// <summary>
        /// All the explicitly configured Media.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMedia> Media();

        IEnumerable<IMedia<T>> Media<T>();
            
            
            
        /// <summary>
        /// All the possible mimetypes for the explicitly added writers
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> MimeTypes();

        /// <summary>
        /// Is there at least one writer for this mimetype
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        bool Writes(MimeType mimeType);

        /// <summary>
        /// Is there at least one writer for this mimetype?
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        bool Writes(string mimeType);

        /// <summary>
        /// Remove all existing writers
        /// </summary>
        void ClearAll();

        Type ResourceType { get; }
        bool HasView(IConditional conditional);
    }
}