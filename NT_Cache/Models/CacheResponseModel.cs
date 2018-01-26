using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace NT_Cache.Models
{
    public class CacheSuccessResponseModel
    {
        [JsonProperty(PropertyName = nameof(Value), DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("")]
        public string Value { get; set; }
    }

    public class CacheFailResponseModel
    {
        public object ErrorMsg { get; set; }
    }
}