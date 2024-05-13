namespace DotnetMassTransitApp.Sample1.Consumer.Extensions;

public static class QueueExtensions
{
    public static string RoutingKeySuffixByComma(this string? routingKey)
    {
        if (routingKey == null) 
            return string.Empty;

        string[] parts = routingKey.Split('.');
        string dynamicPart = parts[parts.Length - 1];

        return dynamicPart;
    }
}
