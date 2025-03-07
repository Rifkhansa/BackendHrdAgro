using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Database.MySql.Support;
using BackendHrdAgro.Models.Employee;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHrdAgro.Models.Attendance
{
    public class AbsenteeModel
    {
        public List<AttendanceRecordQuery> attendanceRecordQueries(string filter)
        {
            var sql = "select  a.employee_id, a.employee_first_name,  a.employee_last_name," +
                "b.department_name,title_name,c.is_cut_absentee,c.is_overtime,\r\n      " +
                "case when c.is_cut_absentee =1 then 'YES' else 'NO' end absent_status,\r\n      " +
                "case when c.is_overtime =1 then 'YES' else 'NO' end overtime_status\r\n      " +
                "from tm_employee_affair a\r\n      inner join tm_department b on a.department_id = b.department_id\r\n      " +
                "inner join tm_title c on a.title_id = c.title_id\r\n      " +
                $"where a.status not in ('5','7','9') and  a.department_id not in ('DP001','DP009') {filter}  order by employee_first_name\t ASC";
            var list = new DatabaseContext().AttendanceRecordQueries.FromSqlRaw(sql).ToList();
            return list;
        }

        public List<Departments> departments(string filter)
        {
            var sql = "select department_id,department_name\r\n\t\tfrom tm_department b " +
                $"where b.status in (1) {filter}  order by department_name ASC";
            var department = new DatabaseContext().Departments.FromSqlRaw(sql).ToList();
            return department;
        }

        public List<PeriodsQuery> periodsQueries()
        {
            var sql = "select periode_id,\r\nCONCAT(CAST(MONTHNAME(STR_TO_DATE(a.month, '%m')) " +
                "AS CHAR CHARACTER SET utf8),' ' ,CAST(year AS CHAR CHARACTER SET utf8))\r\n " +
                "as periode_name from  tp_absentee_periode a  where a.status in (1,99) order by periode_id ASC";
            var periode = new DatabaseContext().PeriodsQueries.FromSqlRaw(sql).ToList();
            return periode;
        }

        public List<FindIsOB> FindIsOB(string employeeId)
        {
            string sql = $"SELECT is_office_boy FROM tm_employee_affair where employee_id = '{employeeId}' ";
            var list = new DatabaseContext().FindIsOBs.FromSqlRaw(sql).ToList();
            return list;
        }

        public List<AttendanceEmployeeQuery> AbsenteeEmployee(string clausa, string clausb, string myEmployeeId, string myPeriodeId, string myPeriodeId2)
        {
            string sql = "";

            sql = $"select DATE_FORMAT(b.absent_date,'%d-%m-%Y') as absent_date, DATE_FORMAT(b.absent_date,'%Y-%m-%d') as absent_date_db, " +
            $"DATE_FORMAT(b.absent_date, '%M %Y') as periode,DATE_FORMAT(b.absent_date, '%d') as my_date, DATE_FORMAT(b.absent_date, '%Y%m') as periode_link, " +
            $"a.employee_id, a.employee_first_name, a.employee_last_name,department_name,title_name, " +
            $"b.time_in, b.time_out,d.is_cut_absentee as is_cut_absentee_des,d.is_overtime as is_overtime_des, a.title_id, a.department_id, " +
            $"e.category,e.type_revisi,e.reason,e.file_name,e.size,e.status,1 as is_absent, " +
            $"'0.00' as my_hour,'0.00' as my_hour_efektif,'N' as is_overtime, '0.00' as overtime,' ' as status_potong_cuti,' ' as cut_off_absent_des,' ' as link_modal_cut_off " +
            $"from tm_employee_affair a " +
            $"left outer join tp_absent b on a.employee_id = b.employee_id " +
            $"inner join tm_department c on a.department_id = c.department_id " +
            $"inner join tm_title d on a.title_id = d.title_id " +
            $"left outer join tp_absent_revisi e on b.absent_date = e.absent_date and a.employee_id = e.employee_id " +
            $"where a.status = 1 {clausa}  " +
            $"union all " +
            $"select DATE_FORMAT(b.date_field,'%d-%m-%Y') as absent_date,DATE_FORMAT(b.date_field, '%Y-%m-%d') as absent_date_db, " +
            $"DATE_FORMAT(b.date_field, '%M %Y') as periode,DATE_FORMAT(b.date_field, '%d') as my_date, DATE_FORMAT(b.date_field, '%Y%m') as periode_link, " +
            $"a.employee_id, a.employee_first_name, a.employee_last_name,department_name,title_name, " +
            $"'00:00' as time_in, '00:00' as time_out, " +
            $"d.is_cut_absentee as is_cut_absentee_des,d.is_overtime as is_overtime_des, a.title_id, a.department_id, " +
            $"e.category,e.type_revisi,e.reason,e.file_name,e.size,e.status,0 as is_absent, " +
            $"'0.00' as my_hour,'0.00' as my_hour_efektif,'N' as is_overtime,'0.00' as overtime,' ' as status_potong_cuti,' ' as cut_off_absent_des,' ' as link_modal_cut_off " +
            $"from tm_employee_affair a " +
            $"inner " +
                $"join ( " +
                $"SELECT employee_id, " +
                $"cal.my_date AS date_field, DATE_FORMAT(cal.my_date, '%d %b %Y') as date_field_cap " +
                $"FROM " +
                    $"(SELECT CAST('{myEmployeeId}' AS CHAR CHARACTER SET utf8) as employee_id, " +
                    $"s.start_date + INTERVAL(tm_days.d) DAY  AS my_date " +
                    $"FROM " +
                        $"(SELECT LAST_DAY('{myPeriodeId2}') + INTERVAL 1 DAY - INTERVAL 1 MONTH AS start_date, LAST_DAY('{myPeriodeId2}') AS end_date " +
                        $") AS s " +
                        $"JOIN tm_days ON  tm_days.d <= DATEDIFF(s.end_date, s.start_date) " +
                    $") AS cal " +
                    $"where DAYOFWEEK(my_date) not in (1, 7) and concat(my_date, employee_id) not in (select concat(absent_date, employee_id) from tp_absent a " +
                    $"where DATE_FORMAT(a.absent_date, '%Y%m') = '{myPeriodeId}') " +
                $") b on a.employee_id = b.employee_id " +
                $"inner join tm_department c on a.department_id = c.department_id " +
                $"inner join tm_title d on a.title_id = d.title_id " +
                $"left outer join tp_absent_revisi e on b.date_field = e.absent_date and a.employee_id = e.employee_id " +
                $"where a.status = 1  {clausb} ";


            var employee = new DatabaseContext().AttendanceEmployeeQueries.FromSqlRaw(sql).ToList();
            return employee;
        }

        public List<UnAbsenteeEmployeeQuery> UnAbsenteeEmployee(string clausa, int isOB, string myEmployeeId, string myYear, string myMonth, string myPeriodeId2)
        {
            string sql = "";
            string notIn = "(1,7)";
            if (isOB == 1)
            {
                notIn = "(1)";
            }
            Console.WriteLine("isOB: " + isOB);
            sql = $"SELECT a.my_date as absent_date, DATE_FORMAT(a.my_date, '%d %M %Y') as absent_date_cap, " +
                $"    COALESCE(b.description, c.description, d.description, e.description, f.description, '') AS description," +
                $"    COALESCE(b.status, c.status, d.status, e.status, f.status, '') AS status " +
                $"    FROM (" +
                $"           SELECT  DATE(CONCAT('{myPeriodeId2}-', LPAD(n, 2, '0'))) AS my_date, e.employee_id " +
                $"           FROM " +
                $"               (SELECT n FROM numbers WHERE n <= DAY(LAST_DAY('{myPeriodeId2}-01'))) a " +
                $"                   CROSS JOIN " +
                $"                   (SELECT employee_id FROM tm_employee_affair WHERE status=1 AND employee_id='{myEmployeeId}') e " +
                $"                       WHERE  " +
                $"                       CONCAT((CONCAT('{myPeriodeId2}-', LPAD(n, 2, '0'))),employee_id) NOT IN (SELECT DISTINCT concat(absent_date,employee_id) FROM tp_absent WHERE is_wfh=0 AND employee_id='{myEmployeeId}'  and year(absent_date)='{myYear}' and MONTH(absent_date)='{myMonth}') " +
                $"                       AND DAYOFWEEK(DATE(CONCAT('{myPeriodeId2}-', LPAD(n, 2, '0')))) NOT IN {notIn} " + //jika ob maka kecualikan minggu doang kalo bukan sabtu dan minggu(1,7)
                $"                ) a " +
                $"/** join ke izin **/" +
                $"     LEFT JOIN (" +
                $"     SELECT  b.employee_id," +
                $"        a.permission_date AS my_date, " +
                $"        CONCAT(c.permission_type, '-', b.reason) AS description, " +
                $"        CASE " +
                $"            WHEN b.status = 1 THEN 'Approved' " +
                $"            ELSE 'Waiting Approve' " +
                $"        END AS status " +
                $"     FROM tp_permission_detail a " +
                $"       INNER JOIN tp_permission b ON a.permission_id = b.permission_id " +
                $"       INNER JOIN tm_permission_type c ON b.permission_type_id = c.permission_type_id " +
                $"       WHERE b.deleted_at IS NULL " +
                $"     ) b ON a.my_date = b.my_date AND a.employee_id = b.employee_id " +
                $"/** join ke holiday hari besar **/ " +
                $"       LEFT JOIN (" +
                $"       SELECT " +
                $"           holiday_date AS my_date, " +
                $"           holiday_name AS description, " +
                $"           'Hari Besar' AS status " +
                $"       FROM tm_holiday_date " +
                $"       ) c ON a.my_date = c.my_date  " +
                $"/** join ke cuti **/" +
                $"       LEFT JOIN (" +
                $"       SELECT " +
                $"           b.employee_id, " +
                $"           a.cuti_date AS my_date, " +
                $"           CONCAT('Cuti ', c.nama_cuti, '-', b.keperluan) AS description, " +
                $"           CASE " +
                $"               WHEN b.status = 1 THEN 'Approved' " +
                $"            ELSE 'Waiting Approve' " +
                $"               END AS status " +
                $"       FROM tp_detail_cuti a " +
                $"           INNER JOIN tp_cuti b ON a.cuti_id = b.cuti_id " +
                $"           INNER JOIN tm_type_cuti c ON b.type_cuti_id = c.type_cuti_id     WHERE b.deleted_at IS NULL " +
                $"       ) d ON a.my_date = d.my_date AND a.employee_id = d.employee_id " +
                $"/** join wfh **/ " +
                $"       LEFT JOIN (" +
                $"        SELECT " +
                $"           employee_id," +
                $"           request_date AS my_date, " +
                $"           CONCAT('WFH-', reason) AS description, " +
                $"           CASE " +
                $"               WHEN status = 1 THEN 'Approved' " +
                $"               ELSE 'Waiting Approve' " +
                $"           END AS status " +
                $"       FROM tp_request_wfh " +
                $"           WHERE status NOT IN (-1) " +
                $"       ) e ON a.my_date = e.my_date AND a.employee_id = e.employee_id " +
                $"/** join no card **/ " +
                $"       LEFT JOIN (" +
                $"        SELECT " +
                $"           employee_id," +
                $"           absent_date AS my_date, " +
                $"           'No Card (No NameTag)' AS description, " +
                $"           ' ' AS status " +
                $"       FROM tp_absent_no_card " +
                $"       ) f ON a.my_date = f.my_date AND a.employee_id = f.employee_id " +
                $";";

            var employee = new DatabaseContext().UnAbsenteeEmployeeQueries.FromSqlRaw(sql).ToList();
            return employee;
        }

        public List<ComeLateQuery> ComeLateEmployee(int isOB, string departmentId, string myEmployeeId, string myYear, string myMonth)
        {
            string sql = "";
            string filterDept = "";
            string filterEmployee = "";

            string notIn = "DAYOFWEEK(a.absent_date) not in (1,7)"; // Hari Minggu (1) dan Sabtu (7) //harusnya ada kondisi juga untuk dept all
            string timeCondition = "";

            if (departmentId == null || departmentId == "all" || departmentId == "none")
            {
                notIn = "CASE WHEN e.is_office_boy = 1 THEN DAYOFWEEK(a.absent_date) not in (1) ELSE DAYOFWEEK(a.absent_date) not in (1,7) END";
                timeCondition = "CASE WHEN e.is_office_boy = 1 THEN CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_in > '09:15:00' ELSE a.time_in > '07:45:00' END WHEN e.is_office_boy = 0 THEN a.time_in > '09:00:00' END ";
            }
            else
            {
                filterDept = $" e.department_id='{departmentId}' AND ";

                notIn = "CASE WHEN e.is_office_boy = 1 THEN DAYOFWEEK(a.absent_date) not in (1) ELSE DAYOFWEEK(a.absent_date) not in (1,7) END";
                timeCondition = "CASE WHEN e.is_office_boy = 1 THEN CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_in > '09:15:00' ELSE a.time_in > '07:45:00' END WHEN e.is_office_boy = 0 THEN a.time_in > '09:00:00' END ";
            }
            if (myEmployeeId == null || myEmployeeId == "all" || myEmployeeId == "none")
            {

            }
            else
            {
                filterEmployee = $" AND a.employee_id='{myEmployeeId}' ";

                if (isOB == 1)
                {
                    // Jika OB, atur waktu terlambat Senin-Jumat 07:45 dan Sabtu 09:15
                    timeCondition = "CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_in > '09:15:00' ELSE a.time_in > '07:45:00' END";
                    notIn = "DAYOFWEEK(a.absent_date) not in (1)"; // Hanya hari Minggu yang libur
                }
                else
                {
                    // Jika bukan OB, atur waktu terlambat Senin-Jumat 09:00
                    timeCondition = "a.time_in > '09:00:00'";
                }
            }



            sql = $"select a.employee_id,e.department_id,a.absent_date,a.time_in,a.time_out,  DATE_FORMAT(a.absent_date, '%d %M %Y') as absent_date_cap," +
                $"COALESCE(b.description, c.description, d.description, '') AS description, " +
                $"COALESCE(b.status, c.status, d.status, '') AS status, " +
                $"concat(e.employee_first_name, ' ', e.employee_last_name) as employee_name, " +
                $"f.department_name " +
                $"from tp_absent a " +
                $"LEFT JOIN ( " +
                $"      SELECT  b.employee_id, a.permission_date AS my_date,  " +
                $"      CONCAT(c.permission_type, '-', b.reason) AS description,  " +
                $"      CASE WHEN b.status = 1 THEN 'Approved'  ELSE 'Waiting Approve'  END AS status  " +
                $"      FROM tp_permission_detail a  " +
                $"          INNER JOIN tp_permission b ON a.permission_id = b.permission_id  " +
                $"          INNER JOIN tm_permission_type c ON b.permission_type_id = c.permission_type_id  " +
                $"          WHERE b.deleted_at IS NULL  " +
                $"      ) b ON a.absent_date = b.my_date AND a.employee_id = b.employee_id  " +
                $"LEFT JOIN ( " +
                $"      SELECT b.employee_id,a.cuti_date AS my_date, CONCAT('Cuti ', c.nama_cuti, '-', b.keperluan, ' (', b.jumlah_cuti, 'hari' ,')') AS description,  " +
                $"      CASE WHEN b.status = 1 THEN 'Approved'  ELSE 'Waiting Approve'  END AS status  " +
                $"      FROM tp_detail_cuti a  " +
                $"          INNER JOIN tp_cuti b ON a.cuti_id = b.cuti_id " +
                $"          INNER JOIN tm_type_cuti c ON b.type_cuti_id = c.type_cuti_id     WHERE b.deleted_at IS NULL " +
                $"    ) c ON a.absent_date = c.my_date AND a.employee_id = c.employee_id " +
                $"LEFT JOIN ( " +
                $"      SELECT  employee_id, absent_date AS my_date, 'No Card (No NameTag)' AS description,  ' ' AS status  " +
                $"      FROM tp_absent_no_card  " +
                $"    ) d ON a.absent_date = d.my_date AND a.employee_id = d.employee_id  " +
                $"INNER JOIN tm_employee_affair e on a.employee_id=e.employee_id " +
                $"INNER JOIN tm_department f on e.department_id = f.department_id  " +
                $"where {filterDept} YEAR(a.absent_date)='{myYear}' and MONTH(a.absent_date)='{myMonth}' {filterEmployee} " +
                $"AND e.title_id in ('DS006','DS004','DS003') and  {timeCondition} and  {notIn} and a.absent_date not in (select holiday_date from tm_holiday_date ) " +
                $"order by f.department_name,a.employee_id,a.absent_date asc ";

            var employee = new DatabaseContext().ComeLateQueries.FromSqlRaw(sql).ToList();
            return employee;
        }

        public List<ComeLateQuery> IncompleteAttendance(int isOB, string departmentId, string myEmployeeId, string myYear, string myMonth)
        {
            string sql = "";
            string filterDept = "";
            string filterEmployee = "";

            string notIn = "DAYOFWEEK(a.absent_date) not in (1,7)"; // Hari Minggu (1) dan Sabtu (7) //harusnya ada kondisi juga untuk dept all
            string timeCondition = "";

            if (departmentId == null || departmentId == "all" || departmentId == "none")
            {
                notIn = "CASE WHEN e.is_office_boy = 1 THEN DAYOFWEEK(a.absent_date) not in (1) ELSE DAYOFWEEK(a.absent_date) not in (1,7) END";

                timeCondition = "CASE WHEN e.is_office_boy = 1 THEN CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_out < '12:00:00' ELSE a.time_out < '18:00:00' END WHEN e.is_office_boy = 0 THEN CASE WHEN a.time_in  <= '08:00:00' THEN time_out < '17:00:00' ELSE CASE WHEN a.time_in <= '09:00:00' THEN TIMEDIFF(a.time_out, a.time_in) < '09:00:00' END END  ELSE 0 END";
            }
            else
            {
                filterDept = $" e.department_id='{departmentId}' AND ";

                notIn = "CASE WHEN e.is_office_boy = 1 THEN DAYOFWEEK(a.absent_date) not in (1) ELSE DAYOFWEEK(a.absent_date) not in (1,7) END";
                /*      timeCondition = "CASE WHEN e.is_office_boy = 1 THEN CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_out < '12:00:00' ELSE a.time_out < '18:00:00' END WHEN e.is_office_boy = 0 THEN TIMEDIFF(a.time_out, a.time_in) < '09:00:00' ELSE 0 END";*/

                timeCondition = "CASE WHEN e.is_office_boy = 1 THEN CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_out < '12:00:00' ELSE a.time_out < '18:00:00' END WHEN e.is_office_boy = 0 THEN CASE WHEN a.time_in  <= '08:00:00' THEN time_out < '17:00:00' ELSE CASE WHEN a.time_in <= '09:00:00' THEN TIMEDIFF(a.time_out, a.time_in) < '09:00:00' END END  ELSE 0 END";
            }
            if (myEmployeeId == null || myEmployeeId == "all" || myEmployeeId == "none")
            {

            }
            else
            {
                filterEmployee = $" AND a.employee_id='{myEmployeeId}' ";

                if (isOB == 1)
                {
                    // Jika OB, atur waktu terlambat Senin-Jumat 07:45 dan Sabtu 09:15
                    timeCondition = "CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_out < '12:00:00' ELSE a.time_out < '18:00:00' END";
                    notIn = "DAYOFWEEK(a.absent_date) not in (1)"; // Hanya hari Minggu yang libur
                }
                else
                {
                    // Jika bukan OB, atur waktu terlambat Senin-Jumat 09:00
                    //timeCondition = "CASE WHEN a.time_in <= '08:00:00' THEN a.time_out < '17:00:00' ELSE a.time_out < '18:00:00' END";
                    timeCondition = "CASE WHEN a.time_in <= '08:00:00' THEN time_out < '17:00:00' ELSE CASE WHEN a.time_in <= '09:00:00' THEN TIMEDIFF(a.time_out, a.time_in) < '09:00:00' END END ";
                }
            }

            var conditionEmployee = "";

            sql = $"select a.employee_id,e.department_id,a.absent_date,a.time_in,a.time_out,  DATE_FORMAT(a.absent_date, '%d %M %Y') as absent_date_cap," +
                $"COALESCE(b.description, c.description, d.description, '') AS description, " +
                $"COALESCE(b.status, c.status, d.status, '') AS status, " +
                $"concat(e.employee_first_name, ' ', e.employee_last_name) as employee_name, " +
                $"f.department_name " +
                $"from tp_absent a " +
                $"LEFT JOIN ( " +
                $"      SELECT  b.employee_id, a.permission_date AS my_date,  " +
                $"      CONCAT(c.permission_type, '-', b.reason) AS description,  " +
                $"      CASE WHEN b.status = 1 THEN 'Approved'  ELSE 'Waiting Approve'  END AS status  " +
                $"      FROM tp_permission_detail a  " +
                $"          INNER JOIN tp_permission b ON a.permission_id = b.permission_id  " +
                $"          INNER JOIN tm_permission_type c ON b.permission_type_id = c.permission_type_id  " +
                $"          WHERE b.deleted_at IS NULL  " +
                $"      ) b ON a.absent_date = b.my_date AND a.employee_id = b.employee_id  " +
                $"LEFT JOIN ( " +
                  $"      SELECT b.employee_id,a.cuti_date AS my_date, CONCAT('Cuti ', c.nama_cuti, '-', b.keperluan, ' (', b.jumlah_cuti, 'hari' ,')') AS description,  " +
                $"      CASE WHEN b.status = 1 THEN 'Approved'  ELSE 'Waiting Approve'  END AS status  " +
                $"      FROM tp_detail_cuti a  " +
                $"          INNER JOIN tp_cuti b ON a.cuti_id = b.cuti_id " +
                $"          INNER JOIN tm_type_cuti c ON b.type_cuti_id = c.type_cuti_id     WHERE b.deleted_at IS NULL " +
                $"    ) c ON a.absent_date = c.my_date AND a.employee_id = c.employee_id " +
                $"LEFT JOIN ( " +
                $"      SELECT  employee_id, absent_date AS my_date, 'No Card (No NameTag)' AS description,  ' ' AS status  " +
                $"      FROM tp_absent_no_card  " +
                $"    ) d ON a.absent_date = d.my_date AND a.employee_id = d.employee_id  " +
                $"INNER JOIN tm_employee_affair e on a.employee_id=e.employee_id " +
                $"INNER JOIN tm_department f on e.department_id = f.department_id  " +
                $"where {filterDept} YEAR(a.absent_date)='{myYear}' and MONTH(a.absent_date)='{myMonth}' {filterEmployee} " +
                $"AND e.title_id in ('DS006','DS004','DS003') and  {timeCondition} and  {notIn} and a.absent_date not in (select holiday_date from tm_holiday_date ) {conditionEmployee}" +
                $"order by f.department_name,a.employee_id,a.absent_date asc ";

            var employee = new DatabaseContext().ComeLateQueries.FromSqlRaw(sql).ToList();
            return employee;
        }

        public List<ComeLateSumQuery> ComeLateEmployeeSum(int isOB, string departmentId, string myEmployeeId, string myYear, string myMonth)
        {
            string sql = "";
            string filterDept = "";
            string filterEmployee = "";

            string notIn = "DAYOFWEEK(a.absent_date) not in (1,7)"; // Hari Minggu (1) dan Sabtu (7) //harusnya ada kondisi juga untuk dept all
            string timeCondition = "";
            if (departmentId == null || departmentId == "all" || departmentId == "none")
            {
                notIn = "CASE WHEN e.is_office_boy = 1 THEN DAYOFWEEK(a.absent_date) not in (1) ELSE DAYOFWEEK(a.absent_date) not in (1,7) END";
                timeCondition = "CASE WHEN e.is_office_boy = 1 THEN CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_in > '09:15:00' ELSE a.time_in > '07:45:00' END WHEN e.is_office_boy = 0 THEN a.time_in > '09:00:00' END ";
            }
            else
            {
                filterDept = $" e.department_id='{departmentId}' AND ";
                notIn = "CASE WHEN e.is_office_boy = 1 THEN DAYOFWEEK(a.absent_date) not in (1) ELSE DAYOFWEEK(a.absent_date) not in (1,7) END";
                timeCondition = "CASE WHEN e.is_office_boy = 1 THEN CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_in > '09:15:00' ELSE a.time_in > '07:45:00' END WHEN e.is_office_boy = 0 THEN a.time_in > '09:00:00' END ";
            }
            if (myEmployeeId == null || myEmployeeId == "all" || myEmployeeId == "none")
            {

            }
            else
            {
                filterEmployee = $" AND a.employee_id='{myEmployeeId}' ";

                if (isOB == 1)
                {
                    // Jika OB, atur waktu terlambat Senin-Jumat 07:45 dan Sabtu 09:15
                    timeCondition = "CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_in > '09:15:00' ELSE a.time_in > '07:45:00' END";
                    notIn = "DAYOFWEEK(a.absent_date) not in (1)"; // Hanya hari Minggu yang libur
                }
                else
                {
                    // Jika bukan OB, atur waktu terlambat Senin-Jumat 09:00
                    timeCondition = "a.time_in > '09:00:00'";
                }
            }

            sql = $"SELECT a.employee_id,a.department_id,a.employee_name,a.department_name,count(a.absent_date) as qty " +
                $"From (" +
                $"select a.employee_id,e.department_id,a.absent_date,a.time_in,a.time_out,  DATE_FORMAT(a.absent_date, '%d %M %Y') as absent_date_cap," +
                $"COALESCE(b.description, c.description, d.description, '') AS description, " +
                $"COALESCE(b.status, c.status, d.status, '') AS status, " +
                $"case when b.description is  null then 1 " +
                $"  when c.description is  null then 1 " +
                $"  when d.description is  null then 1 " +
                $"  else 0 end as flag, " +
                $"concat(e.employee_first_name, ' ', e.employee_last_name) as employee_name, " +
                $"f.department_name " +
                $"from tp_absent a " +
                $"LEFT JOIN ( " +
                $"      SELECT  b.employee_id, a.permission_date AS my_date,  " +
                $"      CONCAT(c.permission_type, '-', b.reason) AS description,  " +
                $"      CASE WHEN b.status = 1 THEN 'Approved'  ELSE 'Waiting Approve'  END AS status  " +
                $"      FROM tp_permission_detail a  " +
                $"          INNER JOIN tp_permission b ON a.permission_id = b.permission_id  " +
                $"          INNER JOIN tm_permission_type c ON b.permission_type_id = c.permission_type_id  " +
                $"          WHERE b.deleted_at IS NULL  " +
                $"      ) b ON a.absent_date = b.my_date AND a.employee_id = b.employee_id  " +
                $"LEFT JOIN ( " +
                $"      SELECT b.employee_id,a.cuti_date AS my_date, CONCAT('Cuti ', c.nama_cuti, '-', b.keperluan) AS description,  " +
                $"      CASE WHEN b.status = 1 THEN 'Approved'  ELSE 'Waiting Approve'  END AS status  " +
                $"      FROM tp_detail_cuti a  " +
                $"          INNER JOIN tp_cuti b ON a.cuti_id = b.cuti_id " +
                $"          INNER JOIN tm_type_cuti c ON b.type_cuti_id = c.type_cuti_id     WHERE b.deleted_at IS NULL " +
                $"    ) c ON a.absent_date = c.my_date AND a.employee_id = c.employee_id " +
                $"LEFT JOIN ( " +
                $"      SELECT  employee_id, absent_date AS my_date, 'No Card (No NameTag)' AS description,  ' ' AS status  " +
                $"      FROM tp_absent_no_card  " +
                $"    ) d ON a.absent_date = d.my_date AND a.employee_id = d.employee_id  " +
                $"INNER JOIN tm_employee_affair e on a.employee_id=e.employee_id " +
                $"INNER JOIN tm_department f on e.department_id = f.department_id  " +
                $"where {filterDept} YEAR(a.absent_date)='{myYear}' and MONTH(a.absent_date)='{myMonth}' {filterEmployee} " +
                $"AND e.title_id in ('DS006','DS004','DS003') and {timeCondition} and  {notIn} and a.absent_date not in (select holiday_date from tm_holiday_date ) " +
                $") a " +
                $"where a.flag=1 group by a.employee_id,a.department_id,a.employee_name,a.department_name";

            var employee = new DatabaseContext().ComeLateSumQueries.FromSqlRaw(sql).ToList();
            return employee;
        }

        public List<UnAbsenteeDepartmentQuery> UnAbsenteeDepartment(string departmentId, string myYear, string myMonth, string myPeriodeId2)
        {
            string sql = "";
            string filterDept = "";
            if (departmentId == null || departmentId == "all" || departmentId == "none")
            {

            }
            else
            {
                filterDept = $" z.department_id='{departmentId}' AND ";
            }

            sql = $"SELECT a.employee_id,a.employee_name,a.department_id,a.department_name,a.my_date as absent_date, " +
                  $"DATE_FORMAT(a.my_date, '%d %M %Y') as absent_date_cap, " +
                  $"COALESCE(b.description, c.description, d.description, e.description, f.description, '') AS description, " +
                  $"COALESCE(b.status, c.status, d.status, e.status, f.status, '') AS status " +
                  $"FROM (" +
                  $"    SELECT DATE(CONCAT('{myPeriodeId2}-', LPAD(n, 2, '0'))) AS my_date, " +
                  $"    e.employee_id, e.employee_name, e.department_id, e.department_name, e.is_office_boy " +
                  $"    FROM (SELECT n FROM numbers WHERE n <= DAY(LAST_DAY('{myPeriodeId2}-01'))) a " +
                  $"    CROSS JOIN ( " +
                  $"        SELECT z.employee_id, CONCAT(z.employee_first_name, ' ', z.employee_last_name) as employee_name, " +
                  $"        z.department_id, b.department_name, z.is_office_boy " +
                  $"        FROM tm_employee_affair z " +
                  $"        INNER JOIN tm_department b on z.department_id=b.department_id " +
                  $"        WHERE {filterDept} z.status=1 " +
                  $"    ) e " +
                  $"    WHERE CONCAT(DATE(CONCAT('{myPeriodeId2}-', LPAD(n, 2, '0'))), e.employee_id) NOT IN ( " +
                  $"        SELECT DISTINCT CONCAT(a.absent_date, a.employee_id) " +
                  $"        FROM tp_absent a " +
                  $"        INNER JOIN tm_employee_affair z ON a.employee_id = z.employee_id " +
                  $"        WHERE {filterDept} a.is_wfh=0 AND YEAR(a.absent_date)='{myYear}' AND MONTH(a.absent_date)='{myMonth}' " +
                  $"    ) " +
                  $"    AND CASE WHEN e.is_office_boy = 1 THEN DAYOFWEEK(DATE(CONCAT('{myPeriodeId2}-', LPAD(n, 2, '0')))) NOT IN (1) " +
                  $"             ELSE DAYOFWEEK(DATE(CONCAT('{myPeriodeId2}-', LPAD(n, 2, '0')))) NOT IN (1,7) END " +
                  $") a " +
                  $"/** Join ke izin **/ " +
                  $"LEFT JOIN ( " +
                  $"    SELECT b.employee_id, a.permission_date AS my_date, " +
                  $"    CONCAT(c.permission_type, '-', b.reason) AS description, " +
                  $"    CASE WHEN b.status = 1 THEN 'Approved' ELSE 'Waiting Approve' END AS status " +
                  $"    FROM tp_permission_detail a " +
                  $"    INNER JOIN tp_permission b ON a.permission_id = b.permission_id " +
                  $"    INNER JOIN tm_permission_type c ON b.permission_type_id = c.permission_type_id " +
                  $"    WHERE b.deleted_at IS NULL " +
                  $") b ON a.my_date = b.my_date AND a.employee_id = b.employee_id " +
                  $"/** Join ke hari libur besar **/ " +
                  $"LEFT JOIN ( " +
                  $"    SELECT holiday_date AS my_date, holiday_name AS description, 'Hari Besar' AS status " +
                  $"    FROM tm_holiday_date " +
                  $") c ON a.my_date = c.my_date " +
                  $"/** Join ke cuti **/ " +
                  $"LEFT JOIN ( " +
                  $"    SELECT b.employee_id, a.cuti_date AS my_date, CONCAT('Cuti ', c.nama_cuti, '-', b.keperluan) AS description, " +
                  $"    CASE WHEN b.status = 1 THEN 'Approved' ELSE 'Waiting Approve' END AS status " +
                  $"    FROM tp_detail_cuti a " +
                  $"    INNER JOIN tp_cuti b ON a.cuti_id = b.cuti_id " +
                  $"    INNER JOIN tm_type_cuti c ON b.type_cuti_id = c.type_cuti_id " +
                  $"    WHERE b.deleted_at IS NULL " +
                  $") d ON a.my_date = d.my_date AND a.employee_id = d.employee_id " +
                  $"/** Join ke WFH **/ " +
                  $"LEFT JOIN ( " +
                  $"    SELECT employee_id, request_date AS my_date, CONCAT('WFH-', reason) AS description, " +
                  $"    CASE WHEN status = 1 THEN 'Approved' ELSE 'Waiting Approve' END AS status " +
                  $"    FROM tp_request_wfh WHERE status NOT IN (-1) " +
                  $") e ON a.my_date = e.my_date AND a.employee_id = e.employee_id " +
                  $"/** Join ke no card **/ " +
                  $"LEFT JOIN ( " +
                  $"    SELECT employee_id, absent_date AS my_date, 'No Card (No NameTag)' AS description, ' ' AS status " +
                  $"    FROM tp_absent_no_card " +
                  $") f ON a.my_date = f.my_date AND a.employee_id = f.employee_id " +
                  $"ORDER BY a.department_id, a.employee_id, a.my_date ASC;";
          
            var department = new DatabaseContext().UnAbsenteeDepartmentQueries.FromSqlRaw(sql).ToList();
            return department;
        }

        public List<ComeLateSumQuery> IncompleteAttendanceSum(int isOB, string departmentId, string myEmployeeId, string myYear, string myMonth)
        {
            string sql = "";
            string filterDept = "";
            string filterEmployee = "";

            string notIn = "DAYOFWEEK(a.absent_date) not in (1,7)"; // Hari Minggu (1) dan Sabtu (7) //harusnya ada kondisi juga untuk dept all
            string timeCondition = "";

            if (departmentId == null || departmentId == "all" || departmentId == "none")
            {
                notIn = "CASE WHEN e.is_office_boy = 1 THEN DAYOFWEEK(a.absent_date) not in (1) ELSE DAYOFWEEK(a.absent_date) not in (1,7) END";
                timeCondition = "CASE WHEN e.is_office_boy = 1 THEN CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_out < '12:00:00' ELSE a.time_out < '18:00:00' END WHEN e.is_office_boy = 0 THEN CASE WHEN a.time_in  <= '08:00:00' THEN time_out < '17:00:00' ELSE CASE WHEN a.time_in <= '09:00:00' THEN TIMEDIFF(a.time_out, a.time_in) < '09:00:00' END END  ELSE 0 END";
            }
            else
            {
                filterDept = $" e.department_id='{departmentId}' AND ";

                notIn = "CASE WHEN e.is_office_boy = 1 THEN DAYOFWEEK(a.absent_date) not in (1) ELSE DAYOFWEEK(a.absent_date) not in (1,7) END";
                /*      timeCondition = "CASE WHEN e.is_office_boy = 1 THEN CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_out < '12:00:00' ELSE a.time_out < '18:00:00' END WHEN e.is_office_boy = 0 THEN TIMEDIFF(a.time_out, a.time_in) < '09:00:00' ELSE 0 END";*/

                timeCondition = "CASE WHEN e.is_office_boy = 1 THEN CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_out < '12:00:00' ELSE a.time_out < '18:00:00' END WHEN e.is_office_boy = 0 THEN CASE WHEN a.time_in  <= '08:00:00' THEN time_out < '17:00:00' ELSE CASE WHEN a.time_in <= '09:00:00' THEN TIMEDIFF(a.time_out, a.time_in) < '09:00:00' END END  ELSE 0 END";
            }

            if (myEmployeeId == null || myEmployeeId == "all" || myEmployeeId == "none")
            {

            }
            else
            {
                filterEmployee = $" AND a.employee_id='{myEmployeeId}' ";

                if (isOB == 1)
                {
                    // Jika OB, atur waktu terlambat Senin-Jumat 07:45 dan Sabtu 09:15
                    timeCondition = "CASE WHEN DAYOFWEEK(a.absent_date) = 7 THEN a.time_out < '12:00:00' ELSE a.time_out < '18:00:00' END";
                    notIn = "DAYOFWEEK(a.absent_date) not in (1)"; // Hanya hari Minggu yang libur
                }
                else
                {
                    // Jika bukan OB, atur waktu terlambat Senin-Jumat 09:00
                    //timeCondition = "CASE WHEN a.time_in <= '08:00:00' THEN a.time_out < '17:00:00' ELSE a.time_out < '18:00:00' END";
                    timeCondition = "CASE WHEN a.time_in <= '08:00:00' THEN time_out < '17:00:00' ELSE CASE WHEN a.time_in <= '09:00:00' THEN TIMEDIFF(a.time_out, a.time_in) < '09:00:00' END END ";
                }
            }

            sql = $"SELECT a.employee_id,a.department_id,a.employee_name,a.department_name,count(a.absent_date) as qty " +
                $"From (" +
                $"select a.employee_id,e.department_id,a.absent_date,a.time_in,a.time_out,  DATE_FORMAT(a.absent_date, '%d %M %Y') as absent_date_cap," +
                $"COALESCE(b.description, c.description, d.description, '') AS description, " +
                $"COALESCE(b.status, c.status, d.status, '') AS status, " +
                $"case when b.description is  null then 1 " +
                $"  when c.description is  null then 1 " +
                $"  when d.description is  null then 1 " +
                $"  else 0 end as flag, " +
                $"concat(e.employee_first_name, ' ', e.employee_last_name) as employee_name, " +
                $"f.department_name " +
                $"from tp_absent a " +
                $"LEFT JOIN ( " +
                $"      SELECT  b.employee_id, a.permission_date AS my_date,  " +
                $"      CONCAT(c.permission_type, '-', b.reason) AS description,  " +
                $"      CASE WHEN b.status = 1 THEN 'Approved'  ELSE 'Waiting Approve'  END AS status  " +
                $"      FROM tp_permission_detail a  " +
                $"          INNER JOIN tp_permission b ON a.permission_id = b.permission_id  " +
                $"          INNER JOIN tm_permission_type c ON b.permission_type_id = c.permission_type_id  " +
                $"          WHERE b.deleted_at IS NULL  " +
                $"      ) b ON a.absent_date = b.my_date AND a.employee_id = b.employee_id  " +
                $"LEFT JOIN ( " +
                $"      SELECT b.employee_id,a.cuti_date AS my_date, CONCAT('Cuti ', c.nama_cuti, '-', b.keperluan) AS description,  " +
                $"      CASE WHEN b.status = 1 THEN 'Approved'  ELSE 'Waiting Approve'  END AS status  " +
                $"      FROM tp_detail_cuti a  " +
                $"          INNER JOIN tp_cuti b ON a.cuti_id = b.cuti_id " +
                $"          INNER JOIN tm_type_cuti c ON b.type_cuti_id = c.type_cuti_id     WHERE b.deleted_at IS NULL " +
                $"    ) c ON a.absent_date = c.my_date AND a.employee_id = c.employee_id " +
                $"LEFT JOIN ( " +
                $"      SELECT  employee_id, absent_date AS my_date, 'No Card (No NameTag)' AS description,  ' ' AS status  " +
                $"      FROM tp_absent_no_card  " +
                $"    ) d ON a.absent_date = d.my_date AND a.employee_id = d.employee_id  " +
                $"INNER JOIN tm_employee_affair e on a.employee_id=e.employee_id " +
                $"INNER JOIN tm_department f on e.department_id = f.department_id  " +
                $"where {filterDept} YEAR(a.absent_date)='{myYear}' and MONTH(a.absent_date)='{myMonth}' {filterEmployee} " +
                $"AND e.title_id in ('DS006','DS004','DS003') and {timeCondition} and  {notIn} and a.absent_date not in (select holiday_date from tm_holiday_date ) " +
                $") a " +
                $"where a.flag=1 group by a.employee_id,a.department_id,a.employee_name,a.department_name";

            var employee = new DatabaseContext().ComeLateSumQueries.FromSqlRaw(sql).ToList();
            return employee;
        }

        public List<Dictionary<string, dynamic>> CreateReqPermitt(TpAbsentRevisi value)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();
            var executionStrategy = context.Database.CreateExecutionStrategy();
            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {

                    try
                    {
                        context.Database.ExecuteSqlRaw($"DELETE FROM tp_absent_revisi WHERE employee_id='{value.EmployeeId}' and  absent_date='{value.AbsentDate.ToString("yyyy-MM-dd")}'"); // ngetest rollback, field id tidak dikenal
                        context.SaveChanges();


                        context.TpAbsentRevisis.Add(value);
                        context.SaveChanges();

                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Membuat Pengajuan Izin");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException == null ? e.Message : e.InnerException);
                        System.Console.WriteLine(e.Message);
                        transaction.Rollback();

                        data.Clear();
                        data.Add("result", false);
                        data.Add("message", e.Message);

                        result.Clear();
                        result.Add(data);


                    }
                }

            });
            return result;
        }

        public List<Dictionary<string, dynamic>> AppPermitt(TpAbsentRevisi value)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();

            var current = context.TpAbsentRevisis.Where(x => x.EmployeeId == value.EmployeeId && x.AbsentDate == value.AbsentDate).FirstOrDefault() ?? throw new Exception(value.EmployeeId + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();
            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {

                    try
                    {

                        TpAbsentRevisi updateForApproval = new TpAbsentRevisi()
                        {
                            EmployeeId = value.EmployeeId,
                            AbsentDate = value.AbsentDate,
                            Status = value.Status,
                            ApproveReason = value.ApproveReason,
                            ApproveBy = value.ApproveBy,
                            ApproveDate = DateTime.Now,
                            Category = current.Category,
                            TypeRevisi = current.TypeRevisi,
                            RequestBy = current.RequestBy,
                            Reason = current.Reason,
                            FileName = current.FileName,
                            Type = current.Type,
                            Size = current.Size,
                            CreatedBy = current.CreatedBy,
                            CreatedAt = current.CreatedAt,
                            UpdatedBy = current.UpdatedBy,
                            UpdatedAt = current.UpdatedAt,
                        };

                        context.TpAbsentRevisis.Entry(current).CurrentValues.SetValues(updateForApproval);
                        context.SaveChanges();

                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Approval izin berhasil disimpan");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                        transaction.Rollback();

                        data.Clear();
                        data.Add("result", false);
                        data.Add("message", e.Message);

                        result.Clear();
                        result.Add(data);


                    }
                }

            });
            return result;
        }


    }

    public class ApprovePermitt
    {
        public string EmployeeId { get; set; }
        public DateTime AbsentDate { get; set; }
        public string ApproveReason { get; set; }
        public int Status { get; set; }
    }

    public class RequestPermitt
    {
        public string EmployeeId { get; set; }
        public DateTime AbsentDate { get; set; }
        public string Category { get; set; }
        public string TypeRevisi { get; set; }
        public string Reason { get; set; }
        public IFormFile FileName { get; set; }
        public int Status { get; set; }
    }

    public class UnAbsenteeDeptModel
    {
        [Required] public string periodeId { get; set; } = null!;
        [Required] public string departmentId { get; set; } = null!;
    }

    public class IncompleteAttendanceModel
    {
        [Required] public string periodeId { get; set; } = null!;
        [Required] public string departmentId { get; set; } = null!;
    }

    public class AbsenteeRetrive
    {
        [Required] public string periodeId { get; set; } = null!;
        [Required] public string departmentId { get; set; } = null!;
    }

    public class AbsenteeEmployeeModel
    {
        [Required] public string periodeId { get; set; } = null!;
        [Required] public string employeeId { get; set; } = null!;
    }

    [Keyless]
    public class AttendanceRecordQuery
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string EmployeeLastName { get; set; }

        [Column("department_name")]
        public string DepartmentName { get; set; }

        [Column("title_name")]
        public string TitleName { get; set; }

        [Column("is_cut_absentee")]
        public int IsCutAbsentee { get; set; }

        [Column("is_overtime")]
        public int IsOvertime { get; set; }

        [Column("absent_status")]
        public string AbsentStatus { get; set; }

        [Column("overtime_status")]
        public string OvertimeStatus { get; set; }

    }

    [Keyless]
    public class PeriodsQuery
    {
        [Column("periode_id")]
        public string PeriodeId { get; set; }

        [Column("periode_name")]
        public string? PeriodeName { get; set; }
    }

    [Keyless]
    public class FindIsOB
    {
        [Column("is_office_boy")]
        public int isOB { get; set; }
    }

    [Keyless]
    public class AttendanceEmployeeQuery
    {
        [Column("absent_date")]
        public string AbsentDate { get; set; }

        [Column("absent_date_db")]
        public string AbsentDateDb { get; set; }

        [Column("periode")]
        public string Periode { get; set; }

        [Column("my_date")]
        public string MyDate { get; set; }

        [Column("periode_link")]
        public string PeriodeLink { get; set; }

        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string EmployeeLastName { get; set; }

        [Column("department_name")]
        public string DepartmentName { get; set; }

        [Column("title_name")]
        public string TitleName { get; set; }

        [Column("time_in")]
        public string TimeIn { get; set; }

        [Column("time_out")]
        public string TimeOut { get; set; }

        [Column("is_cut_absentee_des")]
        public int IsCutAbsenteeDes { get; set; }

        [Column("is_overtime_des")]
        public int IsOvertimeDes { get; set; }

        [Column("title_id")]
        public string TitleId { get; set; }

        [Column("department_id")]
        public string DepartmentId { get; set; }

        [Column("category")]
        public string? Category { get; set; }

        [Column("type_revisi")]
        public string? TypeRevisi { get; set; }

        [Column("reason")]
        public string? Reason { get; set; }

        [Column("file_name")]
        public string? FileName { get; set; }

        [Column("size")]
        public int? Size { get; set; }

        [Column("status")]
        public int? Status { get; set; }

        [Column("is_absent")]
        public int? IsAbsent { get; set; }

        [Column("my_hour")]
        public string? MyHour { get; set; }

        [Column("my_hour_efektif")]
        public string? MyHourEfektif { get; set; }

        [Column("is_overtime")]
        public string? IsOvertime { get; set; }

        [Column("overtime")]
        public string? Overtime { get; set; }

        [Column("status_potong_cuti")]
        public string? StatusPotongCuti { get; set; }

        [Column("cut_off_absent_des")]
        public string? CutOffAbsentDes { get; set; }

        [Column("link_modal_cut_off")]
        public string? LinkModalCutOff { get; set; }

    }

    [Keyless]
    public class UnAbsenteeEmployeeQuery
    {
        [Column("absent_date_cap")]
        public string AbsentDateCap { get; set; }

        [Column("absent_date")]
        public DateTime AbsentDate { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("status")]
        public string Status { get; set; }
    }

    [Keyless]
    public class ComeLateQuery
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("employee_name")]
        public string EmployeeName { get; set; }

        [Column("department_id")]
        public string DepartmentId { get; set; }

        [Column("department_name")]
        public string DepartmentName { get; set; }

        [Column("absent_date_cap")]
        public string AbsentDateCap { get; set; }

        [Column("time_in")]
        public string TimeIN { get; set; }

        [Column("time_out")]
        public string TimeOut { get; set; }

        [Column("absent_date")]
        public DateTime AbsentDate { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("status")]
        public string? Status { get; set; }


    }

    [Keyless]
    public class ComeLateSumQuery
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("employee_name")]
        public string EmployeeName { get; set; }

        [Column("qty")]
        public int Qty { get; set; }

        [Column("department_id")]
        public string DepartmentId { get; set; }

        [Column("department_name")]
        public string DepartmentName { get; set; }





    }


    [Keyless]
    public class UnAbsenteeDepartmentQuery
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("employee_name")]
        public string EmployeeName { get; set; }

        [Column("department_id")]
        public string DepartmentId { get; set; }

        [Column("department_name")]
        public string DepartmentName { get; set; }

        [Column("absent_date_cap")]
        public string AbsentDateCap { get; set; }

        [Column("absent_date")]
        public DateTime AbsentDate { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("status")]
        public string? Status { get; set; }


    }



}
