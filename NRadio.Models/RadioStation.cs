using System.ComponentModel.DataAnnotations;

namespace NRadio.Models
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class RadioStation
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = default;
        [Required]
        public string Url { get; set; } = default;

        public int? Bitrate { get; set; }

        public string Country { get; set; }

        public string CountryCode { get; set; }

        public string Language { get; set; }

        public string Tags { get; set; }

        public string Favicon { get; set; }

        public string HomePage { get; set; }

        public bool IsFavorite { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is RadioStation other)
            {
                return Name == other.Name && Url == other.Url;
            }

            return false;
        }
    }
}