﻿using System;
using System.Collections.Generic;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Provides information about the document comments of a parameter of a method
    /// </summary>
    public class ParameterCommentInformation
    {
        #region Public Properties

        /// <summary>
        /// The parameter's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The parameter's description
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// The parameters that are used in the <see cref="Comments"/>
        /// </summary>
        public IEnumerable<ParameterCommentInformation> CommentParameters { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ParameterCommentInformation()
        {

        }

	    #endregion
    }
}
