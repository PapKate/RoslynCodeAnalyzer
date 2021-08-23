namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Provides enumeration over the syntax types
    /// </summary>
    public enum DeclarationSyntaxType
    {
        /// <summary>
        /// The information is related to a class
        /// </summary>
        Class = 0,

        /// <summary>
        /// The information is related to a property
        /// </summary>
        Property = 1,

        /// <summary>
        /// The information is related to a method
        /// </summary>
        Method = 2,

        /// <summary>
        /// The information is related to a parameter
        /// </summary>
        Parameter = 3,

        /// <summary>
        /// The information is related to an interface
        /// </summary>
        Interface = 4,

        /// <summary>
        /// The information is related to a struct / value type
        /// </summary>
        Struct = 5
    }
}
