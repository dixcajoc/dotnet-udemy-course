using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using AutoMapper;

namespace ApiEcommerce.Mapping;

public class ProductProfile: Profile
{
    public ProductProfile()
    {
        // Al convertir Product a ProductDto, obtiene el nombre de la categoría relacionada
        // (Product.Category.Name) y lo asigna a CategoryName; ReverseMap permite también
        // convertir de ProductDto a Product.
        // dest representa el DTO de destino; src es el Product de origen. MapFrom indica
        // a AutoMapper que lea src.Category.Name y asigne ese valor a dest.CategoryName.
        CreateMap<Product, ProductDto>()
        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
        .ReverseMap();
        CreateMap<Product, CreateProductDto>().ReverseMap();
        CreateMap<Product, UpdateProductDto>().ReverseMap();
    }


}
