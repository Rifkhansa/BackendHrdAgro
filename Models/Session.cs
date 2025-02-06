using Microsoft.EntityFrameworkCore;
using BackendHrdAgro.Models.Database.MySql.Master;
using BackendHrdAgro.Models.Database.MySql;

namespace BackendHrdAgro.Models
{
    public class SessionDB
    {
        BaseModel baseModel = new BaseModel();
        DatabaseContext context = new DatabaseContext();

        public string sessionGet(string userId)
        {
            var sql = from session in context.LoginSessions
                      where session.UserId == userId
                      select session;
            var sessionList = sql.ToList();
            if (sessionList == null || sessionList.Count == 0)
            {
                CreateSession(userId: userId);
            }
            var token = BaseModel.generateToken();
            UpdateSession(token: token, userId: userId);
            return token;
        }

        public void UpdateSession(string token, string userId)
        {
            try
            {
                using DatabaseContext context = new DatabaseContext();
                var session = context.LoginSessions.Where(x => x.UserId.Equals(userId)).FirstOrDefault() ?? throw new Exception("Session not found");
                session.AccessToken = token;
                session.UpdatedAt = DateTime.Now;
                context.LoginSessions.Update(session);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CreateSession(string userId)
        {
            try
            {
                context.LoginSessions.Add(new LoginSession
                {
                    CreatedAt = DateTime.Now,
                    UserId = userId
                });
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
