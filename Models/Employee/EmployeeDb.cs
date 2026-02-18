using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Database.MySql.Support;
using Microsoft.EntityFrameworkCore;
using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Controllers.Master;

namespace BackendHrdAgro.Models.Employee
{
    public class EmployeeDbModel
    {
        DatabaseContext databaseContext = new DatabaseContext();
        UserDB userDB = new UserDB();

        public List<EmployeeGroupQuery> employeeGroupQueries(string kriteria)
        {
            var sql = "select a.*,case when a.status = 1 then 'Active' when a.status=0 then 'Non Active' " +
                "when a.status=5 then 'PHK' when a.status=7 then 'Kontrak Habis / Kontrak Tidak diperpanjang' " +
                "when a.status=9 then 'Resign' end as my_status,\r\n  bank_name,department_name,title_name,religi_name," +
                "level_name,gender_name,employee_status_name,married_name,i.termination_date,\r\n  " +
                "DATE_FORMAT(a.tanggal_lahir, '%d %M %Y') as tanggal_lahir_view,DATE_FORMAT(a.joint_date, '%d %M %Y') as " +
                "joint_date_view,DATE_FORMAT(a.end_of_contract, '%d %M %Y') as end_of_contract_view," +
                "DATE_FORMAT(a.permanent_date, '%d %M %Y') as permanent_date_view\r\n\t\tfrom tm_employee_affair a\r\n\t\t" +
                "left outer join tm_department b on a.department_id=b.department_id\r\n\t\tleft outer join tm_title c on a.title_id=c.title_id\r\n    " +
                "left outer join tm_level d on a.level_id=d.level_id\r\n    left outer join tm_employee_status e on a.employee_status_id=e.employee_status_id\r\n    " +
                "left outer join tm_religi f on a.religi_id=f.religi_id\r\n    left outer join tm_married g on a.married_id=g.married_id\r\n    " +
                "left outer join tm_bank h on a.bank_id=h.bank_id\r\n    left outer join tm_sex i on a.gender_id=i.gender_id\r\n    " +
                "left outer join (select * from tp_termination where  DATE_FORMAT(now(),'%Y-%m-%d') <= DATE_FORMAT(DATE_ADD(termination_date, INTERVAL 1 MONTH),'%Y-%m-%d') ) as i " +
                $"on a.employee_id=i.employee_id\r\n\t\twhere a.status in (1,5,7,9) {kriteria} order by employee_first_name ASC";
            var group = new DatabaseContext().EmployeeGroups.FromSqlRaw(sql).ToList();
            return group;
        }

        public List<Departments> departments()
        {
            var sql = "select department_id,department_name from tm_department a  where a.status =1  order by department_name ASC";
            var department = new DatabaseContext().Departments.FromSqlRaw(sql).ToList();
            return department;
        }

        public List<Types> types()
        {
            var sql = "select type_id,type_name from tm_type a  where a.status in (1,0,9) order by type_name ASC";
            var type = new DatabaseContext().Types.FromSqlRaw(sql).ToList();
            return type;
        }

        public List<Religions> religions()
        {
            var sql = "select religi_id,religi_name from tm_religi a  where a.status in (1,0,9) order by religi_name ASC";
            var religi = new DatabaseContext().Religions.FromSqlRaw(sql).ToList();
            return religi;
        }

        public List<Marrieds> marrieds()
        {
            var sql = "select married_id,married_name from tm_married a  where a.status in (1,0,9) order by married_name ASC";
            var married = new DatabaseContext().Marrieds.FromSqlRaw(sql).ToList();
            return married;
        }

        public List<Genders> genders()
        {
            var sql = "select gender_id,gender_name from tm_sex a  where a.status in (1,0,9) order by gender_name ASC";
            var gender = new DatabaseContext().Genders.FromSqlRaw(sql).ToList();
            return gender;
        }

        public List<Titles> titles()
        {
            var sql = "select title_id,title_name from tm_title a  where a.status in (1) order by title_name ASC";
            var title = new DatabaseContext().Titles.FromSqlRaw(sql).ToList();
            return title;
        }

        public List<Levels> levels()
        {
            var sql = "select level_id,level_name from tm_level a  where a.status in (1,0,9) order by level_id ASC";
            var level = new DatabaseContext().Levels.FromSqlRaw(sql).ToList();
            return level;
        }

        public List<EmployeeStatus> employeeStatuses()
        {
            var sql = "select employee_status_id,employee_status_name from tm_employee_status a  where a.status in (1,0,9) order by employee_status_name ASC";
            var employeeStatus = new DatabaseContext().EmployeeStatuses.FromSqlRaw(sql).ToList();
            return employeeStatus;
        }

        public List<BankStatus> bankStatuses()
        {
            var sql = "select bank_id,bank_name from tm_bank a  where a.status in (1,0,9) order by bank_name ASC";
            var bankStatus = new DatabaseContext().BankStatuses.FromSqlRaw(sql).ToList();
            return bankStatus;
        }

        public List<ReasonStatus> reasonStatuses()
        {
            var sql = "select reason_id,reason_name from tm_reason a  where a.status in (1,0,9) order by reason_id ASC";
            var reasonStatus = new DatabaseContext().ReasonStatuses.FromSqlRaw(sql).ToList();
            return reasonStatus;
        }

