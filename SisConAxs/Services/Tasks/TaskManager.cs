using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Threading;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using SisConAxs_DM.DTO;
using System.Configuration;
using System.IO;
using SisConAxs_DM.Utils;

namespace SisConAxs.Services
{
    public class TaskManager
    {
        public const string TASK_RESOURCEPEOPLE_EXPIRED = "TASK_RESOURCEPEOPLE_EXPIRED";
        public const string TASK_CANCEL_REQUEST = "TASK_CANCEL_REQUEST";
        public const string TASK_WORKFLOWS_PENDING = "TASK_WORKFLOWS_PENDING";
        public const string TASK_RESPONSE_ITEMS = "TASK_RESPONSE_ITEMS";
        public const string TASK_WORKFLOWS_TIMEOUT = "TASK_WORKFLOWS_TIMEOUT";
        public const string TASK_WORKFLOW_ITEMS = "TASK_ITEMS";
        public const string TASK_WORKFLOW_ITEMS_ACTION = "TASK_ITEMS_ACTION";
        public const string TASK_REMINDER_ITEMS = "TASK_REMINDER_ITEMS";
        public const string TASK_SEND_MAILS = "SEND_MAILS";
        

        private Timer ManagerTimer;
        private Dictionary<string, AbstractTask> Tasks = new Dictionary<string, AbstractTask>();
        public int Lapse { get; set; }  // Ciclo en 5 segundos;

        public TaskManager()
        {
            ManagerTimer = new Timer(new TimerCallback(DoTasks), null, 1000 * 2, Timeout.Infinite);
            Lapse = 5;

            Tasks.Add(TASK_RESOURCEPEOPLE_EXPIRED, new TaskResourcePeopleExpired(this));
            Tasks.Add(TASK_CANCEL_REQUEST, new TaskCancelRequest(this));
            Tasks.Add(TASK_WORKFLOWS_PENDING, new TaskWorkflowPending(this));
            Tasks.Add(TASK_RESPONSE_ITEMS, new TaskResponseItems(this));
            Tasks.Add(TASK_WORKFLOWS_TIMEOUT, new TaskWorkflowTimeout(this));
            Tasks.Add(TASK_WORKFLOW_ITEMS, new TaskWorkflowItems(this));
            Tasks.Add(TASK_WORKFLOW_ITEMS_ACTION, new TaskWorkflowItemsAction(this));
            Tasks.Add(TASK_REMINDER_ITEMS, new TaskReminderItems(this)); // Adicionado el 20190107 <-- JONATAN LOBATO
            Tasks.Add(TASK_SEND_MAILS, new TaskSendMails(this));

            //Tasks.Add("Dummy", new TaskDummyWakeUp(this));

            LogManager.Log(String.Format(" > Task Manager >> Creado ciclo en {0} seg", Lapse));
        }

        private void DoTasks(object state)
        {
            string taskStep = "<INIT>";
            try
            {
                foreach (var item in Tasks)
                {
                    if(!item.Value.Initialized)
                    {
                        item.Value.Initialize();
                    }
                    taskStep = item.Key;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("> TaskManager:", ex);
                LogManager.Error("> TaskManager: Hora del servidor : " + System.DateTime.Now + " Ult. paso ejecutado correctamente: " + taskStep);
                ManagerTimer = new Timer(new TimerCallback(DoTasks), null, 1000 * 2, Timeout.Infinite);
            }
            finally
            {
                ManagerTimer.Change(1000 * Lapse, Timeout.Infinite);
            }
        }
    }




    //public class TaskDummyWakeUp : AbstractTask
    //{
    //    public TaskDummyWakeUp(TaskManager manager) : base(manager)
    //    {
    //        Lapse = 10 * 60 * 1000; // 10 min
    //    }

    //    public override void Execute(object state)
    //    {
    //        string path = @"D:\Test-WakeUp.txt";
    //        string text = $"{DateTime.Now} - Ejecutado TaskDummyWakeUp!";

    //        if (!File.Exists(path))
    //        {
    //            using (StreamWriter sw = File.CreateText(path))
    //            {
    //                sw.WriteLine(text);
    //            }
    //        } else
    //        {
    //            using (StreamWriter sw = File.AppendText(path))
    //            {
    //                sw.WriteLine(text);
    //            }
    //        }

    //        Timer.Change(Lapse, Timeout.Infinite);
    //    }
    //}
}