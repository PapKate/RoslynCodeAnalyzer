using System;
using System.Collections.Generic;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Provides abstractions for a documentation analyzer
    /// </summary>
    public interface IDocumentationAnalyzer
    {
        /// <summary>
        /// Analyzers the files of the directory with the specified <paramref name="directoryPath"/> and
        /// extracts all the documentation information
        /// </summary>
        /// <param name="types"></param>
        /// <param name="directoryPath">The directory path</param>
        IEnumerable<ClassCommentInformation> Analyze(IEnumerable<Type> types, string directoryPath);
    }
}
