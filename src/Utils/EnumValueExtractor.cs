using System.Collections.Concurrent;
using System.Globalization;

namespace codecrafters_http_server.Utils;

public enum StringConversion
{
    Default,
    ToUpper,
    ToLower
}

public static class EnumValueExtractor
{
    public static Dictionary<T, string> GetEnumValues<T>(StringConversion stringConversion)
        where T : Enum
    {
        var enumValues = Enum.GetValues(typeof(T)).Cast<T>();
        var result = new Dictionary<T, string>();
        foreach (var enumValue in enumValues)
        {
            //Enum.GetValues returns some values multiple times for some reason (HttpStatusCode, for example)
            if (result.ContainsKey(enumValue))
                continue;
            var stringValue = enumValue.ToString();
            stringValue = stringConversion switch
            {
                StringConversion.ToUpper => stringValue.ToUpper(CultureInfo.InvariantCulture),
                StringConversion.ToLower => stringValue.ToLower(CultureInfo.InvariantCulture),
                _ => stringValue
            };
            result.Add(enumValue, stringValue);
        }

        return result;
    }
}