namespace Brainf_ck_sharp_UWP.DataModels.IDEResults
{
    /// <summary>
    /// The base class for models that indicate a specific result type when running code in the IDE
    /// </summary>
    public abstract class IDEResultSectionDataBase
    {
        /// <summary>
        /// Gets the type of info for the current model
        /// </summary>
        public IDEResultSection Section { get; }

        // Protected constructor that initializes the section type
        protected IDEResultSectionDataBase(IDEResultSection section)
        {
            Section = section;
        }
    }
}
