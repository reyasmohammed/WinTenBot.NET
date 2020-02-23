using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WinTenBot.Helpers.JsonSettings
{
    public class CamelCaseFollowProperty:CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (member.GetCustomAttribute<JsonPropertyAttribute>() is JsonPropertyAttribute jsonProperty)
            {
                property.PropertyName = jsonProperty.PropertyName;
            }

            return property;
        }
    }
}
