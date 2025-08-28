using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using AutoMapper;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Data;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Utilities;

namespace SisConAxs_DM.Repository
{
    public class UsedCounts
    {
        public String Count_Name { get; set; }
        public int Count { get; set; }
    }

    public class WorkflowRepository : AxsBaseRepository
    {
        public WorkflowRepository(SessionData sessionData)
        {
            this.sessionData = sessionData;
            dbSet = db.Workflow;
        }

        /// <summary>Llamada para obtener la lista de Workflow.</summary>
        /// <para>GET api/Workflows </para>
        /// <returns>Lista de Workflow</returns>
        public IQueryable<WorkflowDTO> GetWorkflows()
        {
            var query = from wf in db.Workflow
                        join fwah in db.WorkflowApproveHierarchy on wf.WfApproveHierarchyID equals fwah.WfApproveHierarchyID into wfApproveHierarchy
                        from fwah in wfApproveHierarchy.DefaultIfEmpty()
                        where
                            wf.WfActivo == 1
                            && wf.WfCompany == this.sessionData.CompanyID
                        orderby
                            wf.WfName
                        select new WorkflowDTO()
                        {
                            WfID = wf.WfID,
                            WfName = wf.WfName,
                            WfDescription = wf.WfDescription,
                            WfApproveHierarchyID = fwah.WfApproveHierarchyID,
                            WfApproveHierarchyName = fwah.WfApproveHierarchyName
                        };
            return query;
        }


        public WorkflowDTO GetWorkflowById(int id)
        {
            SisConAxsContext db = new SisConAxsContext();

            Workflow workflow = db.Workflow.FirstOrDefault(wf => wf.WfID == id);
            WorkflowDTO workflowDTO = null;
            if (workflow != null)
            {
                workflowDTO = new WorkflowDTO();
                Mapper.Map<Workflow, WorkflowDTO>(workflow, workflowDTO);

                foreach (WorkflowItemsDTO items in workflowDTO.WorkflowItems)
                {
                    foreach (WorkflowItemNextDTO itemNext in items.WorkflowItemNextParents)
                    {
                        switch (itemNext.WfItemNextType)
                        {
                            case 1: items.WfItemNextItem = itemNext.WfItemNextStep; break;
                            case 2: items.WfItemApproveItem = itemNext.WfItemNextStep; break;
                            case 3: items.WfItemRejectItem = itemNext.WfItemNextStep; break;
                            case 4: items.WfItemTimeoutItem = itemNext.WfItemNextStep; break;
                        }
                    }
                }
            }
            workflowDTO.WorkflowItems = workflowDTO.WorkflowItems.OrderBy(x => x.WfItemStep).ToList();
            return workflowDTO;
        }
        /*
        public IQueryable<WorkflowDTO> GetWorkflowById(int id)
        {
            var query = from wf in db.Workflow
                        join fwah in db.WorkflowApproveHierarchy on wf.WfApproveHierarchyID equals fwah.WfApproveHierarchyID into wfApproveHierarchy
                        from fwah in wfApproveHierarchy.DefaultIfEmpty()
                        where wf.WfID == id
                        select new WorkflowDTO()

                        {
                            WfID = wf.WfID,
                            WfApproveHierarchyID = wf.WfApproveHierarchyID,
                            WfApproveHierarchyName = fwah.WfApproveHierarchyName,
                            WfName = wf.WfName,
                            WfDescription = wf.WfDescription,
                            WorkflowItems = (
                                from wfi in db.WorkflowItems
                                join cv in db.CommonValues on wfi.WfItemType equals cv.CommonValueID into wfiItemType
                                from cv in wfiItemType.DefaultIfEmpty()
                                where wfi.WfItemWfID == wf.WfID
                                orderby wfi.WfItemStep
                                select new WorkflowItemsDTO()
                                {
                                    WfItemId = wfi.WfItemId,
                                    WfItemWfID = wfi.WfItemWfID,
                                    WfItemName = wfi.WfItemName,
                                    WfItemType = wfi.WfItemType,
                                    WfItemTypeName = cv.CommonValueName,
                                    WfItemSubject = wfi.WfItemSubject,
                                    WfItemMessage = wfi.WfItemMessage,
                                    WfItemStep = wfi.WfItemStep,
                                    WfItemDestType = wfi.WfItemDestType,                                    
                                    WfItemDestMail = wfi.WfItemDestMail,
                                    //WfItemNextItem = wfi.WfItemNextItem,
                                    //WfItemApproveItem = wfi.WfItemApproveItem,
                                    //WfItemRejectItem = wfi.WfItemRejectItem,
                                    //WfItemTimeoutItem = wfi.WfItemTimeoutItem,
                                    WfItemTimeoutDueTime = wfi.WfItemTimeoutDueTime,
                                    WfItemTimeoutDueUnits = wfi.WfItemTimeoutDueUnits,
                                    WfItemActionProperty = wfi.WfItemActionProperty,
                                    WfItemActionValue = wfi.WfItemActionValue
                                }
                            ).ToList()
                        };
            return query;
        }
        */


