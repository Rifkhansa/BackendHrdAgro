using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using BackendHrdAgro.Models.Database.MySql;

namespace BackendHrdAgro.Models.Master
{
    public class MenuDB
    {
        public IEnumerable<Object> MenuFindAll()
        {
            using DatabaseContext context = new DatabaseContext();
            var sql = from menu in context.TmMenus
                      where menu.DeletedAt == null && menu.DeletedBy == null
                      join menus in context.TmMenus on menu.Parent equals menus.Id into menusLeft
                      from menus in menusLeft.DefaultIfEmpty()
                      orderby menu.Parent, menu.Hierarchy, menu.Name ascending
                      select new
                      {
                          menu.Id,
                          menu.Name,
                          menu.Caption,
                          menu.Url,
                          menu.Icon,
                          menu.Parent,
                          menu.Hierarchy,
                          ParentName = menus.Name
                      };
            var menuList = sql.ToList();
            return menuList;
        }

        public IEnumerable<Object> MenuFindById(string id)
        {
            using DatabaseContext context = new DatabaseContext();
            var sql = from menu in context.TmMenus
                      where menu.DeletedAt == null && menu.DeletedBy == null && menu.Id == id
                      join menus in context.TmMenus on menu.Parent equals menus.Id into menusLeft
                      from menus in menusLeft.DefaultIfEmpty()
                      orderby menu.Parent, menu.Hierarchy, menu.Name ascending
                      select new
                      {
                          menu.Id,
                          menu.Name,
                          menu.Caption,
                          menu.Url,
                          menu.Icon,
                          menu.Parent,
                          menu.Hierarchy,
                          ParentName = menus.Name
                      };
            var menuList = sql.ToList();
            return menuList;
        }

        public IEnumerable<Object> MenuFindParent()
        {
            using DatabaseContext context = new DatabaseContext();
            var sql = from menu in context.TmMenus
                      where menu.DeletedAt == null && menu.DeletedBy == null && menu.Url == "#"
                      join menus in context.TmMenus on menu.Parent equals menus.Id into menusLeft
                      from menus in menusLeft.DefaultIfEmpty()
                      orderby menu.Parent, menu.Hierarchy
                      select new
                      {
                          menu.Id,
                          menu.Name,
                          menu.Caption,
                          menu.Url,
                          menu.Icon,
                          menu.Parent,
                          menu.Hierarchy,
                          menu.CreatedBy,
                          menu.CreatedAt,
                          menu.UpdatedBy,
                          menu.UpdatedAt,
                          menu.DeletedBy,
                          menu.DeletedAt,
                          ParentName = menus.Name
                      };
            var parentList = sql.ToList();
            return parentList;
        }

