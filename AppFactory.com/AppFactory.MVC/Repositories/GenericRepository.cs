﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using AppFactory.DAL;

namespace AppFactory.MVC.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        DbSet<TEntity> dbSet;
        private AppFactoryEntities dbContext;
        public GenericRepository(AppFactoryEntities dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<TEntity>();
        }

        public TEntity Add(TEntity entity)
        {
            dbSet.Add(entity);
            return entity;
        }

        public TEntity Delete(TEntity entity)
        {
            if (dbContext.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
            return entity;
        }

        public TEntity Edit(TEntity entity)
        {
            //dbSet.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return dbSet.ToList();
        }

        public IEnumerable<TEntity> GetALLByPage(int pageNumber, int pageSize)
        {
            IEnumerable<TEntity> result =
                dbSet.ToList()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            return result;
        }

        public IEnumerable<TEntity> GetBy(Expression<Func<TEntity, bool>> filter = null)
        {
            if (filter != null)
            {
                return dbSet.Where(filter);
            }
            return dbSet.ToList();
        }
    }
}
