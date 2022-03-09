using Gordon360.Database.CCT;
using Gordon360.Database.MyGordon;
using Gordon360.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Gordon360.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T: class
    {
        /// <summary>
        ///     The database context for the repository
        /// </summary>
        private readonly CCTContext _context;
        private readonly MyGordonContext _myGordonCtx;

        /// <summary>
        ///     The data set of the repository
        /// </summary>
        private readonly DbSet<T> _dbSet;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GenericRepository{T}" /> class.
        /// </summary>
        /// <param name="context">The context for the repository</param>
        public GenericRepository(CCTContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GenericRepository{T}" /> class.
        /// </summary>
        /// <param name="context">The context for the repository</param>
        public GenericRepository(MyGordonContext context)
        {
            _myGordonCtx = context;
            _dbSet = _myGordonCtx.Set<T>();
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
        /// Gets single entity by username
        /// </summary>
        /// <returns>All entities</returns>
        public T GetByUsername(string username)
        {
            return _dbSet.Find(username);
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
        ///     Attaches a given entity to the context
        /// </summary>
        /// <param name="entity">The entity to attach</param>
        public void Attach(T entity)
        {
            _dbSet.Attach(entity);
        }

        ///// <summary>
        ///// Executes a stored procedure
        ///// </summary>
        ///// <param name="query">Name of the stored procedure </param>
        ///// <param name="parameters">Parameters to pass to the stored procedure</param>
        //public IEnumerable<T> ExecWithStoredProcedure(string query, params object[] parameters)
        //{
        //    return _context.Database.SqlQuery<T>(query, parameters);
        //}

        


    }
}
