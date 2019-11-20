namespace BracketPairColorizer.Settings.Contracts
{
    public interface IPackageUserOptions
    {
        byte[] Read();

        void Write(byte[] options);
    }
}