        public List<BloodType> bloodTypes()
        {
            var sql = "select blood_type_id, blood_type_name from tm_blood_type a where a.status in (1,0,9) order by blood_type_name ASC";
            var blood = new DatabaseContext().BloodTypes.FromSqlRaw(sql).ToList();
            return blood;
        }

        //GetOne
        public List<listFamily> listFamilies(getModel value)
        {
            var sql = "select a.*,case when a.status = 1 then 'Yes' when a.status=0 then 'No' end as my_status,\r\n        " +
                "employee_first_name,hubungan_name, DATE_FORMAT(a.tanggal_lahir, '%d %M %Y') as tanggal_lahir_view        " +
                "from tp_employee_family a\r\n        left outer join tm_employee_affair b on a.employee_id=b.employee_id\r\n        " +
                "left outer join tm_hubungan d on a.hubungan_id=d.hubungan_id\r\n        " +
                $"where b.employee_id= {value.EmployeeId} and a.status in (1,0,9) order by employee_first_name ASC";
            var listFamily = new DatabaseContext().ListFamilies.FromSqlRaw(sql).ToList();
            return listFamily;
        }

        public List<Relationship> relationships()
        {
            var sql = "select hubungan_id,hubungan_name from tm_hubungan a  where a.status in (1,0,9) and is_family=1 order by hubungan_name ASC";
            var relationship = new DatabaseContext().Relationships.FromSqlRaw(sql).ToList();
            return relationship;
        }

        public List<EmployeeDepartment> employeeDepartments(getModel value)
        {
            var sql = "select employee_first_name,employee_last_name,department_name\r\n        " +
                "from tm_employee_affair a\r\n        inner join tm_department b on a.department_id=b.department_id\r\n        " +
                $"where employee_id = '{value.EmployeeId}'";
            var myresult = new DatabaseContext().EmployeeDepartments.FromSqlRaw(sql).ToList();
            return myresult;
        }
        //END GetOne

        //GetTwo
        public List<listStudi> listStudis(getModel value)
        {
            var sql = "select a.*,case when a.status = 1 then 'Certified' when a.status=0 then 'No Certified' " +
                "end as my_status,employee_first_name,type_ins_name,studi_name,\r\n DATE_FORMAT(a.periode, '%d %M %Y') " +
                "as periode_view,DATE_FORMAT(a.akhir, '%d %M %Y') as akhir_view\r\n      from tp_employee_studi a\r\n      " +
                "left outer join tm_employee_affair b on a.employee_id=b.employee_id\r\n  " +
                "left outer join tm_studi c on a.studi_id=c.studi_id\r\n  " +
                "left outer join tm_type_ins d on a.type_ins_id=d.type_ins_id\r\n      " +
                $"where b.employee_id= '{value.EmployeeId}' and a.status in (1,0,9) order by employee_first_name ASC";
            var listStudi = new DatabaseContext().ListStudis.FromSqlRaw(sql).ToList();
            return listStudi;
        }

        public List<StudyId> studyIds()
        {
            var sql = "select studi_id,studi_name from tm_studi a  where a.status in (1,0,9) order by studi_id Desc";
            var studi = new DatabaseContext().StudyIds.FromSqlRaw(sql).ToList();
            return studi;
        }

        public List<InsuranceType> insuranceTypes()
        {
            var sql = "select type_ins_id,type_ins_name from tm_type_ins a  where a.status in (1,0,9) order by     type_ins_id Desc";
            var insuranceType = new DatabaseContext().InsuranceTypes.FromSqlRaw(sql).ToList();
            return insuranceType;
        }
        //END GetTwo 

        //GetThree
        public List<listLicense> listLicenses(getModel value)
        {
            var sql = "select a.*,case when a.status = 1 then 'Certified' when a.status=0 " +
                "then 'No Certified' end as my_status,employee_first_name,type_ins_name, DATE_FORMAT(a.periode, '%d %M %Y') " +
                "as periode_view\r\n          from tp_employee_license a\r\n          " +
                "left outer join tm_employee_affair b on a.employee_id=b.employee_id\r\n          " +
                "left outer join tm_type_ins c on a.type_ins_id=c.type_ins_id\r\n          " +
                $"where b.employee_id= '{value.EmployeeId}' and a.status in (1,0,9) order by employee_first_name ASC";
            var listLicense = new DatabaseContext().ListLicenses.FromSqlRaw(sql).ToList();
            return listLicense;
        }

        public List<License> licenses()
        {
            var sql = "select license_id,license_name from tp_employee_license a  where a.status in (1,0,9) order by license_name ASC";
            var licenseid = new DatabaseContext().Licenses.FromSqlRaw(sql).ToList();
            return licenseid;
        }
        //END GetThree

        //GetFour
        public List<listRelation> listRelations(getModel value)
        {
            var sql = "select a.*,case when a.status = 1 then 'Yes' when a.status=0 then 'No' end as my_status,\r\n          " +
                "employee_first_name,hubungan_name " +
                "from tp_employee_relation a\r\n          left outer join tm_employee_affair b on a.employee_id=b.employee_id\r\n          " +
                "left outer join tm_hubungan d on a.hubungan_id=d.hubungan_id\r\n          " +
                $"where b.employee_id= '{value.EmployeeId}' and a.status in (1,0,9) order by employee_first_name ASC";
            var listRelation = new DatabaseContext().ListRelations.FromSqlRaw(sql).ToList();
            return listRelation;
        }
        //END GetFour

