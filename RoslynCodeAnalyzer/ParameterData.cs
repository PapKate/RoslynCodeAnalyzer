namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Represents a parameter's data
    /// </summary>
    public class ParameterData
    {
        #region Public Properties

        /// <summary>
        /// The parameter's name
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// The parameter's description
        /// </summary>
        public string ParamDescription { get; set; }

        /// <summary>
        /// The parameter's color
        /// </summary>
        public string ParamColor { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ParameterData()
        {

        }

	    #endregion
    }

}
