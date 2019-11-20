namespace BracketPairColorizer.Settings.Settings
{
    public interface IPersistSettings
    {
        void Write(byte[] data);

        byte[] Read();
    }
}