        public List<ViewData> viewDatas(getModel value)
        {
            var sql = "select employee_first_name,employee_last_name,department_name,title_name,level_name,\r\n          " +
                "employee_status_name,joint_date,end_of_contract,permanent_date,tempat_lahir,tanggal_lahir,alamat,alamat_tinggal,religi_name,\r\n          " +
                "married_name,gender_name,bank_name,no_rekening, IFNULL(h.file_name, '') as file_name\r\n          " +
                "from tm_employee_affair a left outer join tm_department b on a.department_id=b.department_id\r\n          " +
                "left outer join tm_title c on a.title_id=c.title_id  left outer join tm_level d on a.level_id=d.level_id\r\n          " +
                "left outer join tm_employee_status e on a.employee_status_id=e.employee_status_id\r\n          " +
                "left outer join tm_religi f on a.religi_id=f.religi_id left outer join tm_married g on a.married_id=g.married_id\r\n          " +
                "left outer join tm_bank h on a.bank_id=h.bank_id\r\n          left outer join tm_sex i on a.gender_id=i.gender_id\r\n          " +
                "inner join (select employee_id, file_name from tp_employee_affair_document where claim_type_id='CT000' and document_id=1) h on a.employee_id=h.employee_id      " +
                $"where a.employee_id = '{value.EmployeeId}'";
            var viewdata = new DatabaseContext().ViewData.FromSqlRaw(sql).ToList();
            return viewdata;
        }

        public List<viewLicense> viewLicenses(getModel value)
        {
            var sql = "select a.*,case when a.status = 1 then 'Yes' when a.status=0 then 'No' end " +
                "as my_status,employee_first_name,type_ins_name,DATE_FORMAT(a.periode, '%d %M %Y') as periode_cetak\r\n          " +
                "from tp_employee_license a\r\n          left outer join tm_employee_affair b on a.employee_id=b.employee_id\r\n          " +
                "left outer join tm_type_ins d on a.type_ins_id=d.type_ins_id\r\n          " +
                $"where b.employee_id= '{value.EmployeeId}' and a.status in (1,0,9) order by employee_first_name ASC";
            var viewLicense = new DatabaseContext().ViewLicense.FromSqlRaw(sql).ToList();
            return viewLicense;
        }

        public List<viewRelation> viewRelations(getModel value)
        {
            var sql = "select a.*,case when a.status = 1 then 'Yes' when a.status=0 then 'No' end as my_status,\r\n            " +
                "employee_first_name,alamat from tp_employee_relation a\r\n            " +
                "left outer join tm_employee_affair b on a.employee_id=b.employee_id\r\n           " +
                $"where b.employee_id= '{value.EmployeeId}' and a.status in (1,0,9) order by employee_first_name ASC";
            var viewRelation = new DatabaseContext().ViewRelations.FromSqlRaw(sql).ToList();
            return viewRelation;
        }

        public List<Documents> documents(getModel value)
        {
            var sql = "select a.* ,document_name  from tp_employee_affair_document a  inner join tm_employee_affair_general_document c on a.document_id=c.document_id     " +
                $"where a.employee_id= '{value.EmployeeId}' order by document_id ASC";
            var document = new DatabaseContext().Documents.FromSqlRaw(sql).ToList();
            return document;
        }

        public List<Dictionary<string, dynamic>> CreateGetOne(TpEmployeeFamily value)
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
                        context.TpEmployeeFamilies.Add(value);
                        context.SaveChanges();

                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Dibuat");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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

        public List<Dictionary<string, dynamic>> CreateGetTwo(TpEmployeeStudi value)
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
                        context.TpEmployeeStudis.Add(value);
                        context.SaveChanges();

                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Dibuat");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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

        public List<Dictionary<string, dynamic>> CreateGetThree(TpEmployeeLicense value)
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
                        context.TpEmployeeLicenses.Add(value);
                        context.SaveChanges();

                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Dibuat");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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

        public List<Dictionary<string, dynamic>> CreateGetFour(TpEmployeeRelation value)
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
                        context.TpEmployeeRelations.Add(value);
                        context.SaveChanges();

                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Dibuat");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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

        // getEdit
        public List<getEdit> getEdits(getModel value)
        {
            /*var findSessionData = userDB.FindSessionDataUser(id);
            string employeeId = findSessionData[0].EmployeeId;
*/
            var sql = "select a.*,b.department_name,c.title_name,d.religi_name  from tm_employee_affair a\r\n                     " +
                "left outer join tm_department b on a.department_id = b.department_id\r\n                     " +
                "left outer join tm_title c on a.title_id = c.title_id\r\n                     " +
                "left outer join tm_religi d on a.religi_id = d.religi_id  " +
                $"WHERE a.employee_id = '{value.EmployeeId}' order by a.employee_id asc";
            var getedit = new DatabaseContext().GetEdits.FromSqlRaw(sql).ToList();
            return getedit;
        }

        public List<ListDocument> listDocuments(string employeeId)
        {
            var sql = "select * from ( select a.*,b.document_name,case when " +
                "a.fc_document=1 then 'fotocopy' when a.fc_document=0 then 'asli' " +
                "else 'tidak jelas' end as fc_document_desc   \t\tfrom `tp_employee_affair_document` a  " +
                "inner join (select a.*, _latin1'CT000' as claim_type_id from `tm_employee_affair_general_document` a) " +
                "b on a.`document_id`=b.document_id and a.`claim_type_id`=b.claim_type_id    " +
                $"where a.employee_id = '{employeeId}' ) a  order by claim_type_id ASC, document_id asc";
            var listDocument = new DatabaseContext().ListDocuments.FromSqlRaw(sql).ToList();
            return listDocument;
        }
        //End getEdit

