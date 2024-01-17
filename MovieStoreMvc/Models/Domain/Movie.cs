using System.ComponentModel.DataAnnotations;

namespace MovieStoreMvc.Models.Domain
{
    public class Movie
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public int RealeaseYear { get; set; }
        [Required]
        public string? MovieImage { get; set; } //stores movie image name with extension (eg, image0001.jpg)
        [Required]
        public string? Cast { get; set; }
        [Required]
        public string? Director { get; set; }

    }
}
