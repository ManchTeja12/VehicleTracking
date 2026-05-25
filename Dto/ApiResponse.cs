using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VehicleMangement.Dto
{
    public class ApiResponse
        {
            public ApiResponse()
            {
                error = new ErrorResponse();
            }

            [Required, DefaultValue(false)]
            public bool success { get; set; }

            [DefaultValue(null)]
            public object? data { get; set; }

            [DefaultValue(null)]
            public string? message { get; set; }
            public ErrorResponse error { get; set; }

            [DefaultValue(null)]
            public object? log { get; set; }
        }

        public class ErrorResponse
        {
            [DefaultValue(null), StringLength(10)]
            public string? code { get; set; }

            public string? message { get; set; }
        }
}


