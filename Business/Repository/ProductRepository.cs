﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Business.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _context;
        public ProductRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductList()
        {
            var products = new List<ProductDTO>();
            products = await (from product in _context.Product
                              join cat in _context.Category on product.CategoryId equals cat.Id
                              join subcat in _context.SubCategory on product.SubCategoryId equals subcat.Id
                              join color in await GetColorList() on product.ColorId equals color.Id
                              join carat in await GetCaratList() on product.CaratId equals carat.Id
                              join shape in await GetShapeList() on product.ShapeId equals shape.Id
                              join clarity in await GetClarityList() on product.ClarityId equals clarity.Id
                              select new ProductDTO
                              {
                                  Id = product.Id,
                                  Title = product.Title,
                                  CaratId = carat.Id,
                                  CaratName = carat.Name,
                                  CategoryId = cat.Id,
                                  CategoryName = cat.Name,
                                  ColorId = color.Id,
                                  ColorName = color.Name,
                                  SubCategoryId = subcat.Id,
                                  SubCategoryName = subcat.Name,
                                  ClarityId = clarity.Id,
                                  ClarityName = clarity.Name,
                                  ShapeName = shape.Name,
                                  ShapeId = shape.Id,
                                  UnitPrice = product.UnitPrice,
                                  Price = product.Price,
                                  IsActivated = product.IsActivated,
                                  CaratSizeId=product.CaratSizeId,
                                  CaratSizeName=carat.Name,
                                  Description=product.Description,
                                  Sku=product.Sku,
                                  ProductType=cat.ProductType,
                                  StyleId=product.StyleId
                              }).ToListAsync();

            return products;

        }

        public async Task<int> GetColorId()
        {
            var colorDT = await _context.ProductProperty.Where(static x => x.Name == "Metal").FirstOrDefaultAsync();
            return colorDT.Id;
        }

        public async Task<List<ProductProperty>> GetColorList()
        {
            int metalId = await GetColorId();
            var result = await _context.ProductProperty.Where(x => x.ParentId == metalId).ToListAsync();
            return result;
        }

        public async Task<int> GetCaratId()
        {
            var caratDT = await _context.ProductProperty.Where(static x => x.Name == "Carat").FirstOrDefaultAsync();
            return caratDT.Id;
        }

        public async Task<List<ProductProperty>> GetCaratList()
        {
            int caratId = await GetCaratId();
            var result = await _context.ProductProperty.Where(x => x.ParentId == caratId).ToListAsync();
            return result;
        }

        public async Task<int> GetShapeId()
        {
            var shapeDT = await _context.ProductProperty.Where(static x => x.Name == "Shape").FirstOrDefaultAsync();
            return shapeDT.Id;
        }

        public async Task<List<ProductProperty>> GetShapeList()
        {
            int shapeId = await GetShapeId();
            var result = await _context.ProductProperty.Where(x => x.ParentId == shapeId).ToListAsync();
            return result;
        }

        public async Task<int> GetClarityId()
        {
            var clarityDT = await _context.ProductProperty.Where(static x => x.Name == "Clarity").FirstOrDefaultAsync();
            return clarityDT.Id;
        }

        public async Task<List<ProductProperty>> GetClarityList()
        {
            int clarityId = await GetClarityId();
            var result = await _context.ProductProperty.Where(x => x.ParentId == clarityId).ToListAsync();
            return result;
        }

    }
}
