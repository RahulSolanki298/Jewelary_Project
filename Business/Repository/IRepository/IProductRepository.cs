﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities;
using Models;

namespace Business.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<List<ProductProperty>> GetColorList();
        Task<List<ProductProperty>> GetCaratList();
        Task<List<ProductProperty>> GetShapeList();
        Task<List<ProductProperty>> GetClarityList();
        Task<IEnumerable<ProductDTO>> GetProductCollectionList();
        Task<IEnumerable<ProductDTO>> GetProductStyleList();
        Task<bool> SaveProductList(List<ProductDTO> products);
        Task<bool> SaveProductCollectionList(List<ProductDTO> products);
        Task<List<ProductProperty>> GetGoldWeightList();
        Task<List<ProductProperty>> GetGoldPurityList();
        Task<bool> SaveImageVideoAsync(ProductImages ImgVdoData);
        Task<IEnumerable<Product>> GetProductCollectionNewList();
        Task<bool> SaveNewProductCollectionList(List<ProductDTO> products);
        Task<int> SaveImageVideoPath(string imgVdoPath);
        FileSplitDTO ExtractStyleName(string fileName);

        Task<Product> GetProductByDesignNo(string designNo);
    }
}
