using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SERDAL_API.Application.DTOs.UserDto;
using SERDAL_API.Application.Models.User;
using SERDAL_API.Application.Services;
using SERDAL_API.Data;
using SERDAL_API.Helper;

namespace SERDAL_API.ServiceLayer
{
    public class UserService: ServiceReponse
    {

        private readonly DataContext _context;
        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<User>> CreateUser(User userInfo)
        {
            DateTime dt = DateTime.Now;

            var getUser = await _context.users.Where(x => x.Email == userInfo.Email).FirstOrDefaultAsync();
            if (getUser != null)
            {
                return new ServiceResponse<User>
                {
                    Code = 400,
                    Success = false,
                    ErrorMessage = "User already exists. Please login your account."
                };
            }

            var passwordHasher = new PasswordHasher<User>();
            userInfo.Password = passwordHasher.HashPassword(userInfo, userInfo.Password);

            userInfo.IsActive = 0;
            userInfo.CreateDateTime = dt;
            userInfo.Role = "user";

            _context.users.Add(userInfo);
            await _context.SaveChangesAsync();

            return new ServiceResponse<User>
            {
                Code = 200,
                Success = true,
                Data = userInfo
            };
        }

        public async Task<ServiceResponse<OTPs>> SendOTP(OTPs otp)
        {
            DateTime dt = DateTime.Now;
            var user = await _context.users.Where(o => o.Email == otp.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                return new ServiceResponse<OTPs>
                {
                    Code = 400,
                    Success = false,
                    ErrorMessage = "UUser is already exist. please login your account"
                };

            }

            var OTPdata = otp;
            
            var random = new Random();
            string OTP = random.Next(100000, 999999).ToString();

            OTPdata.UserId = 0;
            OTPdata.OTPCode = OTP;
            OTPdata.isActive = 0;
            OTPdata.CreatedDateTime = dt;
            OTPdata.ExpiryTime = dt.AddMinutes(10);

            //_context.otp.Add(OTPdata);
            //await _context.SaveChangesAsync();

            if (!Email.sendOTP(OTP, otp.Email))
            {
                return new ServiceResponse<OTPs>
                {
                    Code = 400,
                    Success = false,
                    ErrorMessage = "Failed to send OTP"
                };
            }

            return new ServiceResponse<OTPs>
            {
                Code = 200,
                Success = true,
                Data = OTPdata
            };
        }






    }
}
