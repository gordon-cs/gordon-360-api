using CCT_App.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace CCT_App.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T: class
    {
        /// <summary>
        ///     The database context for the repository
        /// </summary>
        private readonly CCTEntities _context;

        /// <summary>
        ///     The data set of the repository
        /// </summary>
        private readonly IDbSet<T> _dbSet;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GenericRepository{T}" /> class.
        /// </summary>
        /// <param name="context">The context for the repository</param>
        public GenericRepository(CCTEntities context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        ///     Gets single entity by id
        /// </summary>
        /// <returns>All entities</returns>
        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        /// <summary>
        ///     Gets single entity by id
        /// </summary>
        /// <returns>All entities</returns>
        public T GetById(string id)
        {
            return _dbSet.Find(id);
        }

        /// <summary>
        ///     Gets all entities
        /// </summary>
        /// <returns>All entities</returns>
        public IEnumerable<T> GetAll()
        {
            return _dbSet;
        }

        /// <summary>
        ///     Gets all entities matching the predicate
        /// </summary>
        /// <param name="predicate">The filter clause</param>
        /// <returns>All entities matching the predicate</returns>
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        /// <summary>
        ///     Set based on where condition
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <returns>The records matching the given condition</returns>
        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        /// <summary>
        ///     Finds an entity matching the predicate
        /// </summary>
        /// <param name="predicate">The filter clause</param>
        /// <returns>An entity matching the predicate</returns>
        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        /// <summary>
        ///     Determines if there are any entities matching the predicate
        /// </summary>
        /// <param name="predicate">The filter clause</param>
        /// <returns>True if a match was found</returns>
        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }

        /// <summary>
        ///     Returns the first entity that matches the predicate
        /// </summary>
        /// <param name="predicate">The filter clause</param>
        /// <returns>An entity matching the predicate</returns>
        public T First(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.First(predicate);
        }

        /// <summary>
        ///     Returns the first entity that matches the predicate else null
        /// </summary>
        /// <param name="predicate">The filter clause</param>
        /// <returns>An entity matching the predicate else null</returns>
        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        /// <summary>
        ///     Adds a given entity to the context
        /// </summary>
        /// <param name="entity">The entity to add to the context</param>
        public T Add(T entity)
        {
            return _dbSet.Add(entity);
        }

        /// <summary>
        ///     Deletes a given entity from the context
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteCollection(T entity, string collectionName, ICollection<object> collectionToRemove)
        {
            var objectContext = (_context as IObjectContextAdapter).ObjectContext;

            while (collectionToRemove.Any())
                objectContext.DeleteObject(collectionToRemove.First());

            //_context.Entry(entity).Collection(collectionName).Load();
            //var list = _context.Entry(entity).Collection(collectionName).EntityEntry.CurrentValues;
            //while (list.Any())
            //    list.Remove(collectionToRemove.First());
        }

        /// <summary>
        ///     Attaches a given entity to the context
        /// </summary>
        /// <param name="entity">The entity to attach</param>
        public void Attach(T entity)
        {
            _dbSet.Attach(entity);
        }

        /// <summary>
        /// Executes a stored procedure
        /// </summary>
        /// <param name="query">Name of the stored procedure </param>
        /// <param name="parameters">Parameters to pass to the stored procedure</param>
        public IEnumerable<T> ExecWithStoredProcedure(string query, params object[] parameters)
        {
            return _context.Database.SqlQuery<T>(query, parameters);
        }


    }
}
