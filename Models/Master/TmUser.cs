using Microsoft.EntityFrameworkCore;
using BackendHrdAgro.Models.Database.MySql;

namespace BackendHrdAgro.Models.Master
{
    public class UserDB
    {
        SessionDB sessionDB = new SessionDB();
        DatabaseContext context = new DatabaseContext();
        public List<Dictionary<string,dynamic>> retriveProfile(AuthLoginModel value)
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            data.Add("user", string.Empty);
            data.Add("token", string.Empty);
            data.Add("userDetails", string.Empty);
            data.Add("role", string.Empty);

            result.Add(data);

            try
            {

                var sqlSignIn = from user in context.TmUsers
                                where user.EmployeeId == value.EmployeeId && user.Password == value.Password
                                select new
                                {
                                    user.UserId,
                                    user.UserName,
                                    GroupId = user.GroupId == null ? "b" : user.GroupId
                                };

                var users = sqlSignIn.ToList();
                if (users == null || users.Count == 0)
                {
                    Console.WriteLine("Mass");
                    return result;
                }

                var retUser = context.TmUsers.Where(e => e.EmployeeId == value.EmployeeId && e.Password == value.Password).ToList();
               // var retGroup = context.TmGroups.ToList();
                var retThirdParty = context.TmThirdParties.ToList();

                /*var retProfile = from u in retUser
                                 join g in retGroup on u.GroupId equals g.Id into grp
                                 from g in grp.DefaultIfEmpty()
                                 select new
                                 {
                                     u.UserId,
                                     u.UserFullName,
                                     u.UserName,
                                     u.Email,
                                     GroupName = g == null ? null : g.Name,
                                     u.LastChangePassword
                                 };*/

                /*var retriveProfile = retProfile.ToList();
                var userFind = retriveProfile.Find(e => e.UserId == e.UserId);

                var sqlUserRole = from role in context.TpUserRoles
                                  join mRole in context.TmRoles on role.RoleId equals mRole.Id
                                  where role.UserId == userFind.UserId
                                  select new
                                  {
                                      role.RoleId
                                  };

                var roles = sqlUserRole.ToList();*/

               // var accessToken = sessionDB.sessionGet(value.EmployeeId);

                data.Clear();
                data.Add("user", users);
                //data.Add("token", accessToken);
                //data.Add("token", "");
                //data.Add("userDetails", userFind);
                //data.Add("role", roles);

                result.Clear();
                result.Add(data);

                return result;

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        public string? retriveUserId(string userId) => new DatabaseContext().TmUsers.Where(x => x.UserId.Equals(userId)).Select(x=> x.UserId).FirstOrDefault();

        /*fauzan*/
        public List<FindSessionDataQuery> FindSessionDataUser(string userId)
        {

            string sql = $"select  a.user_id,b.employee_id, a.user_full_name, b.department_id,b.title_id,b.level_id,c.div_id, c.department_name " +
                $"from tm_users a " +
                $"inner join tm_employee_affair b on a.employee_id = b.employee_id " +
                $"inner join tm_department c on b.department_id = c.department_id " +
                $"where a.user_id='{userId}'";
            var list = new DatabaseContext().FindSessionDataQueries.FromSqlRaw(sql).ToList();
            return list;
        }

        //samsul
        //public List<FindSessionDataQuery> FindSessionUserByPhone(string phone)
        //{

        //    string sql = $"select  a.user_id,b.employee_id, a.user_full_name, b.department_id,b.title_id,b.level_id,c.div_id, c.department_name " +
        //        $"from tm_users a " +
        //        $"inner join tm_employee_affair b on a.employee_id = b.employee_id " +
        //        $"inner join tm_department c on b.department_id = c.department_id " +
        //        $"where a.phone='{phone}'";
        //    var list = new DatabaseContext().FindSessionDataQueries.FromSqlRaw(sql).ToList();
        //    return list;
        //}

        public List<FindSessionDataQuery> FindSessionDataByEmployeeId(string employeeId)
        {

            string sql = $"select  a.user_id,b.employee_id, a.user_full_name, b.department_id,b.title_id,b.level_id,c.div_id, c.department_name " +
                $"from tm_users a " +
                $"inner join tm_employee_affair b on a.employee_id = b.employee_id " +
                $"inner join tm_department c on b.department_id = c.department_id " +
                $"where b.employee_id='{employeeId}'";
            var list = new DatabaseContext().FindSessionDataQueries.FromSqlRaw(sql).ToList();
            return list;
        }
        /*public List<FindEmployeeIdByNumberWA> FindEmployeeIdByNumberWA(string phoneNumber)
        {

            string sql = $"SELECT employee_id, user_id FROM tm_users where phone = '{phoneNumber}' ";
            var list = new DatabaseContext().FindEmployeeIdByNumberWA.FromSqlRaw(sql).ToList();
            return list;
        }*/
        public List<FindRequestData> FindRequestData(string table, string fildName, string requestId)
        {

            string sql = $"SELECT employee_id FROM {table} where {fildName} = '{requestId}' ";
            var list = new DatabaseContext().FindRequestDatas.FromSqlRaw(sql).ToList();
            return list;
        }
        //public int CountUserByWA(string WhatsappNumber) => new DatabaseContext().TmUsers.Where(x => x.Phone.Equals(WhatsappNumber)).Count();


        /*fauzan*/


    }
}
