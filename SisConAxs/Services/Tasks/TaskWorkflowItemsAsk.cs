using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using SisConAxs_DM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SisConAxs.Services
{
    // Envía una consulta al aprobador
    public class TaskWorkflowItemsAsk : AbstractTask
    {
        public TaskWorkflowItemsAsk(TaskManager manager) : base(manager)
        {
        }

        public bool Process(WorkflowExecution wfExec)
        {
            SisConAxsContext db = new SisConAxsContext();

            // VALIDACIONES ================================================================================================================= //
            // Si no tiene jerarquía de aprobacion ------------------------------------------------------------------------------------------
            WorkflowApproveHierarchy wfHierarchy = db.WorkflowApproveHierarchy.FirstOrDefault(x => x.WfApproveHierarchyID == wfExec.Workflow.WfApproveHierarchyID);
            AccessRequests request = db.AccessRequests.FirstOrDefault(r => r.RequestID == wfExec.WfExecParentObjectID);
            if (wfHierarchy == null)
            {
                wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado
                this.WFExecuteRepository.SaveWFExecutionEnd(
                    wfExec, WfExecStatus.FinishedRejected,
                    $"No se ejecutó la consulta porque no tiene jerarquía de aprobación."
                );
                return false;
            }

            // Si no tiene un aprobador -----------------------------------------------------------------------------------------------------
            Nullable<int> approverID = this.WFExecuteRepository.GetWfExecIntParam(wfExec.WfExecID, "approver");
            if (approverID == null)
            {
                wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado

                Nullable<int> approverOrder = this.WFExecuteRepository.GetWfExecIntParam(wfExec.WfExecID, "approver_order");
                WorkflowHierarchyMembers wfHMember = wfHierarchy.WorkflowHierarchyMembers.FirstOrDefault(m => m.WfHierarchyMemberOrder == (approverOrder ?? 1));
                if (wfHMember != null)
                {
                    string departementDisplay = "";
                    string positionDisplay = "";
                    CommonValues department = db.CommonValues.FirstOrDefault(c => c.CommonValueID == wfHMember.WfHierarchyMemberDepartment);
                    CommonValues position = db.CommonValues.FirstOrDefault(c => c.CommonValueID == wfHMember.WfHierarchyMemberPosition);

                    if (department != null)
                        departementDisplay = department.CommonValueDisplay;
                    if (position != null)
                        positionDisplay = position.CommonValueDisplay;

                    this.WFExecuteRepository.SaveWFExecutionEnd(
                        wfExec, WfExecStatus.FinishedRejected,
                        $"No se ejecutó la consulta porque no se encontró a un aprobador con área '{departementDisplay}' y cargo '{positionDisplay}'."
                    );
                    return false;
                }

                this.WFExecuteRepository.SaveWFExecutionEnd(
                    wfExec, WfExecStatus.FinishedRejected,
                    $"No se ejecutó la consulta porque no se encontró a un aprobador."
                );
                return false;
            }

            // Si no encuentra a la persona asignada como aprobador -------------------------------------------------------------------------
            People people = db.People.FirstOrDefault(p => p.PeopleID == approverID && p.PeopleStatus == 1);
            if (people == null)
            {
                wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado
                this.WFExecuteRepository.SaveWFExecutionEnd(
                    wfExec, WfExecStatus.FinishedRejected,
                    $"No se ejecutó la consulta porque no se encuentra a la persona asignada como aprobador."
                );
                LogManager.Error($"> TaskWorkflowItems.ProcessAskItem >> No se envió un Pedido de Aprobación, no existe un usuario con el id '{approverID}'. Solicitud ID: {wfExec.WfExecParentObjectID}. WFExec ID: {wfExec.WfExecID}");
                return false;
            }

            // Si la persona no tiene un correo válido ---------------------------------------------------------------------------------------
            if (String.IsNullOrEmpty(people.PeopleEmail))
            {
                wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado
                this.WFExecuteRepository.SaveWFExecutionEnd(
                    wfExec, WfExecStatus.FinishedRejected,
                    String.Format("No se ejecutó la consulta porque la persona '{0}' no tiene un correo válido.", people.GetFullName())
                );
                LogManager.Error($"> TaskWorkflowItems.ProcessAskItem >> No se envió un Pedido de Aprobación, la persona '{people.GetFullName()}' no tiene un correo válido." +
                                  $" Solicitud ID: {wfExec.WfExecParentObjectID}. WFExec ID: {wfExec.WfExecID}");
                return false;
            }

            // Si la persona no tiene un registro de usuario (No podría aprobar la solicitud) -------------------------------------------------
            AccessUsers user = db.AccessUsers.FirstOrDefault(p => p.UserInternalID == people.UserID);
            if (user == null)
            {
                wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado
                this.WFExecuteRepository.SaveWFExecutionEnd(
                    wfExec, WfExecStatus.FinishedRejected,
                    $"No se ejecutó la consulta porque la persona '{people.GetFullName()}' no está registrado como usuario del sistema."
                );
                LogManager.Error($"> TaskWorkflowItems.ProcessAskItem >> No se envió un Pedido de Aprobación, la persona '{people.GetFullName()}' no tiene un registro en la tabla Usuarios." +
                                  $" Solicitud ID: {wfExec.WfExecParentObjectID}. WFExec ID: {wfExec.WfExecID}");
                return false;
            }
            // VALIDACIONES ================================================================================================================= //


            // Graba el registro de consultas -----------------------------------------------------------------------------------------------
            //this.Manager.EmailDataStorage.PrepareEmailData(wfExec);  // Prepara la data del correo, este va agrupando las consultas en un solo correo si tienen la misma Solicitud, Workflow y WorkflowStep
            RequestEmailDataStorage.PrepareEmailData(wfExec, "",
                                                     "",
                                                     false, 
                                                     this.GetEmailCc(wfExec,request,db)); //Modificado el 20191004 <-- DIANA CAMUS

            string HierarchyMemberDescription = "";
            WorkflowHierarchyMembers hierarchyMember = wfExec.Workflow.WorkflowApproveHierarchy.WorkflowHierarchyMembers.FirstOrDefault(
                x => x.WfHierarchyMemberOrder == this.WFExecuteRepository.GetWfExecIntParam(wfExec.WfExecID, "approver_order")
            );
            if (hierarchyMember != null && !String.IsNullOrWhiteSpace(hierarchyMember.WfHierarchyMemberDescription))
            {
                HierarchyMemberDescription = " - " + hierarchyMember.WfHierarchyMemberDescription;
            }

            //graba el workflow execute con un nuevo estado
            this.WFExecuteRepository.SaveWFExecution(
                wfExec, wfExec.WfExecCurrentStep, WfExecStatus.WaitingResponse,
                $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> Se envió un Pedido de Aprobación a <strong>[{people.GetFullName()} - {people.PeopleEmail}]</strong> {HierarchyMemberDescription}."
            );
            return true;
        }

        //INICIO Adicionado el 20191004 <-- DIANA CAMUS
        public string GetEmailCc(WorkflowExecution wfExec, AccessRequests request, SisConAxsContext db)
        {
            string destEmailCc = string.Empty;
            // Con Copia ----------------------------------------------------------------------------------------------------- 
            if (wfExec.WorkflowItem.WfItemCcType.HasValue && wfExec.WorkflowItem.WfItemCcType.Value > 0)
            {
                People peopleCc = null;
                switch (wfExec.WorkflowItem.WfItemCcType)
                {
                    case (int)WfItemDestType.Aprobador:
                        Nullable<int> approverID = this.WFExecuteRepository.GetWfExecIntParam(wfExec.WfExecID, "approver");
                        peopleCc = db.People.FirstOrDefault(p => p.PeopleID == approverID);
                        if (peopleCc != null)
                        {
                           destEmailCc = peopleCc.PeopleEmail;
                        }
                        break;
                    case (int)WfItemDestType.Solicitante:
                        peopleCc = db.People.FirstOrDefault(p => p.UserID == request.RequestBy);
                        if (peopleCc != null)
                        {
                            destEmailCc = peopleCc.PeopleEmail;
                        }
                        break;
                    case (int)WfItemDestType.SolicitadoPara:
                        peopleCc = db.People.FirstOrDefault(p => p.PeopleID == request.RequestTo);
                        if (peopleCc != null)
                        {
                            destEmailCc = peopleCc.PeopleEmail;
                        }
                        break;
                    case (int)WfItemDestType.Otro:
                        destEmailCc = wfExec.WorkflowItem.WfItemCcMail;
                        break;
                    case (int)WfItemDestType.Ejecutor:
                        destEmailCc = wfExec.WorkflowItem.WfItemDestMail;
                        //sendAccountMailer = true;
                        break;
                }
            }
            return destEmailCc;
        }
        //FIN Adicionado el 20191004 <-- DIANA CAMUS

        public override void Execute(object state)
        {
            throw new NotImplementedException();
        }
    }
}