using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebhookManager.Model;

namespace WebhookManager.Helpers
{

    public class WebhookManagerException : Exception
    {
        public int StatusCode { get; set; }

        public List<string> Errors { get; set; }

        public WebhookManagerException(string message,
                            int statusCode = 500,
                            List<string> errors = null) :  base(message)
        {
            StatusCode = statusCode;
            Errors = errors ?? new List<string>();
        }

        public WebhookManagerException(Exception ex, int statusCode = 500) : base(ex.Message)
        {
            StatusCode = statusCode;
            Errors = new List<string>();
        }

    }

    public class ApiError
    {
        public string message { get; set; }
        public bool isError { get; set; }
        public string detail { get; set; }
        public List<string> errors { get; set; }

        public ApiError(string message)
        {
            this.message = message;
            isError = true;
        }

        public ApiError(ModelStateDictionary modelState)
        {
            this.isError = true;
        }
    }

    //public class ExceptionHelper
    //{
    //    public static AppErrorReponse UnknowErrorFactory(string message)
    //    {
    //        return new AppErrorReponse
    //        {
    //            Error = "Unknown Error",
    //            Message = message,
    //            Status = "500",
    //            Timestamp = DateTime.Now.ToLongDateString()
    //        };
    //    }
    //}
}
