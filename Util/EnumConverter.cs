using PubQuizBackend.Exceptions;

namespace PubQuizBackend.Util
{
    public static class EnumConverter
    {
        public static TEnum FromInt<TEnum>(int value) where TEnum : struct, Enum
        {
            if (Enum.IsDefined(typeof(TEnum), value))
                return (TEnum)(object)value;

            throw new MyBadException();
        }

        public static TEnum FromString<TEnum>(string value, bool ignoreCase = true) where TEnum : struct, Enum
        {
            if (Enum.TryParse<TEnum>(value, ignoreCase, out var result) && Enum.IsDefined(result))
                return result;

            throw new MyBadException();
        }
    }
}
