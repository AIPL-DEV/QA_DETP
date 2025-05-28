using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DETP
{
    public class State
    {
        public static readonly string CRITICAL_JOB_STOPPED_DEPT_HOD_FROM_START = "CRITICAL_JOB_STOPPED_DEPT_HOD_FROM_START";
        public static readonly string CRITICAL_JOB_STOPPED_ASSIGNEE_FROM_HOD_DEPT = "CRITICAL_JOB_STOPPED_ASSIGNEE_FROM_HOD_DEPT";
        public static readonly string CRITICAL_JOB_STOPPED_DEPT_HOD_FROM_ASSIGNEE = "CRITICAL_JOB_STOPPED_DEPT_HOD_FROM_ASSIGNEE";
        public static readonly string CRITICAL_JOB_STOPPED_HEAD_DETP_FROM_DEPT_HOD = "CRITICAL_JOB_STOPPED_HEAD_DETP_FROM_HEAD_DETP";
        public static readonly string CRITICAL_JOB_STOPPED_HOD_QA_SHA_FROM_HEAD_DETP = "CRITICAL_JOB_STOPPED_EIC_DETP_FROM_HEAD_DETP";
    }
}
