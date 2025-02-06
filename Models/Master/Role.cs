using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using BackendHrdAgro.Models.Database.MySql;

namespace BackendHrdAgro.Models.Master
{
    public class RolesDB
    {
        public List<TmRole> RoleFind() => new DatabaseContext().TmRoles.Where(x => x.DeletedAt == null && x.DeletedBy == null).ToList();
        public List<TmRole> RoleFind(string id) => new DatabaseContext().TmRoles.Where(x => x.DeletedAt == null && x.DeletedBy == null && x.Id.Equals(id)).ToList();
        public List<TpRoleMenu> RoleAllUserRole(string roleId)=> new DatabaseContext().TpRoleMenus.Where(x => x.RoleId == roleId && x.DeletedAt == null && x.DeletedBy == null).ToList();
        public List<Dictionary<string, dynamic>> CreateRole(TmRole value)
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
                        context.TmRoles.Add(value);
                        context.SaveChanges();

                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Membuat Role");

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

        public List<Dictionary<string, dynamic>> UpdateRole(RoleCreateModel value, string id)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();


            var current = context.TmRoles.Where(x => x.Id == value.Id).FirstOrDefault() ?? throw new Exception(value.Name + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TmRole roleUpdateRole = new TmRole()
                        {
                            Id = value.Id,
                            Name = value.Name,
                            Description = value.Description,
                            CreatedBy = current.CreatedBy,
                            CreatedAt = current.CreatedAt,
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = id
                        };

                        //context.TmRoles.Update(roleUpdateRole);
                        context.TmRoles.Entry(current).CurrentValues.SetValues(roleUpdateRole); //fauzan merubah
                        context.SaveChanges();
                        transaction.Commit();


                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil update Role");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                        transaction.Rollback();
                        //throw;
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

        public List<Dictionary<string, dynamic>> DeleteRole(RoleIdModel value, string id)
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
                        var current = context.TmRoles.Where(x => x.Id == value.Id).FirstOrDefault() ?? throw new Exception(value.Id + " not found");
                        TmRole delete = new TmRole()
                        {
                            Id = value.Id,
                            Name = current.Name,
                            Description = current.Description,
                            CreatedAt = current.CreatedAt,
                            CreatedBy = current.CreatedBy,
                            DeletedAt = DateTime.Now,
                            DeletedBy = id
                        };
                        //context.TmRoles.Update(delete);
                        context.TmRoles.Entry(current).CurrentValues.SetValues(delete); //fauzan merubah
                        context.SaveChanges();

                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil menghapus Role");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                        transaction.Rollback();
                        //throw;
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



        public List<Dictionary<string, dynamic>> ChangePermission(List<string> insert, List<string> delete, string roleId, string id)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();

            //bool myResult = false;
            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    List<TpRoleMenu> insertRoleMenuList = new List<TpRoleMenu>();
                    List<TpRoleMenu> deleteRoleMenuList = new List<TpRoleMenu>();
                    try
                    {
                        foreach (var i in insert)
                        {
                            TpRoleMenu menu = new TpRoleMenu()
                            {
                                RoleId = roleId,
                                MenuId = i,
                                CreatedAt = DateTime.Now,
                                CreatedBy = id
                            };

                            insertRoleMenuList.Add(menu);
                        }
                        context.TpRoleMenus.AddRangeAsync(insertRoleMenuList);
                        context.SaveChanges();
                        foreach (var i in delete)
                        {
                            context.TpRoleMenus.Remove(context.TpRoleMenus.Where(x => x.MenuId == i && x.RoleId == roleId).FirstOrDefault());
                        }
                        context.SaveChanges();


                        //context.Database.ExecuteSqlRaw($"DELETE FROM tm_users WHERE id='45'"); // ngetest rollback, field id tidak dikenal
                        //context.SaveChanges();

                        transaction.Commit();


                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil merubah permisi");

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

    public class RoleCreateModel
    {
        public string? Id { get; set; }
        [Required] public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class RoleIdModel
    {
        public string Id { get; set; }
    }

    public class RoleChangePermissionRoleModel
    {
        [Required]
        public string RoleId { get; set; }
        [Required]
        public string[] Permission { get; set; }

    }

}
