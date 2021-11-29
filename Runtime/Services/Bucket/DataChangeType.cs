namespace SpicaSDK.Services
{
    public enum DataChangeType
    {
        Error = -1,
        Initial = 0,
        EndOfInitial = 1,
        Insert = 2,
        Delete = 3,
        Expunge = 4,
        Update = 5,
        Replace = 6,
        Order = 7,
        Response = 8
    }
}