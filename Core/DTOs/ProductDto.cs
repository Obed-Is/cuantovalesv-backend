using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string NameSite { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string UrlImg { get; set; } = string.Empty;
        public string UrlSite { get; set; } = string.Empty;
    }
}
