namespace Play.Common.Settings;

public class MongoDBSettings
{
    public string Host { get; init; } = string.Empty;
    public string Port { get; init; } = string.Empty;

    public string ConnectionString => $"mongodb://{Host}:{Port}";
}
