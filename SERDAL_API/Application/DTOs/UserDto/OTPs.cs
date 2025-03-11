namespace SERDAL_API.Application.DTOs.UserDto
{

        public class OTPs
        {
            public int? Id { get; set; }
            public int? UserId { get; set; }
            public int OTPtypeId { get; set; }
            public string Email { get; set; }
            public string? OTPCode { get; set; }
            public DateTime? ExpiryTime { get; set; }
            public int? isActive { get; set; }
            public DateTime? CreatedDateTime { get; set; }
        }

}
