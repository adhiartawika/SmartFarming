using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace backend.Commons
{
    public interface IUtilityService
    {
        public string HashString(string data);
    }
    public class UtilityService: IUtilityService
    {
        public string HashString(string data)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] hashValue;
            UTF8Encoding objUtf8 = new UTF8Encoding();
            return (sha256.ComputeHash(objUtf8.GetBytes(data))).ToString();
        }
    }
}
