namespace PersonalBot.Resources.Providers.Declarations
{
    public interface ISettingsProvider
    {
        string this[string index]
        {
            get;
        }
    }
}