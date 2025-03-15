using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SERDAL_API.Application.DTOs.UserDto;
using SERDAL_API.Application.Models.User;
using SERDAL_API.Data;
using SERDAL_API.Helper;
using SERDAL_API.ServiceLayer;

namespace SERDAL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        UserService _UserService;
        public UsersController(DataContext context)
        {
            _context = context;
            _UserService = new UserService(context);
        }

        [HttpGet]
        [Route("Users")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users  = await _context.users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.users.Where(x => x.IsActive == 1 && x.ID == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound($"User not found");
            }
            return Ok(user);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateUser(User user)
        {
            DateTime dt = DateTime.Now;
            var getUser = await _UserService.CreateUser(user);
            

            return Ok(await _context.users.Where(x => x.Email == getUser.Data.Email).FirstOrDefaultAsync());
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(User user)
        {
            DateTime dt = DateTime.Now;

            var User = await _context.users.Where(u => u.Email.ToLower().Trim() == user.Email.ToLower().Trim() && u.IsActive == 1).FirstOrDefaultAsync();

            if (User == null)
            {
                return BadRequest(new { message = "Invalid email or password" });
            }

            // Initialize the password hasher
            var passwordHasher = new PasswordHasher<User>();

            // Verify the provided password against the stored hash
            var verificationResult = passwordHasher.VerifyHashedPassword(User, User.Password, user.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return BadRequest(new { message = "Invalid email or password" });
            }

            // If password matches, return the user object
            return Ok(User);
        }


        [HttpGet("ResetPasswordOTP/{email}")]
        public async Task<IActionResult> sendResetPasswordOTP(string email)
        {
            var OTP = await _UserService.ResetPasswordOTP(email);
            if (!OTP.Success)
            {
                return Helper.Response.Code(OTP.Code, OTP.ErrorMessage);
            }

            return Ok(OTP.Data.OTPCode);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO user)
        {
            var userData = await _context.users.Where(x => x.Email.ToLower() == user.email.ToLower()).FirstOrDefaultAsync();


            var passwordHasher = new PasswordHasher<User>();
            userData.Password = passwordHasher.HashPassword(userData, user.password);
            userData.ModifiedBy = userData.ID;
            userData.ModifiedDateTime = DateTime.Now;
            _context.users.Update(userData);
            await _context.SaveChangesAsync();
            return Ok(userData);
        }


        #region OTP
        [HttpPost("sendOTP")]
            public async Task<IActionResult> SendOTP(OTPs otp)
            {

                var OTP = await _UserService.SendOTP(otp);
                if (!OTP.Success)
                {
                    return Helper.Response.Code(OTP.Code, OTP.ErrorMessage);
                }

                return Ok(OTP.Data.OTPCode);
            }

            [HttpPost("verifyOTP")]
            public async Task<ActionResult<User>> VerifyOTP(OTPs user)
            {
                DateTime dt = DateTime.Now;
                var otps = await _context.otp.FindAsync(user.Id);
                if (otps == null)
                {
                    BadRequest();
                }

                if (otps.OTPCode == user.OTPCode)
                {
                    return Ok();
                }
                else
                {
                    return NotFound(new { code = 404, message = "Invalid OTP code" });
                }

            }
            #endregion



        //comment test1
            //[HttpPut]
            //public async Task<ActionResult<List<User>>> UpdateUser(User user)
            //{
            //    var tbluser = await _context.users.FindAsync(user.ID);
            //    if (tbluser == null)
            //    {
            //        return NotFound($"User not found");
            //    }

            //    tbluser.FirstName = user.FirstName;
            //    tbluser.LastName = user.LastName;
            //    tbluser.Email = user.Email;
            //    tbluser.IsActive = user.IsActive;
            //    tbluser.Role = user.Role;
            //    tbluser.Img = user.Img;
            //    tbluser.CreateDateTime = tbluser.CreateDateTime;

            //    await _context.SaveChangesAsync();

            //    return Ok(await _context.users.ToListAsync());
            //}

            //[HttpPost]
            //public async Task<ActionResult<User>> ChangeStatus(Status status)
            //{
            //    var user = await _context.users.FindAsync(status.Id);
            //    if (user == null)
            //    {
            //        return NotFound($"User not found");
            //    }

            //    user.IsActive = status.status;
            //    _context.users.Add(user);
            //    await _context.SaveChangesAsync();

            //    return Ok(await _context.users.ToListAsync());
            //}

    }


        //[HttpPost("Check")]
        //public async Task<ActionResult<List<User>>> CheckUser(User user)
        //{
        //    try
        //    {
        //        DateTime dt = DateTime.Now;
        //        var tbluser = await _context.users.FirstOrDefaultAsync(e => e.Email == user.Email);
        //        if (tbluser == null)
        //        {
        //            //Create if not exist
        //            user.ID = 0;
        //            user.ModifiedDateTime = dt.AddMicroseconds(-dt.Microsecond);
        //            user.CreateDateTime = dt.AddMicroseconds(-dt.Microsecond);
        //            _context.users.Add(user);
        //            await _context.SaveChangesAsync();

        //            return Ok(await _context.users.ToListAsync());
        //        }

        //        tbluser.FirstName = user.FirstName;
        //        tbluser.LastName = user.LastName;
        //        tbluser.Email = user.Email;
        //        tbluser.IsActive = user.IsActive;
        //        //tbluser.Role = user.Role;
        //        tbluser.Img = user.Img;
        //        tbluser.CreateDateTime = tbluser.CreateDateTime;

        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {


        //    }

        //    return Ok(await _context.users.FirstOrDefaultAsync(email => email.Email == user.Email));
        //}





        #region Delete
        //[HttpDelete]
        //public async Task<ActionResult<List<Users>>> DeleteUser(int id)
        //{
        //    var tbluser = await _context.users.FindAsync(id);
        //    if (tbluser == null)
        //    {
        //        return NotFound($"User not found");
        //    }

        //    _context.users.Remove(tbluser);
        //    await _context.SaveChangesAsync();

        //    return Ok(await _context.users.ToListAsync());
        //}
        #endregion

}
