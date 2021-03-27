using System.ComponentModel.DataAnnotations;

namespace GreenFlux.Application.DtoModels
{
    public class SaveGroupDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CapacityInAmps should have a value greater than zero.")]
        public int CapacityInAmps { get; set; }
    }
}
