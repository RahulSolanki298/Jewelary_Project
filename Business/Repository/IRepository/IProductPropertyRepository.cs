﻿using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface IProductPropertyRepository
    {
        Task<IEnumerable<ProductProperty>> GetMainPropertyList();
        Task<IEnumerable<ProductProperty>> GetProductPropertyList();
        Task<ProductProperty> GetProductPropertyById(int Id);
        Task<bool> DeleteProductProperty(int Id);
        Task<ProductProperty> SaveProductProperty(ProductProperty productProperty, int producutPropertyId = 0);
    }
}
