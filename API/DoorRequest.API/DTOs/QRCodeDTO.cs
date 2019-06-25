namespace DoorRequest.API.DTOs
{
    public class QRCodeDTO
    {
        public string Image { get; set; }
    }

    public class SetupQRCodeDTO
    {
        public string ManualSetupKey { get; set; }
        public string Image { get; set; }
    }

    public class ValidatateSetupDTO
    {
        public bool IsSuccess { get; set; }
    }
}
