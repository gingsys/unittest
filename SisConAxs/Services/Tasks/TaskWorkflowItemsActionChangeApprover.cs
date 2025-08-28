using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SisConAxs.Services.Tasks
{
    public class TaskWorkflowItemsActionChangeApprover : AbstractTask
    {
        public TaskWorkflowItemsActionChangeApprover(TaskManager manager) : base(manager)
        {
        }

        // Procesa la acción
        public bool Process(WorkflowExecution wfExec)
        {
            string errorMessage = "";
            SisConAxsContext db = new SisConAxsContext();
            WorkflowItems nextItem = null;
            Nullable<int> currentOrder = this.WFExecuteRepository.GetWfExecIntParam(wfExec.WfExecID, "approver_order");

            AccessRequests request = db.AccessRequests.FirstOrDefault(t => t.RequestID == wfExec.WfExecParentObjectID);
            WorkflowApproveHierarchy wfApproveHierarchy = wfExec.Workflow.WorkflowApproveHierarchy;
            int? workflowHierarchyID = wfExec.Workflow.WfApproveHierarchyID;
            string workflowHierarchyname = wfExec.Workflow.WorkflowApproveHierarchy.WfApproveHierarchyName;
            WorkflowItems wfItem = wfExec.WorkflowItem;

            // Realiza la busqueda de la jerarquía si es la jerarquía del área de la persona ---------------------------------------------
            if (workflowHierarchyID == -1)  // Si la jerarquía es del área de la persona
            {
                // Si la persona a la que se le pide el permiso no tiene área
                People people = db.People.FirstOrDefault(t => t.PeopleID == request.RequestTo);
                if (people.PeopleDepartment == null || people.PeopleDepartment == 0)
                {
                    errorMessage = "No se encontró a la jerarquía para el área porque la persona no pertenece a alguna área.";
                    LogManager.Error($"> TaskWorkflowItems.ProcessActionItem >> {errorMessage}.\nSolicitud ID: {wfExec.WfExecParentObjectID}. WFExec ID: {wfExec.WfExecID}");

                    wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado
                    this.WFExecuteRepository.SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedRejected, errorMessage);  // termina el workflow como rechazado
                    return false;
                }

                // Si no existe una jerarquía de aprobación que tenga el área de la persona
                wfApproveHierarchy = db.WorkflowApproveHierarchy.FirstOrDefault(t => t.WfApproveHierarchyDepartment == people.Department.CommonValueID);
                if (wfApproveHierarchy == null)
                {
                    errorMessage = $"No se encontró a la jerarquía para el área [{people.Department.CommonValueDisplay}]";
                    LogManager.Error($"> TaskWorkflowItems.ProcessActionItem >> {errorMessage}.\nSolicitud ID: {wfExec.WfExecParentObjectID}. WFExec ID: {wfExec.WfExecID}");

                    wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado
                    this.WFExecuteRepository.SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedRejected, errorMessage);  // termina el workflow como rechazado
                    return false;
                }

                workflowHierarchyID = wfApproveHierarchy.WfApproveHierarchyID;
                workflowHierarchyname = wfApproveHierarchy.WfApproveHierarchyName;
            }

            // JONATAN LOBATO P.
            // 2016-02-01 se encontro que no se respeta el orden de la jerarquia, se procede a implementar Correccion y se comenta la siguiente linea
            ////WorkflowHierarchyMembers wfHMember = wfApproveHierarchy.WorkflowHierarchyMembers.FirstOrDefault(m => m.WfHierarchyMemberOrder > currentOrder );

            //Si la solicitud fue aprobada pasa al siguinente aprobador
            if (wfExec.WfResponse == (int)WfExecResponse.Approved
                //|| wfExec.WfResponse == (int)WfExecResponse.Timeout
                || wfExec.WfResponse == (int)WfExecResponse.Pending)
            {
                //en la accion dar la opcion para seleccionar la responsabiliodad o siguiente aprobador                 
                if (wfItem.WfItemActionValue == 1)
                {
                    // Si fue aprobado, salta al Siguiente Aprobador
                    currentOrder += 2;
                    // Si el siguiente aprobador no es par (Aprobador Secundario) se retorna al Aprobador Primario
                    // Esto sucede cuando en que aprobó fue resultado de un timeout (segundo aprobador)
                    if (currentOrder % 2 == 0)
                    { currentOrder--; }
                }
                else
                { currentOrder = wfItem.WfItemActionValue; }
            }

            ICollection<WorkflowHierarchyMembers> lstwfHMember = wfApproveHierarchy.WorkflowHierarchyMembers;
            WorkflowHierarchyMembers wfHMember = (from t in lstwfHMember
                                                  where t.WfHierarchyMemberOrder == currentOrder
                                                  orderby t.WfHierarchyMemberOrder ascending
                                                  select t).FirstOrDefault();
            // JONATAN LOBATO P. 

            // Validación: Si no encuentra al siguiente miembro en la jerarquia ----------------------------------------
            if (wfHMember == null)
            {
                if (currentOrder > 0)  // si ya ha terminado con la lista de aprobadores
                {
                    nextItem = WorkflowRepository.GetWfNextItem(wfExec);
                    if (nextItem == null)
                    {
                        this.WFExecuteRepository.SaveWFExecutionEnd(wfExec);
                    }
                    else  // manda al siguiente wfItem con respuesta rechazado
                    {
                        // se actualiza el ultimo aprobador a ninguno, ya que no quedan mas en la jerarquía
                        this.WFExecuteRepository.SaveWFExecParam(wfExec.WfExecID, "approver", 0, "", DateTime.Now);
                        this.WFExecuteRepository.SaveWFExecParam(wfExec.WfExecID, "approver_order", -1, "", DateTime.Now);

                        wfExec.WfResponse = (int)WfExecResponse.Rejected;
                        this.WFExecuteRepository.SaveWFExecution(
                            wfExec, nextItem.WfItemId, WfExecStatus.Responded,
                            $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> No hay más aprobadores en la jerarquía [{wfApproveHierarchy.WfApproveHierarchyName}]." //, la respuesta se establece como <strong><span style='background-color:#E77272'>RECHAZADO</span></strong>.",
                        );
                    }
                    return false;
                }

                wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado
                this.WFExecuteRepository.SaveWFExecutionEnd(       // termina el workflow como rechazado
                    wfExec, WfExecStatus.FinishedRejected,
                    $"No hay un miembro más para la jerarquía [{wfApproveHierarchy.WfApproveHierarchyName}]."
                );
                LogManager.Error($"> TaskWorkflowItems.ProcessActionItem >> No hay un miembro más para la jerarquía [{wfApproveHierarchy.WfApproveHierarchyName}].\nSolicitud ID: {wfExec.WfExecParentObjectID}. WFExec ID: {wfExec.WfExecID}");
                return false;
            }

            // Validacion: Si no encuentra aprobador ------------------------------------------------------------------------------
            People wfApprover = db.People.FirstOrDefault(p => p.PeopleDepartment == wfHMember.WfHierarchyMemberDepartment
                                                              && p.PeoplePosition == wfHMember.WfHierarchyMemberPosition
                                                              && p.PeopleStatus == 1); // VALIDA QUE EL APROBADOR ESTE ACTIVO 2017-04-12
            if (wfApprover == null)
            {
                string departementDisplay = "";
                string positionDisplay = "";
                CommonValues department = db.CommonValues.FirstOrDefault(c => c.CommonValueID == wfHMember.WfHierarchyMemberDepartment);
                CommonValues position = db.CommonValues.FirstOrDefault(c => c.CommonValueID == wfHMember.WfHierarchyMemberPosition);

                if (department != null)
                    departementDisplay = department.CommonValueDisplay;
                if (position != null)
                    positionDisplay = position.CommonValueDisplay;

                wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado
                this.WFExecuteRepository.SaveWFExecutionEnd(       // termina el workflow como rechazado
                    wfExec, WfExecStatus.FinishedRejected,
                    $"No se encontró a un aprobador con área '{departementDisplay}' y cargo '{positionDisplay}' para la jerarquía [{wfApproveHierarchy.WfApproveHierarchyName}]."
                );
                LogManager.Error($"> TaskWorkflowItems.ProcessActionItem >> No se encontró a un aprobador con área '{departementDisplay}' y cargo '{positionDisplay}' para la jerarquía [{wfApproveHierarchy.WfApproveHierarchyName}].\n" +
                                  $" Solicitud ID: {wfExec.WfExecParentObjectID}. WFExec ID: {wfExec.WfExecID}"
                );
                return false;
            }

            // Guarda los cambios --------------------------------------------------------------------------------
            string peopleFullName = wfApprover.GetFullName();
            this.WFExecuteRepository.SaveWFExecParam(wfExec.WfExecID, "approver", wfApprover.PeopleID, peopleFullName, DateTime.Now);
            this.WFExecuteRepository.SaveWFExecParam(wfExec.WfExecID, "approver_order", wfHMember.WfHierarchyMemberOrder, "", DateTime.Now);

            nextItem = WorkflowRepository.GetWfNextItem(wfExec);
            if (nextItem == null)
            {
                this.WFExecuteRepository.SaveWFExecutionEnd(wfExec);
            }
            else
            {
                string HierarchyMemberDescription = "";
                if (wfHMember != null && !String.IsNullOrWhiteSpace(wfHMember.WfHierarchyMemberDescription))
                {
                    HierarchyMemberDescription = " - " + wfHMember.WfHierarchyMemberDescription;
                }

                //grabar el workflow_execute con el nuevo next item
                this.WFExecuteRepository.SaveWFExecution(
                    wfExec, nextItem.WfItemId, WfExecStatus.ToProcess,
                    $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> Se estableció el Aprobador a <strong>[{peopleFullName}]</strong> {HierarchyMemberDescription}"
                );
            }
            return true;
        }

        public override void Execute(object state)
        {
            throw new NotImplementedException();
        }
    }
}