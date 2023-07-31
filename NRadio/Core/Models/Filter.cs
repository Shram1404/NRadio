using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRadio.Core.Models
{
    public class Filter
    {
        public bool HasName { get; set; }
        public bool HasUrl { get; set; }
        public bool HasTag { get; set; }
        public bool HasFavicon { get; set; }
        public bool HasCountry { get; set; }
        public bool HasLanguage { get; set; }
        public int MinBitrate { get; set; }
    }
}
