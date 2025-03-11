using System.Net;
using System.Net.Mail;


namespace SERDAL_API.Helper
{


    public static class Email
    {
        public static bool sendOTP(string OTP, string email) 
        {
            string smtpServer = "smtp.gmail.com";
            string emailAddress = "carlo.cp483@gmail.com";
            string password = "fpfk kdzg gyfu uzul"; 
            int port = 587;

            SmtpClient smtpClient = new SmtpClient(smtpServer)
            {
                Port = port,
                Credentials = new NetworkCredential(emailAddress, password),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(emailAddress),
                Subject = "SERDAL OTP Test Email",
                Body = emailbody(OTP),
                IsBodyHtml = true
            };

            email = "carlo.cp483@gmail.com";
            mailMessage.To.Add(email);

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Credentials are valid! Test email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }

            return true;
        }

        private static string emailbody(string OTP) {

            return $@"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Your OTP Code</title>
                        <style>
                            body {{
                                background-color: #f7fafc;
                                font-family: 'Helvetica Neue', sans-serif;
                                color: #2d3748;
                                margin: 0;
                                padding: 0;
                            }}
                            .container {{
                                max-width: 600px;
                                background-color: #ffffff;
                                padding: 20px;
                                border-radius: 8px;
                                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                            }}

                            .otp-code {{
                                background-color: #edf2f7;
                                color: #17C0CC;
                                font-size: 32px;
                                font-weight: 600;
                                padding: 12px 24px;
                                border-radius: 6px;
                                display: inline-block;
                                margin-top: 5px;
                                margin-bottom:5px;
                            }}
                            .footer {{
                                font-size: 14px;
                                color: #718096;
                                text-align: left;
                                margin-top: 24px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <p class='content'>
                                Your One-Time Password (OTP) is:
                            </p>
                            <div class='otp-code'>
                                <strong>{OTP}</strong>
                            </div>
                            <p class='content'>
                                Please use this OTP within the next 10 minutes. For security reasons, this OTP will expire after that time.
                            </p>
                            <p class='content'>
                                If you did not request this OTP, please disregard this message.
                            </p>
                            <div class='footer'>
                                Thank you for using SERDAL.
                            </div>
                        </div>
                    </body>
                    </html>
                    ";
        }

    }
}
