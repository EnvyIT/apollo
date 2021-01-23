using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Dto
{
    public abstract class BaseDto
    {
        [Key]
        public long Id { get; set; }

        public DateTime RowVersion { get; set; }
    }
}
