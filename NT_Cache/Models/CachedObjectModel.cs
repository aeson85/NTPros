using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Caching.Distributed;

namespace NT_Cache.Models
{
    public class CachedObjectModel
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        public DistributedCacheEntryOptions Options { get; set; }
    }
}