        public List<Dictionary<string, dynamic>> UpdateGetOne(updateGetOne value)
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();

            var current = context.TpEmployeeFamilies.Where(x => x.FamilyId == value.FamilyId).FirstOrDefault() ?? throw new Exception(value.FamilyId + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpEmployeeFamily UpdateGetOne = new TpEmployeeFamily()
                        {
                            FamilyId = value.FamilyId,
                            EmployeeId = value.EmployeeId,
                            HubunganId = value.HubunganId,
                            FamilyName = value.FamilyName,
                            TempatLahir = value.TempatLahir,
                            TanggalLahir = value.TanggalLahir,
                            NoTelp = value.NoTelp,
                            Status = 1
                        };

                        context.TpEmployeeFamilies.Entry(current).CurrentValues.SetValues(UpdateGetOne);
                        context.SaveChanges();
                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Update");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException == null ? e : e.InnerException);
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

        public List<Dictionary<string, dynamic>> UpdateGetTwo(updateGetTwo value)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();


            var current = context.TpEmployeeStudis.Where(x => x.PendidikanId == value.PendidikanId).FirstOrDefault() ?? throw new Exception(value.PendidikanId + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpEmployeeStudi UpdateGetTwo = new TpEmployeeStudi()
                        {
                            PendidikanId = value.PendidikanId,
                            EmployeeId = value.EmployeeId,
                            TypeInsId = value.TypeInsId,
                            StudiId = value.StudiId,
                            SchoolName = value.SchoolName,
                            Periode = value.Periode,
                            Akhir = value.Akhir,
                            Status = value.Status,

                            FileName = value.File.FileName,
                            Type = value.File.ContentType,
                            Size = value.File.Length
                        };

                        context.TpEmployeeStudis.Entry(current).CurrentValues.SetValues(UpdateGetTwo);
                        context.SaveChanges();
                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Update");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException == null ? e : e.InnerException);
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

        public List<Dictionary<string, dynamic>> UpdateGetThree(updateGetThree value)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();


            var current = context.TpEmployeeLicenses.Where(x => x.LicenseId == value.LicenseId).FirstOrDefault() ?? throw new Exception(value.LicenseId + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpEmployeeLicense UpdateGetThree = new TpEmployeeLicense()
                        {
                            LicenseId = value.LicenseId,
                            EmployeeId = value.EmployeeId,
                            TypeInsId = value.TypeInsId,
                            LicenseName = value.LicenseName,
                            Periode = value.Periode,
                            Status = value.Status,

                            FileName = value.File.FileName,
                            Type = value.File.ContentType,
                            Size = value.File.Length
                        };

                        context.TpEmployeeLicenses.Entry(current).CurrentValues.SetValues(UpdateGetThree);
                        context.SaveChanges();
                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Update");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException == null ? e : e.InnerException);
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

        public List<Dictionary<string, dynamic>> UpdateGetFour(updateGetFour value)
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();


            var current = context.TpEmployeeRelations.Where(x => x.RelationId == value.RelationId).FirstOrDefault() ?? throw new Exception(value.RelationId + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpEmployeeRelation UpdateGetFour = new TpEmployeeRelation()
                        {
                            RelationId = value.RelationId,
                            EmployeeId = value.EmployeeId,
                            HubunganId = value.HubunganId,
                            RelationName = value.RelationName,
                            RelationAlamat = value.RelationAlamat,
                            NoTelp = value.NoTelp,
                            Status = 1
                        };

                        context.TpEmployeeRelations.Entry(current).CurrentValues.SetValues(UpdateGetFour);
                        context.SaveChanges();
                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Update");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException == null ? e : e.InnerException);
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

        //Delete
        public IEnumerable<TmEmployeeAffair> nextFindById(string id)
        {
            var sql = databaseContext.TmEmployeeAffairs.Where(x => x.EmployeeId == id).ToList();
            return sql;
        }
        public IEnumerable<TpEmployeeFamily> getOneFindById(string id)
        {
            var sql = databaseContext.TpEmployeeFamilies.Where(x => x.FamilyId == id).ToList();
            return sql;
        }
        public IEnumerable<TpEmployeeStudi> getTwoFindById(string id)
        {
            var sql = databaseContext.TpEmployeeStudis.Where(x => x.PendidikanId == id).ToList();
            return sql;
        }
        public IEnumerable<TpEmployeeLicense> getThreeFindById(string id)
        {
            var sql = databaseContext.TpEmployeeLicenses.Where(x => x.LicenseId == id).ToList();
            return sql;
        }
        public IEnumerable<TpEmployeeRelation> getFourFindById(string id)
        {
            var sql = databaseContext.TpEmployeeRelations.Where(x => x.RelationId == id).ToList();
            return sql;
        }

        public bool getOneDelete(deleteGetOne value)
        {
            try
            {
                var current = databaseContext.TpEmployeeFamilies.Find(value.FamilyId) ?? throw new Exception("Data " + value.FamilyId + " Tidak ditemukan");
                databaseContext.TpEmployeeFamilies.Remove(current);
                databaseContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException == null ? e.Message : e.InnerException);
                return false;
            }
        }
        public bool getTwoDelete(deleteGetTwo value)
        {
            try
            {
                var current = databaseContext.TpEmployeeStudis.Find(value.PendidikanId) ?? throw new Exception("Data " + value.PendidikanId + " Tidak ditemukan");
                databaseContext.TpEmployeeStudis.Remove(current);
                databaseContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException == null ? e.Message : e.InnerException);
                return false;
            }
        }
        public bool getThreeDelete(deleteGetThree value)
        {
            try
            {
                var current = databaseContext.TpEmployeeLicenses.Find(value.LicenseId) ?? throw new Exception("Data " + value.LicenseId + " Tidak ditemukan");
                databaseContext.TpEmployeeLicenses.Remove(current);
                databaseContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException == null ? e.Message : e.InnerException);
                return false;
            }
        }
        public bool getFourDelete(deleteGetFour value)
        {
            try
            {
                var current = databaseContext.TpEmployeeRelations.Find(value.RelationId) ?? throw new Exception("Data " + value.RelationId + " Tidak ditemukan");
                databaseContext.TpEmployeeRelations.Remove(current);
                databaseContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException == null ? e.Message : e.InnerException);
                return false;
            }
        }
        //End Delete
    }

