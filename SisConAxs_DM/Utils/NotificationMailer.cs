using SisConAxs_DM.DTO;
using SisConAxs_DM.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using MimeKit;
using MailKit;
using MailKit.Net.Smtp;

namespace SisConAxs_DM.Utils
{
    public class NotificationMailer
    {
        public string SendTo { get; set; }          // Notification Destinatary
        public string SendCc { get; set; }          // Adicionado el 20191004 <-- DIANA CAMUS
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool SendAccountMailer { get; set; }

        private List<MimePart> attachments { get; set; } = new List<MimePart>();
        private NotifConfigDTO NotifConfig;

        private void GetNotifConfig(NotifConfigDTO dto = null)
        {
            if (dto == null)
                NotifConfig = new SystemConfigRepository().GetNotifConfig();
            else
                NotifConfig = dto;
        }

        public void AddAttachment(string path, string cid = null)
        {
            path = AppDomain.CurrentDomain.BaseDirectory.ToLower() + path;
            MimePart inline = new MimePart();
            //inline.ContentDisposition.Inline = true;
            inline.Content = new MimeContent(File.OpenRead(path), ContentEncoding.Default);
            inline.ContentDisposition = new ContentDisposition(ContentDisposition.Inline);
            //inline.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
            inline.ContentId = String.IsNullOrEmpty(cid) ? Guid.NewGuid().ToString() : cid;
            inline.ContentType.MediaType = MimeTypeMap.GetMimeType(Path.GetExtension(path));
            inline.ContentType.Name = Path.GetFileName(path);
            inline.ContentTransferEncoding = ContentEncoding.Base64;
            inline.FileName = Path.GetFileName(path);
            attachments.Add(inline);
        }

        public bool SendNotificationMail(NotifConfigDTO dto = null)
        {
            int PortNum = 25; //Default value

            GetNotifConfig(dto);

            try
            {
                //SmtpClient client = new SmtpClient(NotifConfig.NotifConfHost);
                //Int32.TryParse(NotifConfig.NotifConfPort, out PortNum);
                //client.UseDefaultCredentials = false;
                //client.Credentials = new NetworkCredential(NotifConfig.NotifConfUser, NotifConfig.NotifConfLock);
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.Port = PortNum;
                //client.EnableSsl = NotifConfig.NotifConfSSL.Equals("1");

                //MailMessage mail = this.BuildMailMessage();
                //client.Send(mail);

                SmtpClient client = new SmtpClient();
                Int32.TryParse(NotifConfig.NotifConfPort, out PortNum);
                client.CheckCertificateRevocation = false;
                client.Connect(NotifConfig.NotifConfHost, PortNum, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate(NotifConfig.NotifConfUser, NotifConfig.NotifConfLock);
                MimeMessage mail = this.BuildMailMessage();
                client.Send(mail);
                client.Disconnect(true);

            }
            catch (SmtpCommandException /*Exception*/ ex)
            {
                if (ex.ErrorCode is /*SmtpFailedRecipientException*/SmtpErrorCode.RecipientNotAccepted)
                    LogManager.Error($"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] No se recepciono el correo.", ex);
                else if (ex.ErrorCode is /*SmtpException*/SmtpErrorCode.SenderNotAccepted)
                    LogManager.Error($"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] Error al intentar enviar correo.", ex);
                else
                    LogManager.Error($"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] Error desconocido.", ex);

                throw ex;
            }
            return true;
        }

        public void SaveLocal(string name)
        {
            GetNotifConfig();
            //SmtpClient client = new SmtpClient(NotifConfig.NotifConfHost);
            //client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            //client.PickupDirectoryLocation = System.Configuration.ConfigurationManager.AppSettings["PathMails"];

            MimeMessage mail = this.BuildMailMessage();
            mail.WriteTo($"{System.Configuration.ConfigurationManager.AppSettings["PathMails"]}/{name}.eml");
            //client.Send(mail);
        }

        private MimeMessage/*MailMessage*/ BuildMailMessage()
        {
            //MailMessage mail = new MailMessage();
            //mail.From = new MailAddress(NotifConfig.NotifConfUser);
            //mail.To.Add(this.SendTo);
            ////INICIO Adicionado el 20191004 <-- DIANA CAMUS
            //if (!string.IsNullOrEmpty(this.SendCc))
            //{
            //    mail.CC.Add(this.SendCc);
            //}
            ////FIN Adicionado el 20191004 <-- DIANA CAMUS
            //mail.Subject = this.Subject;
            //if (SendAccountMailer) { mail.Bcc.Add(NotifConfig.NotifConfUser); }
            //foreach (Attachment a in attachments)
            //    mail.Attachments.Add(a);
            //mail.Body = this.Body;
            //mail.IsBodyHtml = true;
            //return mail;

            MimeMessage mail = new MimeMessage();
            mail.From.Add(new MailboxAddress("From", NotifConfig.NotifConfUser));
            mail.To.Add(new MailboxAddress("To", this.SendTo));
            //INICIO Adicionado el 20191004 <-- DIANA CAMUS
            if (!string.IsNullOrEmpty(this.SendCc))
            {
                mail.Cc.Add(new MailboxAddress("CC",this.SendCc));
            }
            //FIN Adicionado el 20191004 <-- DIANA CAMUS
            mail.Subject = this.Subject;
            if (SendAccountMailer) { mail.Bcc.Add(new MailboxAddress("BCC",NotifConfig.NotifConfUser)); }            
            BodyBuilder builder = new BodyBuilder();
            foreach (MimePart a in attachments)
            {
                builder.Attachments.Add(a);
            }
            //builder.Attachments.Add(AppDomain.CurrentDomain.BaseDirectory.ToLower() + "Content/images/logo_full.png");
            builder.HtmlBody = this.Body;
            mail.Body = builder.ToMessageBody();
            
            return mail;
        }
    }


