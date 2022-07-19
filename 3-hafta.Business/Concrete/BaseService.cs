﻿using _3_hafta.Business.Abstract;
using AutoMapper;
using Core.DataAccess;
using Core.Entity;
using Core.Utilities.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_hafta.Business.Concrete
{
    public class BaseService<TEntity, TDto> : IBaseService<TEntity, TDto>
        where TEntity : class, IEntity, new()
        where TDto : class, IEntity, new()
    {
        private readonly IEntityRepository<TEntity> _entityRepository;
        protected readonly IMapper Mapper;
        public BaseService(IEntityRepository<TEntity> entityRepository, IMapper mapper)
        {
            _entityRepository = entityRepository;
            Mapper = mapper;
        }

        public async Task<IResult> AddAsync(TDto entity)
        {
            TEntity addedEntity = Mapper.Map<TEntity>(entity);
            bool result = await _entityRepository.AddAsync(addedEntity);
            if (result)
                return new SuccessResult();
            return new ErrorResult();
        }

        public async Task<IResult> UpdateAsync(int id, TDto entity)
        {
            TEntity updatedEntity = await _entityRepository.GetByIdAsync(id);
            if (updatedEntity is null)
                return new ErrorResult();
            Mapper.Map(entity, updatedEntity);
            bool result = await _entityRepository.UpdateAsync(updatedEntity);
            if (result)
                return new SuccessResult();
            return new ErrorResult();

        }

        public async Task<IResult> DeleteAsync(int id)
        {
            TEntity deletedEntity = await _entityRepository.GetByIdAsync(id);
            if (deletedEntity is null)
                return new ErrorResult();
            bool result = await _entityRepository.DeleteAsync(deletedEntity);
            if (result)
                return new SuccessResult();
            return new ErrorResult();
        }

        public async Task AddRangeAsync(IEnumerable<TDto> entities)
        {
            var addedEntities = Mapper.Map<IEnumerable<TEntity>>(entities);
            await _entityRepository.AddRangeAsync(addedEntities);
        }

        public async Task<IDataResult<List<TDto>>> GetListAsync()
        {
            var result = await _entityRepository.GetAllAsync();
            var resultDtos = Mapper.Map<List<TDto>>(result);
            return new SuccessDataResult<List<TDto>>(resultDtos);
        }

        public async Task<IDataResult<TDto>> GetByIdAsync(int id)
        {
            var result = await _entityRepository.GetByIdAsync(id);
            if (result is null)
                return new ErrorDataResult<TDto>();
            var resultDto = Mapper.Map<TDto>(result);
            return new SuccessDataResult<TDto>(resultDto);
        }
    }
}
