using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.DAL.Repositories
{
	public interface IAsyncRepository<TEntity>
		where TEntity : class
	{
		Task<IEnumerable<TEntity>> GetAll();
		Task<TEntity?> FindById(long id);
		Task Add(TEntity entity, bool autoSave = true);
		Task Update(TEntity entity, bool autoSave = true);
		Task Delete(TEntity entity, bool autoSave = true);
		Task SaveChanges();
	}
}