    // !!! Extends MailMessage !!! IMPORTANTE ES UN METODO HACKEADO, EN CASO DE CAMBIAR DE VERSION DEL FRAMEWORK .NET VERIFICAR QUE FUNCIONE CON EL METODO COMENTADO
    //static void Main(string[] args)
    //{
    //    try
    //    {
    //        MailMessage _testMail = new MailMessage();
    //        _testMail.Body = "This is a test email";
    //        _testMail.To.Add(new MailAddress("email@domain.com"));
    //        _testMail.From = new MailAddress("sender@domain.com");
    //        _testMail.Subject = "Test email";
    //        _testMail.Save(@"D:\Work\Free\GYM\SGA\mails\testemail.eml");
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex);
    //    }
    //}
    public static class MailMessageExt
    {
        public static void Save(this MimeMessage Message, string FileName)
        {
            Assembly assembly = typeof(SmtpClient).Assembly;
            Type _mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

            using (FileStream _fileStream = new FileStream(FileName, FileMode.Create))
            {
                // Get reflection info for MailWriter contructor
                ConstructorInfo _mailWriterContructor =
                    _mailWriterType.GetConstructor(
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new Type[] { typeof(Stream) },
                        null);

                // Construct MailWriter object with our FileStream
                object _mailWriter = _mailWriterContructor.Invoke(new object[] { _fileStream });

                // Get reflection info for Send() method on MailMessage
                MethodInfo _sendMethod =
                    typeof(MimeMessage).GetMethod(
                        "Send",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                // Call method passing in MailWriter
                _sendMethod.Invoke(
                    Message,
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] { _mailWriter, true, true },
                    null);

                // Finally get reflection info for Close() method on our MailWriter
                MethodInfo _closeMethod =
                    _mailWriter.GetType().GetMethod(
                        "Close",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                // Call close method
                _closeMethod.Invoke(
                    _mailWriter,
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] { },
                    null);
            }
        }
    }
}
