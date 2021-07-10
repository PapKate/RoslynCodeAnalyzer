namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// The constants for the app
    /// </summary>
    public static class Constants
    {
        #region Paths

        /// <summary>
        /// The path to the cs class file
        /// </summary>
        public const string CsFilePath = @"C:\Users\PapKate\Documents\PersonalProjects\C#\RoslynCodeAnalyzer\RoslynCodeAnalyzer\MethodCommentInformation.cs";

        /// <summary>
        /// The path to the xml file
        /// </summary>
        public const string XmlFilePath = @"C:\Users\PapKate\Documents\PersonalProjects\C#\RoslynCodeAnalyzer\RoslynCodeAnalyzer\RoslynCodeAnalyzer.xml";

        #endregion

        #region Xml Members

        /// <summary>
        /// The start summary tag
        /// </summary>
        public const string SummaryXmlStartMember = "<summary>";

        /// <summary>
        /// The end summary tag
        /// </summary>
        public const string SummaryXmlEndMember = "</summary>";

        #endregion

        #region Tags

        /// <summary>
        /// The "class"
        /// </summary>
        public const string ClassTag = "class";

        /// <summary>
        /// The "method"
        /// </summary>
        public const string MethodTag = "method";

        /// <summary>
        /// The "property"
        /// </summary>
        public const string PropertyTag= "property";

        /// <summary>
        /// The "param"
        /// </summary>
        public const string ParameterTag = "param";

        /// <summary>
        /// The "paramref"
        /// </summary>
        public const string ParameterReferenceTag = "paramref";

        /// <summary>
        /// The "summary"
        /// </summary>
        public const string SummaryTag = "summary";

        /// <summary>
        /// The "member"
        /// </summary>
        public const string MemberTag = "member";

        /// <summary>
        /// The "name"
        /// </summary>
        public const string NameTag = "name";

        /// <summary>
        /// The "see"
        /// </summary>
        public const string SeeTag = "see";

        /// <summary>
        /// The "cref"
        /// </summary>
        public const string CrefTag = "cref";

        #endregion

        #region Lines Feed And Symbols

        /// <summary>
        /// The "\r"
        /// </summary>
        public const string CarriageReturn = "\r";

        /// <summary>
        /// The "\n"
        /// </summary>
        public const string NewLine = "\n";

        /// <summary>
        /// The "///"
        /// </summary>
        public const string TripleSlashes = "///";

        /// <summary>
        /// The "."
        /// </summary>
        public const string Dot = ".";

        /// <summary>
        /// The "("
        /// </summary>
        public const string LeftParenthesis = "(";

        #endregion


    }
}
