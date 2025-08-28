using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SisConAxs.Services
{
    public class TaskResourcePeopleExpired : AbstractTask
    {
        public TaskResourcePeopleExpired(TaskManager manager) : base(manager)
        {
            this.Lapse = 30 * 60 * 1000; // 30 min
        }

        public override void Execute(object state)
        {
            try
            {
                var Now = DateTime.Now;

                SisConAxsContext db = new SisConAxsContext();
                var resourcePeople = from rp in db.ResourcePeople
                                      where
                                            rp.PresActive > 0
                                            && rp.PresTemporal > 0
                                            && Now > rp.PresValidityUntil
                                      select rp;

                LogManager.Debug($">> TaskResourcePeopleExpired ({resourcePeople.Count()})");
                foreach (var rp in resourcePeople)
                {
                    try
                    {
                        rp.EditUser = "icarus.taskmanager";
                        rp.ResourcePeopleLog.Add(new ResourcePeopleLog()
                        {
                            Action = AccessRequestDTO.TYPE_BAJA,
                            Source = "ICARUS",
                            Description = "Recurso expirado, se procede con la BAJA."
                        });
                        new ResourcePeopleRepository().SaveResourcePeople(rp, AccessRequestDTO.TYPE_BAJA);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error("> TaskResourcePeopleExpired >> ", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("> TaskResourcePeopleExpired >> ", ex);
            }
            finally
            {
                Timer.Change(this.Lapse, Timeout.Infinite);
            }
        }
    }
}