        public void menuCreate(TmMenu value)
        {
            try
            {
                using DatabaseContext context = new DatabaseContext();
                context.TmMenus.Add(value);
                context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void menuUpdate(TmMenu value)
        {
            try
            {
                using DatabaseContext context = new DatabaseContext();
				var menu = context.TmMenus.Where(x => x.Id == value.Id).FirstOrDefault() ?? throw new Exception();
				menu.Name = value.Name;
				menu.Caption = value.Caption;
				menu.Url = value.Url;
				menu.Icon = value.Icon;
				menu.Parent = value.Parent;
				menu.Hierarchy = value.Hierarchy;
				menu.UpdatedAt = DateTime.Now;
				context.SaveChanges();
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void menuDelete(DeleteMenuModel value)
        {
            try
            {
                using DatabaseContext context = new DatabaseContext();
                var current = context.TmMenus.Find(value.Id);
                TmMenu delete = new TmMenu()
                {
                    Id = value.Id,
                    Name = current.Name,
                    Caption = current.Caption,
                    Url = current.Url,
                    Icon = current.Icon,
                    Parent = current.Parent,
                    Hierarchy = current.Hierarchy,
                    CreatedAt = current.CreatedAt,
                    CreatedBy = current.CreatedBy,
                    UpdatedAt = current.UpdatedAt,
                    UpdatedBy = current.UpdatedBy,
                    DeletedAt = value.DeletedAt,
                    DeletedBy = value.DeletedBy
                };
                context.TmMenus.Entry(current).CurrentValues.SetValues(delete);
                context.SaveChanges();
            }catch(Exception e)
            {
                throw;
            }
        }

        public string generateMenu(string parent, string userId, string setMenuParent, string setMenuChild, string baseUrl)
        {
            using DatabaseContext context = new DatabaseContext();
            Console.WriteLine($" parent {parent}");
            var menu = string.Empty;
            var critParent = string.Empty;
            var allowedMenu = new UsersDB().userRole(userId).ToList();
            var allow = new List<string>();
            string alloww = string.Empty;
            foreach (var roles in allowedMenu)
            {
                foreach (var role in roles.Value)
                {
                    allow.Add(role);
                }

            }
            var allowh = string.Join("','", allow);
            Console.WriteLine(allowh);
            List<TmMenu> Menus = new List<TmMenu>();
            if (parent == null)
            {
                Console.WriteLine("null parent");
                var sql = from i in context.TmMenus
                          where i.DeletedAt == null && i.DeletedBy == null && i.Parent == null
                          orderby i.Parent, i.Hierarchy, i.Caption, i.Name ascending
                          select i;
                Menus = sql.ToList();
            }
            else
            {
                Console.WriteLine("available parent");
                var sql = from i in context.TmMenus
                          where i.DeletedAt == null && i.DeletedBy == null && i.Parent == parent
                          orderby i.Parent, i.Hierarchy, i.Caption, i.Name ascending
                          select i;
                Menus = sql.ToList();
            }

            try
            {
                foreach (var res in Menus)
                {
                    if (res.Url != "#")
                    {
                        var url = baseUrl + res.Url;
                        if (setMenuChild == res.Caption)
                        {
                            menu += $"<li class='nav-active'><a class='nav-link' href='{url}'><i class='{res.Icon}' aria-hidden='true'></i><span>{res.Caption}</span></a></li>";
                        }
                        else
                        {
                            menu += $"<li><a class='nav-link' href='{url}'><i class='{res.Icon}' aria-hidden='true'></i><span>{res.Caption}</span></a></li>";
                        }
                    }
                    else
                    {
                        if (setMenuParent == res.Caption)
                        {
                            menu += $"<li class='nav-parent nav-expanded nav-active'><a class='nav-link' href='#'><i class='{res.Icon}' aria-hidden='true'></i><span>{res.Caption}</span></a>";
                        }
                        else
                        {
                            menu += $"<li class='nav-parent'><a class='nav-link' href='#'><i class='{res.Icon}' aria-hidden='true'></i><span>{res.Caption}</span></a>";
                        }
                    }
                    if (parent != res.Id && res.Url == "#")
                    {
                        var men = generateMenu(
                            parent: res.Id, 
                            userId: userId, 
                            setMenuParent: res.Parent!, 
                            setMenuChild: res.Caption!, 
                            baseUrl: baseUrl);
                        menu += $"<ul class='nav nav-children'>{men}</ul>";
                        menu += "</li>";
                        Console.WriteLine(menu);
                    }
                }

                return menu;

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

    }

    public partial class DeleteMenuModel
    {
        [Key]
        [Column("id")]
        [StringLength(21)]
        [Unicode(false)]
        public string? Id { get; set; } = null!;
        [Column("deleted_at", TypeName = "datetime")]
        public DateTime? DeletedAt { get; set; }
        [Column("deleted_by")]
        [StringLength(21)]
        [Unicode(false)]
        public string? DeletedBy { get; set; }

    }
    public partial class GenerateMenu
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = null!;
        [Column("name")]
        public string? Name { get; set; }
        [Column("caption")]
        public string? Caption { get; set; }
        [Column("url")]
        public string? Url { get; set; }
        [Column("icon")]
        public string? Icon { get; set; }
        [Column("Parent")]
        public string? Parent { get; set; }
        [Column("hierarchy")]
        public int? Hierarchy { get; set; }
    }

    public class SubMenu
    {
        public string Name { get; set; } = null!;
        public string Caption { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string Icon { get; set; } = null!;
        public int? Index { get; set; }
    }

    public class MainMenu
    {
        public string Name { get; set; } = null!;
        public string Caption { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string Icon { get; set; } = null!;
        public int? Index { get; set; }
        public List<SubMenu> SubMenu { get; set; } = null!;
    }

}
