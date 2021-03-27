using System.ComponentModel.DataAnnotations;

namespace GreenFlux.Application.DtoModels
{
    public class SaveChargeStationDto
    {
        [Required]
        public string Name { get; set; }

        [MinLength(Domain.Constants.MinConnectorsInChargeStation, ErrorMessage = "The field {0} must be an array type with a minumum length of '{1}'.")]
        [MaxLength(Domain.Constants.MaxConnectorsInChargeStation, ErrorMessage = "The field {0} must be an array type with a maximum length of '{1}'.")]
        public SaveConnectorDto[] Connectors { get; set; }
    }
}
