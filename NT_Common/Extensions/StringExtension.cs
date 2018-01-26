using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NT_Common.Extensions
{
    public static class StringExtension
    {
        public static T ToEnum<T>(this string selfStr) where T : struct, IConvertible
        {
            var type = typeof(T);
            if(!type.IsEnum) 
            {
                throw new InvalidOperationException();
            }
            foreach(var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute)) is EnumMemberAttribute attribute)
                {
                    if(attribute.Value?.Equals(selfStr, StringComparison.OrdinalIgnoreCase) ?? false)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }
            throw new ArgumentException("Not found.", "description");
        }
    }
}