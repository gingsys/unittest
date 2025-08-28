using LiteDB;
using SisConAxs_DM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SisConAxs_DM.Utils
{
    public class RequestEmailDataStorage
    {
        //static private RequestEmailDataStorage Instance = null;
        //static public RequestEmailDataStorage GetInstance()
        //{
        //    if (Instance == null)
        //    {
        //        Instance = new RequestEmailDataStorage();
        //    }
        //    return Instance;
        //}


        static private LiteCollection<RequestEmailData> mailsCollection;
        static private bool initialized = false;
        private RequestEmailDataStorage()
        {
        }

        static public void Initialize()
        {
            if (!initialized)
            {
                using (var db = new LiteDatabase(System.Configuration.ConfigurationManager.AppSettings["PathLocalDB"] + "/MailsData.db")) //@AppDomain.CurrentDomain.BaseDirectory + "Data/Mails.db"))
                {
                    BsonMapper.Global.Entity<RequestEmailData>()
                        .Id(x => x.Key);
                    mailsCollection = db.GetCollection<RequestEmailData>("mails");
                    initialized = true;
                }
            }
        }

        static public void PrepareEmailData(WorkflowExecution wfExec, string defaultSubject = "", string emailDest = "", bool sendAccountMailer = false, string destEmailCc = "", string body = "") //Modificado el 20191004 <-- DIANA CAMUS
        {
            var mail = RequestEmailData.PrepareEmailData(wfExec, defaultSubject, emailDest, sendAccountMailer, destEmailCc, body); //Modificado el 20191004 <-- DIANA CAMUS
            if (mail != null)
            {
                lock (mailsCollection)
                {
                    string newKey = mail.GetKey();
                    if (!mailsCollection.Exists(m => m.Key == newKey))
                    {
                        mail.Key = newKey;
                        mailsCollection.Insert(mail);
                    }
                    mail = mailsCollection.FindOne(m => m.Key == newKey);
                    mail.ExecutionsID.Add(wfExec.WfExecID);
                    mailsCollection.Update(mail);
                }
            }
        }

        static public List<RequestEmailData> GetAll()
        {
            List<RequestEmailData> data = new List<RequestEmailData>();
            lock (mailsCollection)  // copia los mails para reducir el tiempo de bloqueo de la variable
            {
                data.AddRange(mailsCollection.FindAll());
            }
            return data;
        }
        static public void Delete(string key)
        {
            lock (mailsCollection)
            {
                mailsCollection.Delete(x => x.Key == key);
            }
        }
        static public void Update(RequestEmailData mail)
        {
            lock (mailsCollection)
            {
                mailsCollection.Update(mail);
            }
        }
    }
}