        public WorkflowDTO SaveWorkflow(WorkflowDTO dto, string userId)
        {
            try
            {
                //SisConAxsContext db = new SisConAxsContext();
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Workflow model = null;
                        if (dto.WfID == 0)
                            model = db.Workflow.Create();  // create new from context
                        else
                            model = db.Workflow.FirstOrDefault(a => a.WfID == dto.WfID);  // get from context

                        Mapper.Map<WorkflowDTO, Workflow>(dto, model);
                        model.WfStartDate = DateTime.Today;
                        model.EditUser = userId;
                        model.WfActivo = 1;
                        model.WfCompany = this.sessionData.CompanyID;

                        //PrepareWorkflowItems(model, dto);
                        if (Validate(model, dto))
                        {
                            if (SaveEntity(model.WfID == 0, model))
                            {
                                dto.WfID = model.WfID;
                                SaveWorkflowItems(dto, model, userId);
                                SaveNextItems(dto, model);
                                transaction.Commit();
                                return dto;
                            }
                        }
                    }
                    catch (ModelException ex)  // Si es un error provocado por el usuario
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                    catch (Exception ex)        // Otros errores
                    {
                        transaction.Rollback();
                        if (ex is DataException && ex.InnerException.InnerException != null)
                        {
                            if (ex.InnerException.InnerException is SqlException)
                            {
                                var sqlEx = ex.InnerException.InnerException as SqlException;
                                if (sqlEx.Number == 547 && sqlEx.Message.Contains("\"FK_WORKFLOW_EXECUTION_WORKFLOW_ITEMS\""))   // Number: 547 -> FK exception
                                {
                                    throw new Exception("Error al tratar de borrar un item del workflow (Existen dependencias).", ex);
                                }
                            }
                        }
                        throw new Exception("Error al grabar el workflow.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public WorkflowDTO CopyWorkflow(int wfID, string userId)
        {
            try
            {
                WorkflowDTO dto = new WorkflowDTO();

                dto = GetWorkflowById(wfID);
                dto.WfID = 0;

                Random rnd = new Random();
                int random = rnd.Next(52);
                dto.WfName = dto.WfName + " - copy" + random.ToString();

                foreach(var item in dto.WorkflowItems)
                {
                    item.WfItemId = 0;
                }

                SaveWorkflow(dto, userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public bool DeleteWorkflow(int id, string userId)
        {
            int usedByResources = 0;
            int usedByActiveResources = 0;
            int usedByWFExecs = 0;

            var workflowIdParam = new System.Data.SqlClient.SqlParameter("@WORKFLOW_ID", id);
            List<UsedCounts> usedCount = db.Database.SqlQuery<UsedCounts>("AXSCONTROL.SP_COUNT_WORKFLOW_USES @WORKFLOW_ID", workflowIdParam).ToList();

            foreach (UsedCounts cnt in usedCount)
            {
                if (cnt.Count_Name == "COUNT_RES") usedByResources = cnt.Count;
                if (cnt.Count_Name == "COUNT_ACTIVE_RES") usedByActiveResources = cnt.Count;
                if (cnt.Count_Name == "COUNT_EXECS") usedByWFExecs = cnt.Count;
            }

            // No esta en uso ni por Recursos ni por Workflow
            if (usedByResources == 0 && usedByWFExecs == 0)
            {
                // Si no está en uso lo borramos
                Workflow model = db.Workflow.FirstOrDefault(a => a.WfID == id);
                return DeleteEntity(model);
            }

            // Si el Workflow está en uso en algun Recurso ACTIVO, no podemos borrar
            if (usedByActiveResources > 0)
            {
                throw new ModelException(String.Format("El Workflow no se puede eliminar por que está en uso por {0} Recursos activos.", usedByActiveResources));
            }

            // Si el Workflow está en uso en alguna ejecución o por Recursos inactivos sólo lo desactivamos
            try
            {
                WorkflowDTO dto = new WorkflowDTO { WfID = id };
                Workflow model = db.Workflow.FirstOrDefault(a => a.WfID == id);

                model.WfActivo = 0;
                return SaveEntity(model.WfID == 0, model);
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message, ex);
            }
        }

        private bool Validate(Workflow model, WorkflowDTO dto)
        {
            // validate name
            if (String.IsNullOrEmpty(model.WfName))
            {
                throw new ModelException("No ha asignado un nombre.");
            }

            // validate description
            if (String.IsNullOrEmpty(model.WfDescription))
            {
                throw new ModelException("No ha asignado una descripción.");
            }

            // valiate wf hierarchy
            if(model.WfApproveHierarchyID == null)
            {
                throw new ModelException("No ha asignado una jerarquía de aprobación.");
            }

            // validate repeat name
            int count = db.Workflow.Count(
                    t => t.WfID != model.WfID
                         && t.WfCompany == this.sessionData.CompanyID
                         && t.WfName.Trim().ToUpper() == model.WfName.Trim().ToUpper()
                );
            if (count > 0)
            {
                throw new ModelException(String.Format("El nombre '{0}' ya esta siendo usado.", dto.WfName));
            }

            // detalle ------------------------------------------------------------------------------------- //
            if (dto.WorkflowItems.Count == 0)
            {
                throw new ModelException("El workflow no se puede grabar sin detalle.");
            }
            // next items
            foreach (var item in dto.WorkflowItems)
            {
                int?[] nextItems = new int?[4] { item.WfItemNextItem, item.WfItemApproveItem, item.WfItemRejectItem, item.WfItemTimeoutItem };
                if (nextItems.Where(x => x != null).GroupBy(x => x).Any(x => x.Count() > 1))
                {
                    throw new ModelException($"El item '{item.WfItemName}' tiene acciones siguientes que apuntan al mismo item.");
                }
            }

            return true;
        }

        private void SaveWorkflowItems(WorkflowDTO dto, Workflow model, string userId)
        {
            //// Eliminamos los Items Next que pertenecen a este Workflow
            IEnumerable<WorkflowItemNext> wfItemsNextToDelete = db.WorkflowItemNext.Where(wfn => wfn.WfItemNextWfID == dto.WfID);
            db.WorkflowItemNext.RemoveRange(wfItemsNextToDelete);
            //db.SaveChanges();
            //db.Entry(model).Reload();

            // Buscamos los item que ya no estan para eliminarlos
            List<WorkflowItems> listToRemove = new List<WorkflowItems>();
            IQueryable<WorkflowItems> wfItems = model.WorkflowItems.AsQueryable(); //db.WorkflowItems.Where(wf => wf.WfItemWfID == dto.WfID);
            foreach (WorkflowItems item in wfItems)
            {
                WorkflowItemsDTO tmpItem = dto.WorkflowItems.FirstOrDefault(w => w.WfItemId == item.WfItemId);
                if (tmpItem == null)
                {
                    listToRemove.Add(item);
                }
            }
            if (listToRemove.Count() > 0)
            {
                db.WorkflowItems.RemoveRange(listToRemove);
            }


            // recorremos los items a grabar
            WorkflowItems itemModel;
            foreach (WorkflowItemsDTO itemDTO in dto.WorkflowItems)
            {
                if (itemDTO.WfItemId == 0)
                {
                    itemModel = db.WorkflowItems.Create();  // create new from context
                    Mapper.Map<WorkflowItemsDTO, WorkflowItems>(itemDTO, itemModel);
                    itemModel.WfItemWfID = dto.WfID;
                    itemModel.EditUser = userId;
                    db.WorkflowItems.Add(itemModel);
                }
                else
                {
                    itemModel = wfItems.FirstOrDefault(wfi => wfi.WfItemId == itemDTO.WfItemId);
                    if (itemModel != null)
                    {
                        //db.Entry(itemModel).Reload();
                        Mapper.Map<WorkflowItemsDTO, WorkflowItems>(itemDTO, itemModel);
                        itemModel.WfItemWfID = dto.WfID;
                        db.WorkflowItems.Attach(itemModel);
                        db.Entry(itemModel).State = EntityState.Modified;
                    }
                }
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Grabamos los Next de cada Item
        /// </summary>
        /// <param name="dto"></param>
        private void SaveNextItems(WorkflowDTO dto, Workflow model)
        {
            // Recorremos los items grabados
            IQueryable<WorkflowItems> wfItems = model.WorkflowItems.AsQueryable(); //db.WorkflowItems.Where(wf => wf.WfItemWfID == dto.WfID);
            foreach (WorkflowItems item in wfItems)
            {
                // si el dto tiene el paso del item actual
                WorkflowItemsDTO wfItemsDTO = dto.WorkflowItems.FirstOrDefault(i => i.WfItemStep == item.WfItemStep);
                if (wfItemsDTO != null)
                {

                    if (wfItemsDTO.WfItemNextItem != null && wfItemsDTO.WfItemNextItem != 0)
                    {
                        WorkflowItemNext nextItem = db.WorkflowItemNext.Create();
                        nextItem.WfParentItemID = item.WfItemId;
                        WorkflowItems searchItem = wfItems.FirstOrDefault(s => s.WfItemStep == (int)wfItemsDTO.WfItemNextItem);
                        if (searchItem != null)
                        {
                            nextItem.WfItemNextWfID = dto.WfID;
                            nextItem.WfItemNextItemID = searchItem.WfItemId;  //(int)wfItemsDTO.WfItemNextItem;
                            nextItem.WfItemNextType = 1;
                            nextItem.WfItemNextStep = (int)wfItemsDTO.WfItemNextItem;
                            db.WorkflowItemNext.Add(nextItem);
                        }
                        else
                        {
                            wfItemsDTO.WfItemNextItem = null;
                        }
                    }

                    if (wfItemsDTO.WfItemApproveItem != null && wfItemsDTO.WfItemApproveItem != 0)
                    {
                        WorkflowItemNext nextItem = db.WorkflowItemNext.Create();
                        nextItem.WfParentItemID = item.WfItemId;
                        WorkflowItems searchItem = wfItems.FirstOrDefault(s => s.WfItemStep == (int)wfItemsDTO.WfItemApproveItem);
                        if (searchItem != null)
                        {
                            nextItem.WfItemNextWfID = dto.WfID;
                            nextItem.WfItemNextItemID = searchItem.WfItemId;  //(int)wfItemsDTO.WfItemApproveItem;
                            nextItem.WfItemNextType = 2;
                            nextItem.WfItemNextStep = (int)wfItemsDTO.WfItemApproveItem;
                            db.WorkflowItemNext.Add(nextItem);
                        }
                        else
                        {
                            wfItemsDTO.WfItemApproveItem = null;
                        }
                    }

                    if (wfItemsDTO.WfItemRejectItem != null && wfItemsDTO.WfItemRejectItem != 0)
                    {
                        WorkflowItemNext nextItem = db.WorkflowItemNext.Create();
                        nextItem.WfParentItemID = item.WfItemId;
                        WorkflowItems searchItem = wfItems.FirstOrDefault(s => s.WfItemStep == (int)wfItemsDTO.WfItemRejectItem);
                        if (searchItem != null)
                        {
                            nextItem.WfItemNextWfID = dto.WfID;
                            nextItem.WfItemNextItemID = searchItem.WfItemId;  //(int)wfItemsDTO.WfItemRejectItem;
                            nextItem.WfItemNextType = 3;
                            nextItem.WfItemNextStep = (int)wfItemsDTO.WfItemRejectItem;
                            db.WorkflowItemNext.Add(nextItem);
                        }
                        else
                        {
                            wfItemsDTO.WfItemRejectItem = null;
                        }
                    }

                    if (wfItemsDTO.WfItemTimeoutItem != null && wfItemsDTO.WfItemTimeoutItem != 0)
                    {
                        WorkflowItemNext nextItem = db.WorkflowItemNext.Create();
                        nextItem.WfParentItemID = item.WfItemId;
                        WorkflowItems searchItem = wfItems.FirstOrDefault(s => s.WfItemStep == (int)wfItemsDTO.WfItemTimeoutItem);
                        if (searchItem != null)
                        {
                            nextItem.WfItemNextWfID = dto.WfID;
                            nextItem.WfItemNextItemID = searchItem.WfItemId;  //(int)wfItemsDTO.WfItemTimeoutItem;
                            nextItem.WfItemNextType = 4;
                            nextItem.WfItemNextStep = (int)wfItemsDTO.WfItemTimeoutItem;
                            db.WorkflowItemNext.Add(nextItem);
                        }
                        else
                        {
                            wfItemsDTO.WfItemTimeoutItem = null;
                        }
                    }
                }
            }
            db.SaveChanges();


            //    item.WorkflowItemNext = new List<WorkflowItemNextDTO>();
            //    WorkflowItemNextDTO nextItem = null;
            //    WorkflowItemNextDTO approveItem = null;
            //    WorkflowItemNextDTO rejectItem = null;
            //    WorkflowItemNextDTO timeoutItem = null;

            //    if (item.WfItemNextItem != null) {
            //        nextItem = new WorkflowItemNextDTO();
            //        nextItem.WfParentItemID = item.WfItemId;
            //        nextItem.WfItemNextItemID = (int)item.WfItemNextItem;
            //        nextItem.WFItemNextType = 1;
            //        item.WorkflowItemNext.Add(nextItem);
            //    }
            //    if (item.WfItemApproveItem != null)
            //    {
            //        approveItem = new WorkflowItemNextDTO();
            //        approveItem.WfParentItemID = item.WfItemId;
            //        approveItem.WfItemNextItemID = (int)item.WfItemApproveItem;
            //        approveItem.WFItemNextType = 2;
            //        item.WorkflowItemNext.Add(approveItem);
            //    }
            //    if (item.WfItemRejectItem != null)
            //    {
            //        rejectItem = new WorkflowItemNextDTO();
            //        rejectItem.WfParentItemID = item.WfItemId;
            //        rejectItem.WfItemNextItemID = (int)item.WfItemRejectItem;
            //        rejectItem.WFItemNextType = 3;
            //        item.WorkflowItemNext.Add(rejectItem);
            //    }
            //    if (item.WfItemTimeoutItem != null)
            //    {
            //        timeoutItem = new WorkflowItemNextDTO();
            //        timeoutItem.WfParentItemID = item.WfItemId;
            //        timeoutItem.WfItemNextItemID = (int)item.WfItemTimeoutItem;
            //        timeoutItem.WFItemNextType = 4;
            //        item.WorkflowItemNext.Add(timeoutItem);
            //    }
            //}
        }

        //private void PrepareWorkflowItems(Workflow model, WorkflowDTO dto)
        //{
        //    WorkflowItems modelItem;
        //    WorkflowItemsDTO dtoItem;
        //    List<WorkflowItems> listToRemove = new List<WorkflowItems>();

        //    // update and find items to remove
        //    foreach (WorkflowItems item in model.WorkflowItems)
        //    {
        //        dtoItem = dto.WorkflowItems.FirstOrDefault(cv => cv.WfItemId == item.WfItemId);
        //        if (dtoItem != null)
        //        {
        //            modelItem = new WorkflowItems();
        //            Mapper.Map<WorkflowItemsDTO, WorkflowItems>(dtoItem, modelItem);
        //            //Mapper.Map<WorkflowItemsDTO, WorkflowItems>(dtoItem, model);
        //            //item.EditUser = model.EditUser;
        //            modelItem.EditUser = model.EditUser;
        //            db.WorkflowItems.Add(modelItem);
        //        }
        //        else
        //        {
        //            listToRemove.Add(item);
        //        }
        //    }
        //    // remove items
        //    foreach (WorkflowItems item in model.WorkflowItems)  //listToRemove)
        //    {
        //        db.WorkflowItems.Remove(item);
        //        model.WorkflowItems.Remove(item);
        //    }
        //    // add new items
        //    foreach (WorkflowItemsDTO item in dto.WorkflowItems.Where(at => at.WfItemId == 0))
        //    {
        //        modelItem = AutoMapper.Mapper.Map<WorkflowItems>(item);
        //        modelItem.EditUser = model.EditUser;
        //        model.WorkflowItems.Add(modelItem);
        //    }
        //}        




        // Obtiene el siguiente item de workflow según el tipo
        static public WorkflowItems GetWfNextItem(WorkflowExecution wfExec, WfExecNextItem nextType = WfExecNextItem.Next)
        {
            SisConAxsContext db = new SisConAxsContext();

            Nullable<int> nextItemId = null;
            if (wfExec != null && wfExec.WorkflowItem != null)
            {
                WorkflowItemNext itemNext = wfExec.WorkflowItem.WorkflowItemNextParents.FirstOrDefault(t => t.WfItemNextType == (int)nextType);
                if (itemNext == null)
                {
                    return null;
                }

                nextItemId = itemNext.WfItemNextItemID;
                WorkflowItems nextItem = wfExec.Workflow.WorkflowItems.FirstOrDefault(i => i.WfItemId == nextItemId);
                return nextItem;
            }
            return null;
        }
        static public WorkflowItems SearchNextWorkflowItemByType(WorkflowExecution wfExec,
                                                                 string typeWfItem,
                                                                 WfExecNextItem firstNext = WfExecNextItem.Next)
        {
            WorkflowItems wfItem = GetWfNextItem(wfExec, firstNext);
            WorkflowItemNext wfItemNext = null;
            List<int> ItemsId = new List<int>();

            // recorre el workflow
            while (wfItem != null)
            {
                if (ItemsId.Contains(wfItem.WfItemId))  // si ya pasó por el item entonces sale del bucle
                {
                    wfItem = null;
                    break;
                }

                if (typeWfItem == wfItem.CommonValues.CommonValueName)
                {
                    break;
                }

                ItemsId.Add(wfItem.WfItemId);
                wfItemNext = wfItem.WorkflowItemNextParents.FirstOrDefault(t => t.WfItemNextType == (int)WfExecNextItem.Next);
                wfItem = null;
                if (wfItemNext != null)
                {
                    wfItem = wfExec.Workflow.WorkflowItems.FirstOrDefault(i => i.WfItemId == wfItemNext.WfItemNextItemID);
                }
            }
            return wfItem;
        }
    }
}
