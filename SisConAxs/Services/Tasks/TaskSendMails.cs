using SisConAxs_DM.Models;
using SisConAxs_DM.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;

using MailKit.Net.Smtp;

namespace SisConAxs.Services
{
    public class TaskSendMails : AbstractTask
    {
        private const int LapseOnServerError = 600000;  // 10 min
        private const int LapseOnSuccess = 60000;       // 60 seg

        public TaskSendMails(TaskManager manager) : base(manager)
        {
        }

        public override void Execute(object state)
        {
            try
            {
                this.Lapse = LapseOnSuccess;  // Se inicia con el valor por defecto para el lapso entre las execuciones del hilo

                SisConAxsContext db = new SisConAxsContext();
                List<RequestEmailData> items = new List<RequestEmailData>();
                items = RequestEmailDataStorage.GetAll();

                LogManager.Debug($"> TaskSendMails ({items.Count})");
                foreach (var item in items)
                {
                    NotificationMailer mailer = null;
                    AccessRequests Request = null;

                    try
                    {
                        // Elimina el correo si pasó 1 semana
                        if (DateTime.Now > item.CreateDate.AddDays(7))
                        {
                            RequestEmailDataStorage.Delete(item.Key);
                            continue;
                        }
                        // validaciones reintentos de envío, salta al siguiente si aun no ha cumlido el tiempo de espera
                        if(item.AttemptCount > 1)
                        {
                            if(item.AttemptCount == 2 && DateTime.Now < ((DateTime)item.AttemptDate).AddMinutes(10))  // 10 min
                            {
                                continue;
                            }
                            else if (item.AttemptCount == 3 && DateTime.Now < ((DateTime)item.AttemptDate).AddMinutes(20))  // 20 min
                            {
                                continue;
                            }
                            else if (item.AttemptCount == 4 && DateTime.Now < ((DateTime)item.AttemptDate).AddMinutes(40))  // 40 min
                            {
                                continue;
                            }
                        }

                        // Build mail ----------------------------------------------------------------------------------------------------- //
                        mailer = item.BuildMailer();

                        // Send mail ------------------------------------------------------------------------------------------------------ //
                        Request = db.AccessRequests.FirstOrDefault(t => t.RequestID == item.RequestID);
                        string solNumber = $"- Sol. Nro. {Request.RequestNumber}";
                        string strWfExecParamsIds = $"WfExecParamsIds : {string.Join(",", item.ExecutionsID.Select(x => x.ToString()).ToArray())} -";
                        string destName = item.DestNames;
                        destName = !string.IsNullOrEmpty(destName) ? "/" + destName.ToUpper() : "";
                        LogManager.Log($"> TaskSendMails >> {solNumber} - {strWfExecParamsIds} Enviando el correo para '{mailer.SendTo}' {destName} con asunto '{mailer.Subject}'.");


                        // Solo para test local -----------------------------------------------------------
                        //RequestEmailDataStorage.Delete(item.Key);
                        //mailer.SaveLocal($"Request_{Request.RequestNumber}-Key_'{item.Key}'");
                        // --------------------------------------------------------------------------------

                        if (mailer.SendNotificationMail())
                        {
                            RequestEmailDataStorage.Delete(item.Key);
                            //mailer.SaveLocal($"Request_{Request.RequestNumber}-Key_'{item.Key}'");
                            LogManager.Log($"> TaskSendMails >> {solNumber} - {strWfExecParamsIds} El correo fue enviado para '{mailer.SendTo}'{destName}.");
                        }
                        // Send mail ------------------------------------------------------------------------------------------------------ //
                    }
                    catch (SmtpCommandException ex)
                    {
                        if (ex.ErrorCode is SmtpErrorCode.RecipientNotAccepted/*SmtpFailedRecipientException*/)
                        {
                            if (/*((System.Net.Mail.SmtpFailedRecipientException)(ex)).FailedRecipient*/ex.Mailbox.ToString() != String.Format("<{0}>", mailer.SendCc)) //Adicionado el 20191004 <-- DIANA CAMUS
                            {
                                //revisar validacion
                                if (item.AttemptCount >= 5)  // Al 5to intento envía un mensaje al administrador
                                {
                                    mailer.Body = $"<h5 style='font-size:15px'><span style = 'color:#000080;'>Este mail debio de Enviarse a:</span><strong>{mailer.SendTo}</strong></h5></br>{mailer.Body}";
                                    mailer.SendTo = ConfigurationManager.AppSettings["SupportAccount"].ToString();
                                    mailer.SendNotificationMail();

                                    RequestEmailDataStorage.Delete(item.Key);
                                    mailer.SaveLocal($"Request_{Request.RequestNumber}-Key_'{item.Key}'");
                                }
                                else
                                {
                                    item.AttemptCount += 1;
                                    item.AttemptDate = DateTime.Now;
                                    RequestEmailDataStorage.Update(item);
                                }
                                LogManager.Error($"> TaskSendMails >> ", ex);
                            }
                            else
                            {//Adicionado el 20191029 <-- DIANA CAMUS
                                RequestEmailDataStorage.Delete(item.Key);
                            }
                        }
                        else
                        {
                            // Si te tiene un error en el servidor de correos se vuelve a intentar luego de 10min
                            this.Lapse = LapseOnServerError;
                            LogManager.Error($"> TaskSendMails >> ", ex);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error($"> TaskSendMails >> ", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("> TaskSendMails >> ", ex);
            }
            finally
            {
                Timer.Change(this.Lapse, Timeout.Infinite);
            }
        }
    }
}