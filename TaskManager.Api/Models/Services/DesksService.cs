using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using TaskManager.Api.Models.Abstractions;
using TaskManager.Api.Models.Data;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models.Services
{
    public class DesksService : AbstractionService, ICommonService<DeskModel>
    {
        public DesksService(ApplicationContext _db) : base(_db)
        {
            
        }

        public bool Create(DeskModel model)
        {
            bool result = DoAction(() =>
            {
                Desk newDesk = new Desk(model);
                m_db.Desks.Add(newDesk);
                m_db.SaveChanges();
            });
            return result;
        }

        public bool Delete(int id)
        {
            bool result = DoAction(() =>
            {
                Desk removeDesk = m_db.Desks.FirstOrDefault(d => d.Id == id);
                m_db.Desks.Remove(removeDesk);
                m_db.SaveChanges();
            });
            return result;
        }

        public DeskModel Get(int id)
        {
            Desk desk = m_db.Desks.Include(d => d.Tasks).FirstOrDefault(d => d.Id == id);
            var deskModel = desk?.ToDto();
            if (deskModel != null)
            {
                deskModel.TaskIds = desk?.Tasks.Select(t => t.Id).ToList();
            }
            return deskModel;
        }

        public bool Update(int id, DeskModel model)
        {
            bool result = DoAction(() =>
            {
                Desk desk = m_db.Desks.FirstOrDefault(d => d.Id == id);
                desk.Name = model.Name;
                desk.Description = model.Description;
                desk.Photo = model.Photo;
                desk.AdminId = model.AdminId;
                desk.IsPrivate = model.IsPrivate;
                desk.ProjectId = model.ProjectId;
                desk.Columns = JsonConvert.SerializeObject(model.Columns);
                m_db.Desks.Update(desk);
                m_db.SaveChanges();
            });
            return result;
        }

        public IQueryable<CommonModel> GetAll(int adminId)
        {
            return m_db.Desks.Where(d => d.AdminId == adminId).Select(d => d.ToDto() as CommonModel);
        }

        public IQueryable<CommonModel> GetProjectDesks(int projectId, int adminId)
        {
            return m_db.Desks
                .Where(d => d.ProjectId == projectId && 
                    (d.AdminId == adminId || !d.IsPrivate)
                ).Select(d => d.ToDto() as CommonModel);
        }
    }
}
