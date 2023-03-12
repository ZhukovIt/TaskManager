using System;
using TaskManager.Api.Models.Data;

namespace TaskManager.Api.Models.Abstractions
{
    public abstract class AbstractionService
    {
        protected ApplicationContext m_db;

        protected AbstractionService(ApplicationContext db)
        {
            m_db = db;
        }

        protected bool DoAction(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
