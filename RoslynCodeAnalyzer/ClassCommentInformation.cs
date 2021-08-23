using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassCommentInformation : BaseCommentInformation
    {
        #region Private Members

        /// <summary>
        /// The base classes
        /// </summary>
        private readonly List<Type> mClassCommentInformations = new List<Type>();

        /// <summary>
        /// The methods
        /// </summary>
        private readonly List<MethodCommentInformation> mMethodCommentInformations = new List<MethodCommentInformation>();

        /// <summary>
        /// The properties
        /// </summary>
        private readonly List<PropertyCommentInformation> mPropertyCommentInformations = new List<PropertyCommentInformation>();

        #endregion

        #region Public Properties

        /// <summary>
        /// The type of the class
        /// </summary>
        public Type ClassType { get; }

        /// <summary>
        /// The classes from the inheritance 
        /// </summary>
        public IEnumerable<Type> BaseClasses 
        {
            get { return mClassCommentInformations; }
        }

        /// <summary>
        /// The methods
        /// </summary>
        public IEnumerable<MethodCommentInformation> Methods 
        {
            get { return mMethodCommentInformations; }
        }

        /// <summary>
        /// The properties
        /// </summary>
        public IEnumerable<PropertyCommentInformation> Properties 
        {
            get { return mPropertyCommentInformations; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ClassCommentInformation(Type classType, string summary) : base(DeclarationSyntaxType.Class, classType.FullName, summary)
        {
            ClassType = classType ?? throw new ArgumentNullException(nameof(classType));
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Adds a <paramref name="value"/> to the <see cref="mClassCommentInformations"/>
        /// </summary>
        /// <param name="value">The value</param>
        internal void Add(Type value)
        {
            // Adds to the member the value
            mClassCommentInformations.Add(value);
        }

        /// <summary>
        /// Adds a <paramref name="value"/> to the <see cref="mMethodCommentInformations"/>
        /// </summary>
        /// <param name="value">The value</param>
        internal void Add(MethodCommentInformation value)
        {
            // Adds to the member the value
            mMethodCommentInformations.Add(value);
        }

        /// <summary>
        /// Adds a list of <paramref name="methods"/> to the <see cref="mMethodCommentInformations"/>
        /// </summary>
        /// <param name="methods">The properties</param>
        internal void AddRange(IEnumerable<MethodCommentInformation> methods)
        {
            foreach(var method in methods)
                if (!mMethodCommentInformations.Contains(method))
                    mMethodCommentInformations.Add(method);
        }

        /// <summary>
        /// Adds a <paramref name="value"/> to the <see cref="mPropertyCommentInformations"/>
        /// </summary>
        /// <param name="value">The value</param>
        internal void Add(PropertyCommentInformation value)
        {
            // Adds to the member the value
            mPropertyCommentInformations.Add(value);
        }

        /// <summary>
        /// Adds a list of <paramref name="properties"/> to the <see cref="mPropertyCommentInformations"/>
        /// </summary>
        /// <param name="properties">The properties</param>
        internal void AddRange(IEnumerable<PropertyCommentInformation> properties)
        {
            foreach (var property in properties)
                if (!mPropertyCommentInformations.Contains(property))
                    mPropertyCommentInformations.Add(property);
        }

        #endregion
    }
}
