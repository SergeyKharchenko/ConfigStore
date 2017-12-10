using System;
using System.Linq;
using System.Reflection;

namespace ConfigStore.Api.Extensions {
    public static class EnumExtensions {
        public static T GetAttribute<T>(this Enum enumValue) where T : Attribute {
            Type enumType = enumValue.GetType();
            MemberInfo[] memberInfos = enumType.GetMember(enumValue.ToString());
            return memberInfos?.FirstOrDefault()?.GetCustomAttribute<T>();
        }
    }
}