    public class getModel
    {
        public string EmployeeId { get; set; } = null!;
    }
    public class deleteGetOne
    {
        public string FamilyId { get; set; }
        public string EmployeeId { get; set; }
    }
    public class deleteGetTwo
    {
        public string PendidikanId { get; set; }
        public string EmployeeId { get; set; }
    }
    public class deleteGetThree
    {
        public string LicenseId { get; set; }
        public string EmployeeId { get; set; }
    }
    public class deleteGetFour
    {
        public string RelationId { get; set; }
        public string EmployeeId { get; set; }
    }
    public class updateEmployeeId
    {
        public string EmployeeId { get; set; }
        public string? EmployeeFirstName { get; set; }
        public string? EmployeeLastName { get; set; }
        public string DepartmentId { get; set; }
        public string TitleId { get; set; }
        public string LevelId { get; set; }
        public string EmployeeStatusId { get; set; }
        public DateTime JointDate { get; set; }
        public DateTime EndOfContract { get; set; }
        public DateTime? PermanentDate { get; set; }
        public string? TempatLahir { get; set; }
        public DateTime TanggalLahir { get; set; }
        public string? Alamat { get; set; }
        public string? AlamatTinggal { get; set; }
        public string ReligiId { get; set; }
        public string? GenderId { get; set; }
        public string? MarriedId { get; set; }
        public string? BankId { get; set; }
        public string? NoRekening { get; set; }
        public string? Email { get; set; }
        public int? FcDocument0 { get; set; }
        public int? FcDocument1 { get; set; }
        public int? FcDocument2 { get; set; }
        public int? FcDocument3 { get; set; }
        public int? FcDocument4 { get; set; }
        public int? FcDocument5 { get; set; }
        public int? FcDocument6 { get; set; }
        public int? FcDocument7 { get; set; }
        public int? FcDocument8 { get; set; }
        public int? FcDocument9 { get; set; }
        public int? FcDocument10 { get; set; }
        public int? FcDocument11 { get; set; }
        public int? FcDocument12 { get; set; }
        public IFormFile? File0 { get; set; }
        public IFormFile? File1 { get; set; }
        public IFormFile? File2 { get; set; }
        public IFormFile? File3 { get; set; }
        public IFormFile? File4 { get; set; }
        public IFormFile? File5 { get; set; }
        public IFormFile? File6 { get; set; }
        public IFormFile? File7 { get; set; }
        public IFormFile? File8 { get; set; }
        public IFormFile? File9 { get; set; }
        public IFormFile? File10 { get; set; }
        public IFormFile? File11 { get; set; }
        public IFormFile? File12 { get; set; }
    }
    public class updateGetOne
    {
        public string FamilyId { get; set; }
        public string EmployeeId { get; set; }
        public string HubunganId { get; set; }
        public string FamilyName { get; set; }
        public string TempatLahir { get; set; }
        public string TanggalLahir { get; set; }
        public string NoTelp { get; set; }
        public string Status { get; set; }
    }
    public class updateGetTwo
    {
        public string PendidikanId { get; set; }
        public string EmployeeId { get; set; }
        public string TypeInsId { get; set; }
        public string StudiId { get; set; }
        public string SchoolName { get; set; }
        public DateTime Periode { get; set; }
        public DateTime Akhir { get; set; }
        public int Status { get; set; }
        public IFormFile File { get; set; }
    }
    public class updateGetThree
    {
        public string LicenseId { get; set; }
        public string EmployeeId { get; set; }
        public string TypeInsId { get; set; }
        public string LicenseName { get; set; }
        public string Periode { get; set; }
        public int Status { get; set; }
        public IFormFile File { get; set; }
    }
    public class updateGetFour
    {
        public string RelationId { get; set; }
        public string EmployeeId { get; set; }
        public string HubunganId { get; set; }
        public string RelationName { get; set; }
        public string RelationAlamat { get; set; }
        public string NoTelp { get; set; }
        public string Status { get; set; }
    }
    public class CreateGetOneModel
    {
        public string HubunganId { get; set; }
        public string EmployeeId { get; set; }
        public string FamilyName { get; set; }
        public string TempatLahir { get; set; }
        public string TanggalLahir { get; set; }
        public string NoTelp { get; set; }
        public string Status { get; set; }
    }
    public class CreateGetTwoModel
    {
        public string EmployeeId { get; set; }
        public string TypeInsId { get; set; }
        public string StudiId { get; set; }
        public string SchoolName { get; set; }
        public DateTime Periode { get; set; }
        public DateTime Akhir { get; set; }
        public int Status { get; set; }
        public IFormFile File { get; set; }
    }
    public class CreateGetThreeModel
    {
        [Required] public string employeeId { get; set; }
        [Required] public string typeInsId { get; set; }
        [Required] public string licenseName { get; set; }
        [Required] public string periode { get; set; }
        [Required] public int status { get; set; }
        [Required] public IFormFile file { get; set; }
    }
    public class CreateGetFourModel
    {
        [Required] public string EmployeeId { get; set; }
        [Required] public string HubunganId { get; set; }
        [Required] public string RelationName { get; set; }
        [Required] public string RelationAlamat { get; set; }
        [Required] public string NoTelp { get; set; }
        [Required] public string Status { get; set; }
    }

