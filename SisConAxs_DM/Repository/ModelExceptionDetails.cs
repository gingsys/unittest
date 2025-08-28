using SisConAxs_DM.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Repository
{
    public class ModelExceptionDetails<T> : Exception
    {
        public int Code { get; set; } = -1;
        public T Details { get; set; }


        public ModelExceptionDetails() : base() { }

        public ModelExceptionDetails(string message) : base(message) { }
        public ModelExceptionDetails(string message, Exception innerException) : base(message, innerException) { }
        public ModelExceptionDetails(string message, int code) : base(message)
        {
            this.Code = code;
        }
        public ModelExceptionDetails(string message, int code, T details) : base(message)
        {
            this.Code = code;
            this.Details = details;
        }
        public ModelExceptionDetails(string message, int code, T details, Exception innerException) : base(message, innerException)
        {
            this.Code = code;
            this.Details = details;
        }
        public ModelExceptionDetails(string message, T details) : base(message)
        {
            this.Details = details;
        }
        public ModelExceptionDetails(string message, T details, Exception innerException) : base(message, innerException)
        {
            this.Details = details;
        }

        public ErrorResponseDTO GetErrorResponseDTO()
        {
            return new ErrorResponseDTO()
            {
                Code = this.Code,
                Message = this.Message,
                Details = this.Details
            };
        }
    }
}
