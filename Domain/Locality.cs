using IBGE.API.Domain.DTOs;

namespace IBGE.API.Domain;

public sealed class Locality
{
    public int Id { get; set; }
    public string City { get; set; }
    public State State { get; set; }
    public string Filter { get; set; }

    public static Locality ConvertLocalityDtoToLocality(LocalityDto localityDto) {
        
        if (!Enum.TryParse(localityDto.State, out State state))
            throw new Exception("That state not exist");

        /*Locality a = new() { City = localityDto.City, State = state };
        var context = new ValidationContext(a, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(a, context, validationResults, true);

        if (!isValid)
        {
            throw new Exception(validationResults.ToList().ToString());
        }*/

        return new() { City = localityDto.City, State = state };
    }

    public string GetFilter()
    {
        return Filter ?? string.Empty;
    }
}
