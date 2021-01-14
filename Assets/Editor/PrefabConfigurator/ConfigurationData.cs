using System.Collections.Generic;

namespace UI.Tools.Editor.Data
{
    public class ConfigurationData
    {
        public List<DataEntry> dataEntries;
    }

    [System.Serializable]
    public class DataEntry
    {
        public string text;

        public string color;

        public string imagePath;
    }
}
