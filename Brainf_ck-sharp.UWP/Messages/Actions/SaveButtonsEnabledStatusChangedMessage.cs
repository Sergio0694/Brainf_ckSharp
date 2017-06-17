namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    public class SaveButtonsEnabledStatusChangedMessage
    {
        public bool SaveEnabled { get; }

        public bool SaveAsEnabled { get; }

        public SaveButtonsEnabledStatusChangedMessage(bool save, bool saveAs)
        {
            SaveEnabled = save;
            SaveAsEnabled = saveAs;
        }
    }
}