    [Keyless]
    public class getEdit
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string? EmployeeLastName { get; set; }

        [Column("department_id")]
        public string DepartmentId { get; set; }

        [Column("title_id")]
        public string TitleId { get; set; }

        [Column("level_id")]
        public string LevelId { get; set; }

        [Column("employee_status_id")]
        public string EmployeeStatusId { get; set; }

        [Column("joint_date")]
        public DateTime? JointDate { get; set; }

        [Column("end_of_contract")]
        public DateTime? EndOfContract { get; set; }

        [Column("permanent_date")]
        public DateTime? PermanentDate { get; set; }

        [Column("tempat_lahir")]
        public string TempatLahir { get; set; }

        [Column("tanggal_lahir")]
        public DateTime TanggalLahir { get; set; }

        [Column("alamat")]
        public string? Alamat { get; set; }

        [Column("alamat_tinggal")]
        public string? AlamatTinggal { get; set; }

        [Column("religi_id")]
        public string? ReligiId { get; set; }

        [Column("gender_id")]
        public string? GenderId { get; set; }

        [Column("married_id")]
        public string? MarriedId { get; set; }

        [Column("bank_id")]
        public string? BankId { get; set; }

        [Column("no_rekening")]
        public string NoRekening { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("status")]
        public int? Status { get; set; }

        [Column("dt_etr")]
        public DateTime DtEtr { get; set; }

        [Column("user_etr")]
        public string UserEtr { get; set; }

        [Column("dt_update")]
        public DateTime DtUpdate { get; set; }

        [Column("user_update")]
        public string UserUpdate { get; set; }

        [Column("department_name")]
        public string? DepartmentName { get; set; }

        [Column("title_name")]
        public string? TitleName { get; set; }

