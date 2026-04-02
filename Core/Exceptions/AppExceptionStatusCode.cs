using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Exceptions
{
    public class AppExceptionStatusCode : Exception
    {
        public int StatusCode { get; }

        public AppExceptionStatusCode(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
