using Microsoft.AspNetCore.Mvc;

namespace SERDAL_API.Helper
{
    public static class Response
    {
        public static IActionResult Code(int code, string message)
        {
            // Informational Responses (1xx)
            if (code == 100)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 100 // 100 Continue
                };
            }

            if (code == 101)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 101 // 101 Switching Protocols
                };
            }

            // Successful Responses (2xx)
            if (code == 200)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 200 // 200 OK
                };
            }

            if (code == 201)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 201 // 201 Created
                };
            }

            if (code == 202)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 202 // 202 Accepted
                };
            }

            if (code == 204)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 204 // 204 No Content
                };
            }

            // Redirection Responses (3xx)
            if (code == 301)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 301 // 301 Moved Permanently
                };
            }

            if (code == 302)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 302 // 302 Found
                };
            }

            if (code == 304)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 304 // 304 Not Modified
                };
            }

            // Client Error Responses (4xx)
            if (code == 400)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 400 // 400 Bad Request
                };
            }

            if (code == 401)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 401 // 401 Unauthorized
                };
            }

            if (code == 403)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 403 // 403 Forbidden
                };
            }

            if (code == 404)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 404 // 404 Not Found
                };
            }

            if (code == 405)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 405 // 405 Method Not Allowed
                };
            }

            if (code == 408)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 408 // 408 Request Timeout
                };
            }

            if (code == 429)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 429 // 429 Too Many Requests
                };
            }

            // Server Error Responses (5xx)
            if (code == 500)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 500 // 500 Internal Server Error
                };
            }

            if (code == 501)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 501 // 501 Not Implemented
                };
            }

            if (code == 502)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 502 // 502 Bad Gateway
                };
            }

            if (code == 503)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 503 // 503 Service Unavailable
                };
            }

            if (code == 504)
            {
                return new ObjectResult(new { Code = code, Message = message })
                {
                    StatusCode = 504 // 504 Gateway Timeout
                };
            }

            // Default fallback for unhandled status codes
            return new ObjectResult(new { Code = 500, Message = "Unknown Error" })
            {
                StatusCode = 500 // 500 Internal Server Error as default
            };
        }
    }
}
