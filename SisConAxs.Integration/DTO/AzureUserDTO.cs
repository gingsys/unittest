using SisConAxs_DM.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration.DTO
{
    public class AzureUserDTO
    {
        public string Username { get; set; }
        public string Firtsname { get; set; }
        public string Lastsname { get; set; }
        public string DocNumber { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset? CreatedDateTime { get; set; }

        public IntegrationAADLogDTO ToIntegrationAADLogDTO()
        {
            var log = new IntegrationAADLogDTO
            {
                LogResult = IntegrationAADLogDTO.RESULT_WITHOUT_CHANGES,
                LogDocNumber = DocNumber,
                LogUserName = Username,
                LogNames = Firtsname,
                LogLastnames = Lastsname,
                LogCompanyName = CompanyName,
                LogEmail = Email,
                LogMessage = null
            };

            return log;
        }
    }
}
