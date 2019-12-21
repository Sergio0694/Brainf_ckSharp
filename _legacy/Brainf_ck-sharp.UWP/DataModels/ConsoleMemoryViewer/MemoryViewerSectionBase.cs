namespace Brainf_ck_sharp_UWP.DataModels.ConsoleMemoryViewer
{
    public abstract class MemoryViewerSectionBase
    {
        /// <summary>
        /// Gets the section type associated with the current instance
        /// </summary>
        public ConsoleMemoryViewerSection SectionType { get; }

        protected MemoryViewerSectionBase(ConsoleMemoryViewerSection type) => SectionType = type;
    }
}