        [Column("religi_name")]
        public string? ReligiName { get; set; }
    }

    [Keyless]
    public class ListDocument
    {
        [Column("id")]
        public string Id { get; set; }

        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("claim_type_id")]
        public string? ClaimTypeId { get; set; }

        [Column("document_id")]
        public string? DocumentId { get; set; }

        [Column("document_status")]
        public bool DocumentStatus { get; set; }

        [Column("fc_document")]
        public bool? FcDocument { get; set; }

        [Column("file_name")]
        public string? FileName { get; set; }

        [Column("size")]
        public int Size { get; set; }

        [Column("type")]
        public string? Type { get; set; }

        [Column("submitted")]
        public DateTime? Submitted { get; set; }

        [Column("submitted_by")]
        public string? SubmittedBy { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("document_name")]
        public string DocumentName { get; set; }

        [Column("fc_document_desc")]
        public string FcDocumentDesc { get; set; }
    }

    [Keyless]
    public class EmployeeGroupQuery
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string? EmployeeLastName { get; set; }

        [Column("department_id")]
        public string? DepartmentId { get; set; }

        [Column("title_id")]
        public string? TitleId { get; set; }

        [Column("level_id")]
        public string? LevelId { get; set; }

        [Column("employee_status_id")]
        public string? EmployeeStatusId { get; set; }

        [Column("joint_date")]
        public DateTime? JointDate { get; set; }

        [Column("end_of_contract")]
        public DateTime? EndOfContract { get; set; }

        [Column("permanent_date")]
        public DateTime? PermanentDate { get; set; }

        [Column("tempat_lahir")]
        public string? TempatLahir { get; set; }

        [Column("tanggal_lahir")]
        public DateTime? TanggalLahir { get; set; }

        [Column("alamat")]
        public string? Alamat { get; set; }

        [Column("alamat_tinggal")]
        public string? AlamatTinggal { get; set; }

        [Column("religi_id")]
        public string? ReligiId { get; set; }

        [Column("gender_id")]
        public string? GenderId { get; set; }

        [Column("married_id")]
        public string? MarriedId { get; set; }

        [Column("bank_id")]
        public string? BankId { get; set; }

        [Column("no_rekening")]
        public string? NoRekening { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("dt_etr")]
        public DateTime? DtEtr { get; set; }

        [Column("user_etr")]
        public string? UserEtr { get; set; }

        [Column("dt_update")]
        public DateTime? DtUpdate { get; set; }

        [Column("user_update")]
        public string? UserUpdate { get; set; }

        [Column("my_status")]
        public string? MyStatus { get; set; }

        [Column("bank_name")]
        public string? BankName { get; set; }

        [Column("department_name")]
        public string? DepartmentName { get; set; }

        [Column("title_name")]
        public string? TitleName { get; set; }

        [Column("religi_name")]
        public string? ReligiName { get; set; }

        [Column("level_name")]
        public string? LevelName { get; set; }

        [Column("gender_name")]
        public string? GenderName { get; set; }

        [Column("employee_status_name")]
        public string? EmployeeStatusName { get; set; }

        [Column("married_name")]
        public string? MarriedName { get; set; }

        [Column("termination_date")]
        public DateTime? TerminationDate { get; set; }

        [Column("tanggal_lahir_view")]
        public string? TanggalLahirView { get; set; }

        [Column("joint_date_view")]
        public string? JointDateView { get; set; }

        [Column("end_of_contract_view")]
        public string? EndOfContractView { get; set; }

        [Column("permanent_date_view")]
        public string? PermanentDateView { get; set; }

    }


    [Keyless]
    public class BloodType
    {
        [Column("blood_type_id")]
        public int BloodTypeId { get; set; }

        [Column("blood_type_name")]
        public string BloodTypeName { get; set; }

    }

    public class Departments
    {
        [Key]
        [Column("department_id")]
        public string DepartmentId { get; set; }

        [Column("department_name")]
        public string DepartmentName { get; set; }
    }

    [Keyless]
    public class Types
    {
        [Column("type_id")]
        public string TypeId { get; set; }

        [Column("type_name")]
        public string TypeName { get; set; }
    }

    [Keyless]
    public class Religions
    {
        [Column("religi_id")]
        public int ReligiId { get; set; }

        [Column("religi_name")]
        public string ReligiName { get; set; }
    }

    [Keyless]
    public class Marrieds
    {
        [Column("married_id")]
        public string MarriedId { get; set; }

        [Column("married_name")]
        public string MarriedName { get; set; }
    }

    [Keyless]
    public class Genders
    {
        [Column("gender_id")]
        public string GenderId { get; set; }

        [Column("gender_name")]
        public string GenderName { get; set; }
    }

    [Keyless]
    public class Titles
    {
        [Column("title_id")]
        public string TitleId { get; set; }

        [Column("title_name")]
        public string TitleName { get; set; }
    }

    [Keyless]
    public class Levels
    {
        [Column("level_id")]
        public string LevelId { get; set; }

        [Column("level_name")]
        public string LevelName { get; set; }
    }

    [Keyless]
    public class EmployeeStatus
    {
        [Column("employee_status_id")]
        public string EmployeeStatusId { get; set; }

        [Column("employee_status_name")]
        public string EmployeeStatusName { get; set; }
    }

    [Keyless]
    public class BankStatus
    {
        [Column("bank_id")]
        public string BankId { get; set; }

        [Column("bank_name")]
        public string BankName { get; set; }
    }

    [Keyless]
    public class ReasonStatus
    {
        [Column("reason_id")]
        public string ReasonId { get; set; }

        [Column("reason_name")]
        public string ReasonName { get; set; }
    }

    [Keyless]
    public class listFamily
    {
        [Column("family_id")]
        public string FamilyId { get; set; }

        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("hubungan_id")]
        public string HubunganId { get; set; }

        [Column("family_name")]
        public string FamilyName { get; set; }

        [Column("tempat_lahir")]
        public string? TempatLahir { get; set; }

        [Column("tanggal_lahir")]
        public string? TanggalLahir { get; set; }

        [Column("no_telp")]
        public string NoTelp { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("my_status")]
        public string MyStatus { get; set; }

        [Column("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [Column("hubungan_name")]
        public string? HubunganName { get; set; }

        [Column("tanggal_lahir_view")]
        public DateTime? TanggalLahirView { get; set; }
    }

    [Keyless]
    public class Relationship
    {
        [Column("hubungan_id")]
        public string HubunganId { get; set; }

        [Column("hubungan_name")]
        public string HubunganName { get; set; }
    }

    [Keyless]
    public class EmployeeDepartment
    {
        [Column("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string EmployeeLastName { get; set; }

        [Column("department_name")]
        public string DepartmentName { get; set; }
    }

    [Keyless]
    public class listStudi
    {
        [Column("pendidikan_id")]
        public string PendidikanId { get; set; }

        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("type_ins_id")]
        public string? TypeInsId { get; set; }

        [Column("studi_id")]
        public string StudiId { get; set; }

        [Column("school_name")]
        public string SchoolName { get; set; }

        [Column("periode")]
        public DateTime Periode { get; set; }

        [Column("akhir")]
        public DateTime Akhir { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("file_name")]
        public string? FileName { get; set; }

        [Column("size")]
        public long? Size { get; set; }

        [Column("type")]
        public string? Type { get; set; }

        [Column("my_status")]
        public string MyStatus { get; set; }

        [Column("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [Column("type_ins_name")]
        public string? TypeInsName { get; set; }

        [Column("studi_name")]
        public string StudiName { get; set; }

        [Column("periode_view")]
        public string PeriodeView { get; set; }

        [Column("akhir_view")]
        public string AkhirView { get; set; }
    }

    [Keyless]
    public class StudyId
    {
        [Column("studi_id")]
        public string StudiId { get; set; }

        [Column("studi_name")]
        public string StudiName { get; set; }
    }

    [Keyless]
    public class InsuranceType
    {
        [Column("type_ins_id")]
        public string TypeInsId { get; set; }

        [Column("type_ins_name")]
        public string TypeInsName { get; set; }
    }

    [Keyless]
    public class listLicense
    {
        [Column("license_id")]
        public string LicenseId { get; set; }

        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("type_ins_id")]
        public string TypeInsId { get; set; }

        [Column("license_name")]
        public string LicenseName { get; set; }

        [Column("periode")]
        public string Periode { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("file_name")]
        public string FileName { get; set; }

        [Column("size")]
        public int Size { get; set; }

        [Column("type")]
        public string Type { get; set; }
    }

    [Keyless]
    public class License
    {
        [Column("license_id")]
        public string LicenseId { get; set; }

        [Column("license_name")]
        public string LicenseName { get; set; }
    }

    [Keyless]
    public class listRelation
    {
        [Column("relation_id")]
        public string RelationId { get; set; }

        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("hubungan_id")]
        public string HubunganId { get; set; }

        [Column("relation_name")]
        public string RelationName { get; set; }

        [Column("relation_alamat")]
        public string RelationAlamat { get; set; }

        [Column("no_telp")]
        public string NoTelp { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("my_status")]
        public string MyStatus { get; set; }

        [Column("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [Column("hubungan_name")]
        public string HubunganName { get; set; }
    }

    public class CreateModel
    {
        public int Status { get; set; }
        public string EmployeeFirstName { get; set; }
        public string? EmployeeLastName { get; set; }
        public string DepartmentId { get; set; }
        public string TitleId { get; set; }
        public string LevelId { get; set; }
        public string EmployeeStatusId { get; set; }
        public DateTime JointDate { get; set; }
        public DateTime EndOfContract { get; set; }
        public DateTime? PermanentDate { get; set; }
        public string TempatLahir { get; set; }
        public DateTime TanggalLahir { get; set; }
        public string Alamat { get; set; }
        public string? AlamatTinggal { get; set; }
        public string ReligiId { get; set; }
        public string GenderId { get; set; }
        public string MarriedId { get; set; }
        public string? BankId { get; set; }
        public string? NoRekening { get; set; }
        public string? Email { get; set; }
        public string UserId { get; set; }
        //public string Phone { get; set; }
    }

    [Keyless]
    public class ViewData
    {
        [Column("employee_first_name")]
        public string? EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string? EmployeeLastName { get; set; }

        [Column("department_name")]
        public string? DepartmentName { get; set; }

        [Column("title_name")]
        public string? TitleName { get; set; }

        [Column("level_name")]
        public string? LevelName { get; set; }

        [Column("employee_status_name")]
        public string? EmployeeStatusName { get; set; }

        [Column("joint_date")]
        public DateTime JointDate { get; set; }

        [Column("end_of_contract")]
        public DateTime EndOfContract { get; set; }

        [Column("permanent_date")]
        public DateTime? PermanentDate { get; set; }

        [Column("tempat_lahir")]
        public string? TempatLahir { get; set; }

        [Column("tanggal_lahir")]
        public DateTime TanggalLahir { get; set; }

        [Column("alamat")]
        public string? Alamat { get; set; }

        [Column("alamat_tinggal")]
        public string? AlamatTinggal { get; set; }

        [Column("religi_name")]
        public string? ReligiName { get; set; }

        [Column("married_name")]
        public string? MarriedName { get; set; }

        [Column("gender_name")]
        public string? GenderName { get; set; }

        [Column("bank_name")]
        public string? BankName { get; set; }

        [Column("no_rekening")]
        public string? NoRekening { get; set; }

        [Column("file_name")]
        public string? FileName { get; set; }
    }

    [Keyless]
    public class viewLicense
    {
        [Column("license_id")]
        public string LicenseId { get; set; }

        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("type_ins_id")]
        public string TypeInsId { get; set; }

        [Column("license_name")]
        public string LicenseName { get; set; }

        [Column("periode")]
        public string Periode { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("file_name")]
        public string FileName { get; set; }

        [Column("size")]
        public int Size { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("my_status")]
        public string MyStatus { get; set; }

        [Column("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [Column("periode_cetak")]
        public string PeriodeCetak { get; set; }
    }

    [Keyless]
    public class viewRelation
    {
        [Column("relation_id")]
        public string RelationId { get; set; }

        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("hubungan_id")]
        public string HubunganId { get; set; }

        [Column("relation_name")]
        public string RelationName { get; set; }

        [Column("relation_alamat")]
        public string RelationAlamat { get; set; }

        [Column("no_telp")]
        public string NoTelp { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("my_status")]
        public string MyStatus { get; set; }

        [Column("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [Column("alamat")]
        public string Alamat { get; set; }
    }

    [Keyless]
    public class Documents
    {
        [Column("id")]
        public string Id { get; set; }

        [Column("employee_id")]
        public string? EmployeeId { get; set; }

        [Column("claim_type_id")]
        public string ClaimTypeId { get; set; }

        [Column("document_id")]
        public string DocumentId { get; set; }

        [Column("document_status")]
        public int DocumentStatus { get; set; }

        [Column("fc_document")]
        public int FcDocument { get; set; }

        [Column("file_name")]
        public string? FileName { get; set; }

        [Column("size")]
        public int Size { get; set; }

        [Column("type")]
        public string? Type { get; set; }

        [Column("submitted")]
        public DateTime? Submitted { get; set; }

        [Column("submitted_by")]
        public string SubmittedBy { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; }

        [Column("document_name")]
        public string DocumentName { get; set; }
    }

}
